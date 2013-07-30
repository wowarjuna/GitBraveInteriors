Imports includes
Imports MySql.Data.MySqlClient
Imports System.Data
Imports System.IO
Imports System.Net
Partial Class transactionRegistration
    Inherits System.Web.UI.Page
    '**************************************************************************************************
    ' Description
    ' ===========
    '
    ' This page performs 3 main functions:
    '	(1) Registers the transaction details with Server
    '	(2) Stores the order details, transaction IDs and security keys for this transaction in the database
    '	(3) Redirects the user to the payment pages, or handles errors if the registration fails
    ' If the kit is in SIMULATOR mode, everything is shown on the screen and the user asked to Proceed
    ' to the payment pages.  In Test and Live mode, nothing is echoed to the screen and the browser
    ' is automatically redirected to the payment pages.
    '**************************************************************************************************

    '** Check we have a cart in the session.  If not, go back to the buildOrder page to get one **
    Public strCart As String
    Public strCustomerEMail As String = ""
    Public strBillingFirstnames As String = ""
    Public strBillingSurname As String = ""
    Public strBillingAddress1 As String = ""
    Public strBillingAddress2 As String = ""
    Public strBillingCity As String = ""
    Public strBillingPostCode As String = ""
    Public strBillingCountry As String = ""
    Public strBillingState As String = ""
    Public strBillingPhone As String = ""
    Public bIsDeliverySame As String = ""
    Public strDeliveryFirstnames As String = ""
    Public strDeliverySurname As String = ""
    Public strDeliveryAddress1 As String = ""
    Public strDeliveryAddress2 As String = ""
    Public strDeliveryCity As String = ""
    Public strDeliveryPostCode As String = ""
    Public strDeliveryCountry As String = ""
    Public strDeliveryState As String = ""
    Public strDeliveryPhone As String = ""
    Public strPageState As String = ""
    Public strPageError As String = ""
    Public strStatus As String = ""
    Public strACSURL As String = ""
    Public strPAReq As String = ""
    Public strMD As String = ""
    Public strVendorTxCode As String = ""
    Public objConn As MySqlConnection = oMySQLConnection()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim strThisEntry As String = ""
        Dim intQuantity As Integer
        Dim intProductID As Integer
        Dim intBasketItems As Integer
        Dim oSQLCommand As MySqlCommand
        Dim oSQLReader As MySqlDataReader
        Dim decTotal As Decimal
        Dim strBasket As String
        Dim strSQL As String
        Dim decPrice As String
        Dim strPost As String
        Dim strResponse As String
        Dim strGiftAid As String
        Dim strStatusDetail As String
        Dim strVPSTxId As String
        Dim strSecurityKey As String
        Dim strNextURL As String

        strCart = Session("strCart")
        If String.IsNullOrEmpty(strCart) Then
            Server.Transfer("buildOrder.aspx")
            Response.End()
        End If

        '** Check we have a billing address in the session.  If not, go back to the customerDetails page to get one **
        If String.IsNullOrEmpty(Session("strBillingAddress1")) Then
            Server.Transfer("customerDetails.aspx")
            Response.End()
        End If

        '** Extract customer details from the session **
        strCustomerEMail = Session("strCustomerEMail")
        strBillingFirstnames = Session("strBillingFirstnames")
        strBillingSurname = Session("strBillingSurname")
        strBillingAddress1 = Session("strBillingAddress1")
        strBillingAddress2 = Session("strBillingAddress2")
        strBillingCity = Session("strBillingCity")
        strBillingPostCode = Session("strBillingPostCode")
        strBillingCountry = Session("strBillingCountry")
        strBillingState = Session("strBillingState")
        strBillingPhone = Session("strBillingPhone")
        bIsDeliverySame = Session("bIsDeliverySame")
        strDeliveryFirstnames = Session("strDeliveryFirstnames")
        strDeliverySurname = Session("strDeliverySurname")
        strDeliveryAddress1 = Session("strDeliveryAddress1")
        strDeliveryAddress2 = Session("strDeliveryAddress2")
        strDeliveryCity = Session("strDeliveryCity")
        strDeliveryPostCode = Session("strDeliveryPostCode")
        strDeliveryCountry = Session("strDeliveryCountry")
        strDeliveryState = Session("strDeliveryState")
        strDeliveryPhone = Session("strDeliveryPhone")

        '** All required field are present, so first store the order in the database then format the POST to Server **
        '** First we need to generate a unique VendorTxCode for this transaction **
        '** We're using VendorName, time stamp and a random element.  You can use different mehtods if you wish **
        '** but the VendorTxCode MUST be unique for each transaction you send to Server **
        Randomize()
        strVendorTxCode = cleanInput(strVendorName & "-" & Right(DatePart("yyyy", Now()), 2) & _
        Right("00" & DatePart("m", Now()), 2) & Right("00" & DatePart("d", Now()), 2) & _
        Right("00" & DatePart("h", Now()), 2) & Right("00" & DatePart("n", Now()), 2) & _
        Right("00" & DatePart("s", Now()), 2) & "-" & CStr(Math.Round(Rnd() * 100000)), "VendorTxCode")

        '** Calculate the transaction total based on basket contents.  For security **
        '** we recalculate it here rather than relying on totals stored in the session or hidden fields **
        '** We'll also create the basket contents to pass to Server. See the Server Protocol for **
        '** the full valid basket format.  The code below converts from our "x of y" style into **
        '** the system basket format (using a 20% VAT calculation for the tax columns) **

        strThisEntry = strCart
        While strThisEntry.Length > 0
            '** Extract the Quantity and Product from the list of "x of y," entries in the cart **
            intQuantity = cleanInput(Left(strThisEntry, 1), "Number")
            intProductID = cleanInput(Mid(strThisEntry, 6, InStr(strThisEntry, ",") - 6), "Number")

            '** Add another item to our Server basket **
            intBasketItems = intBasketItems + 1
            Dim sqlSQL As String = "SELECT * FROM tblProducts where ProductID=" & intProductID & ";"

            oSQLCommand = New MySqlCommand(sqlSQL, objConn)
            oSQLReader = oSQLCommand.ExecuteReader()
            oSQLReader.Read()
            '** Look up the products in the database **
            If oSQLReader.HasRows Then
                decTotal = decTotal + (CInt(intQuantity) * CSng(oSQLReader("Price")))
                strBasket = strBasket & ":" & oSQLReader("Description") & ":" & intQuantity
                strBasket = strBasket & ":" & FormatNumber(CSng(oSQLReader("Price")) / 1.2, 2, -1, 0, 0) '** Price ex-Vat **
                strBasket = strBasket & ":" & FormatNumber(CSng(oSQLReader("Price")) * 1 / 6, 2, -1, 0, 0) '** VAT component **
                strBasket = strBasket & ":" & FormatNumber(CSng(oSQLReader("Price")), 2, -1, 0, 0) '** Item price **
                strBasket = strBasket & ":" & FormatNumber(CSng(oSQLReader("Price")) * intQuantity, 2, -1, 0, 0) '** Line total **			
            End If

            oSQLReader.Close()
            oSQLReader = Nothing
            '** Move to the next cart entry, if there is one **
            If InStr(strThisEntry, ",") = 0 Then
                strThisEntry = ""
            Else
                strThisEntry = Mid(strThisEntry, InStr(strThisEntry, ",") + 1)
            End If
        End While

        '** We've been right through the cart, so delivery to the total and the basket **
        decTotal = decTotal + CSng(1.5)
        strBasket = intBasketItems + 1 & strBasket & ":Delivery:1:1.50:---:1.50:1.50"

        '** Now create the Server POST **
        '** Now to build the Server POST.  For more details see the Server Protocol 2.23 **
        '** NB: Fields potentially containing non ASCII characters are URLEncoded when included in the POST **
        strPost = "VPSProtocol=" & strProtocol
        strPost = strPost & "&TxType=" & strTransactionType '** PAYMENT by default.  You can change this in the includes file **
        strPost = strPost & "&Vendor=" & strVendorName
        strPost = strPost & "&VendorTxCode=" & strVendorTxCode '** As generated above **

        '** Optional: If you are a Sage Pay Partner and wish to flag the transactions with your unique partner id, it should be passed here
        If Len(strPartnerID) > 0 Then
            strPost = strPost & "&ReferrerID=" & URLEncode(strPartnerID)  ' ** You can change this in the includes file **
        End If

        strPost = strPost & "&Amount=" & FormatNumber(decTotal, 2, -1, 0, 0) '** Formatted to 2 decimal places with leading digit but no commas or currency symbols **
        strPost = strPost & "&Currency=" & strCurrency

        '** Up to 100 chars of free format description **
        strPost = strPost & "&Description=" & URLEncode("The best DVDs from " & strVendorName)

        '** The Notification URL is the page to which Server calls back when a transaction completes **
        '** You can change this for each transaction, perhaps passing a session ID or state flag if you wish **
        strPost = strPost & "&NotificationURL=" & strYourSiteFQDN & strVirtualDir & "/notificationPage.aspx"

        '** Billing Details **
        strPost = strPost & "&BillingSurname=" & URLEncode(strBillingSurname)
        strPost = strPost & "&BillingFirstnames=" & URLEncode(strBillingFirstnames)
        strPost = strPost & "&BillingAddress1=" & URLEncode(strBillingAddress1)
        If Len(strBillingAddress2) > 0 Then strPost = strPost & "&BillingAddress2=" & URLEncode(strBillingAddress2)
        strPost = strPost & "&BillingCity=" & URLEncode(strBillingCity)
        strPost = strPost & "&BillingPostCode=" & URLEncode(strBillingPostCode)
        strPost = strPost & "&BillingCountry=" & URLEncode(strBillingCountry)
        If Len(strBillingState) > 0 Then strPost = strPost & "&BillingState=" & URLEncode(strBillingState)
        If Len(strBillingPhone) > 0 Then strPost = strPost & "&BillingPhone=" & URLEncode(strBillingPhone)

        '** Delivery Details **
        strPost = strPost & "&DeliverySurname=" & URLEncode(strDeliverySurname)
        strPost = strPost & "&DeliveryFirstnames=" & URLEncode(strDeliveryFirstnames)
        strPost = strPost & "&DeliveryAddress1=" & URLEncode(strDeliveryAddress1)
        If Len(strDeliveryAddress2) > 0 Then strPost = strPost & "&DeliveryAddress2=" & URLEncode(strDeliveryAddress2)
        strPost = strPost & "&DeliveryCity=" & URLEncode(strDeliveryCity)
        strPost = strPost & "&DeliveryPostCode=" & URLEncode(strDeliveryPostCode)
        strPost = strPost & "&DeliveryCountry=" & URLEncode(strDeliveryCountry)
        If Len(strDeliveryState) > 0 Then strPost = strPost & "&DeliveryState=" & URLEncode(strDeliveryState)
        If Len(strDeliveryPhone) > 0 Then strPost = strPost & "&DeliveryPhone=" & URLEncode(strDeliveryPhone)

        '** other optionals **
        strPost = strPost & "&CustomerEMail=" & URLEncode(strCustomerEMail)
        strPost = strPost & "&Basket=" & URLEncode(strBasket) '** As created above **

        '** For charities registered for Gift Aid, set to 1 to display the Gift Aid check box on the payment pages **
        strPost = strPost & "&AllowGiftAid=0"

        '** Allow fine control over AVS/CV2 checks and rules by changing this value. 0 is Default **
        '** It can be changed dynamically, per transaction, if you wish.  See the Server Protocol document **
        If strTransactionType <> "AUTHENTICATE" Then strPost = strPost & "&ApplyAVSCV2=0"

        '** Allow fine control over 3D-Secure checks and rules by changing this value. 0 is Default **
        '** It can be changed dynamically, per transaction, if you wish.  See the Server Protocol document **
        strPost = strPost & "&Apply3DSecure=0"

        '** Optional setting for Profile can be used to set a simpler payment page. See protocol guide for more info. **
        strPost = strPost & "&Profile=NORMAL" 'NORMAL is default setting. Can also be set to LOW for the simpler payment page version.

        '** The full transaction registration POST has now been built **
        Dim objUTFEncode As New UTF8Encoding
        Dim arrRequest As Byte()
        Dim objStreamReq As Stream
        Dim objStreamRes As StreamReader
        Dim objHttpRequest As HttpWebRequest
        Dim objHttpResponse As HttpWebResponse
        Dim objUri As New Uri(SystemURL(strConnectTo, "purchase"))

        objHttpRequest = HttpWebRequest.Create(objUri)
        objHttpRequest.KeepAlive = False
        objHttpRequest.Method = "POST"

        objHttpRequest.ContentType = "application/x-www-form-urlencoded"
        arrRequest = objUTFEncode.GetBytes(strPost)
        objHttpRequest.ContentLength = arrRequest.Length
        objStreamReq = objHttpRequest.GetRequestStream()
        objStreamReq.Write(arrRequest, 0, arrRequest.Length)
        objStreamReq.Close()

        'Get response
        objHttpResponse = objHttpRequest.GetResponse()
        objStreamRes = New StreamReader(objHttpResponse.GetResponseStream(), Encoding.ASCII)

        strResponse = objStreamRes.ReadToEnd()
        objStreamRes.Close()

        If Err.Number <> 0 Then
            '** An non zero Err.number indicates an error of some kind **
            '** Check for the most common error... unable to reach the purchase URL **  
            If Err.Number = -2147012889 Then
                strPageError = "Your server was unable to register this transaction with Sage Pay." & _
                "  Check that you do not have a firewall restricting the POST and " & _
                "that your server can correctly resolve the address " & SystemURL(strConnectTo, "puchase")
            Else
                strPageError = "An Error has occurred whilst trying to register this transaction.<BR>" & _
                "The Error Number is: " & Err.Number & "<BR>" & _
                "The Description given is: " & Err.Description
            End If
            pnlError.Visible = True
            pnlResult.Visible = True
        Else
            '** No transport level errors, so the message got the Sage Pay **
            '** Analyse the response from Server to check that everything is okay **
            '** Registration results come back in the Status and StatusDetail fields **
            strStatus = findField("Status", strResponse)
            strStatusDetail = findField("StatusDetail", strResponse)

            '** Caters for both OK and OK REPEATED if the same transaction is registered twice **
            If Left(strStatus, 2) = "OK" Then
                '** An OK status mean that the transaction has been successfully registered **
                '** Your code needs to extract the VPSTxId (Sage Pay's unique reference for this transaction) **
                '** and the SecurityKey (used to validate the call back from Sage Pay later) and the NextURL **
                '** (the URL to which the customer's browser must be redirected to enable them to pay) **
                strVPSTxId = findField("VPSTxId", strResponse)
                strSecurityKey = findField("SecurityKey", strResponse)
                strNextURL = findField("NextURL", strResponse)

                '** Now store the VPSTxId, SecurityKey, VendorTxCode, order total and order details in **
                '** your database for use both at Notification stage, and your own order fulfilment **
                '** These kits come with a table called tblOrders in which this data is stored **
                '** accompanied by the tblOrderProducts table to hold the basket contents for each order **
                strSQL = "INSERT INTO tblOrders(VendorTxCode, TxType, Amount, Currency, " & _
                    "BillingFirstnames, BillingSurname, BillingAddress1, BillingAddress2, BillingCity, BillingPostCode, BillingCountry, BillingState, BillingPhone, " & _
                    "DeliveryFirstnames,DeliverySurname,DeliveryAddress1,DeliveryAddress2,DeliveryCity,DeliveryPostCode,DeliveryCountry,DeliveryState,DeliveryPhone, " & _
                    "CustomerEMail, VPSTxId, SecurityKey) VALUES ("

                strSQL = strSQL & "'" & SQLSafe(strVendorTxCode) & "',"        '** Add the VendorTxCode generated above **
                strSQL = strSQL & "'" & SQLSafe(strTransactionType) & "',"     '** Add the TxType from the includes file **
                strSQL = strSQL & "'" & FormatNumber(decTotal, 2, -1, 0, 0) & "'," '** Add the formatted total amount **
                strSQL = strSQL & "'" & SQLSafe(strCurrency) & "',"            '** Add the Currency **

                '** Add the Billing details **
                strSQL = strSQL & "'" & SQLSafe(strBillingFirstnames) & "',"
                strSQL = strSQL & "'" & SQLSafe(strBillingSurname) & "',"
                strSQL = strSQL & "'" & SQLSafe(strBillingAddress1) & "',"

                If Len(strBillingAddress2) > 0 Then
                    strSQL = strSQL & "'" & SQLSafe(strBillingAddress2) & "',"
                Else
                    strSQL = strSQL & "null,"
                End If

                strSQL = strSQL & "'" & SQLSafe(strBillingCity) & "',"
                strSQL = strSQL & "'" & SQLSafe(strBillingPostCode) & "',"
                strSQL = strSQL & "'" & SQLSafe(strBillingCountry) & "',"

                If Len(strBillingState) > 0 Then
                    strSQL = strSQL & "'" & SQLSafe(strBillingState) & "',"
                Else
                    strSQL = strSQL & "null,"
                End If

                If Len(strBillingPhone) > 0 Then
                    strSQL = strSQL & "'" & SQLSafe(strBillingPhone) & "',"
                Else
                    strSQL = strSQL & "null,"
                End If

                '** Add the Delivery details **
                strSQL = strSQL & "'" & SQLSafe(strDeliveryFirstnames) & "',"
                strSQL = strSQL & "'" & SQLSafe(strDeliverySurname) & "',"
                strSQL = strSQL & "'" & SQLSafe(strDeliveryAddress1) & "',"

                If Len(strDeliveryAddress2) > 0 Then
                    strSQL = strSQL & "'" & SQLSafe(strDeliveryAddress2) & "',"
                Else
                    strSQL = strSQL & "null,"
                End If

                strSQL = strSQL & "'" & SQLSafe(strDeliveryCity) & "',"
                strSQL = strSQL & "'" & SQLSafe(strDeliveryPostCode) & "',"
                strSQL = strSQL & "'" & SQLSafe(strDeliveryCountry) & "',"

                If Len(strDeliveryState) > 0 Then
                    strSQL = strSQL & "'" & SQLSafe(strDeliveryState) & "',"
                Else
                    strSQL = strSQL & "null,"
                End If

                If Len(strDeliveryPhone) > 0 Then
                    strSQL = strSQL & "'" & SQLSafe(strDeliveryPhone) & "',"
                Else
                    strSQL = strSQL & "null,"
                End If

                '** Customer email **
                If Len(strCustomerEMail) > 0 Then
                    strSQL = strSQL & "'" & SQLSafe(strCustomerEMail) & "',"
                Else
                    strSQL = strSQL & "null,"
                End If

                '** Now save the fields returned from the System and extracted above **
                strSQL = strSQL & "'" & SQLSafe(strVPSTxId) & "'," '** Save the System's unique transaction reference **
                strSQL = strSQL & "'" & SQLSafe(strSecurityKey) & "')" '** Save the MD5 Hashing security key, used in notification **

                '** Execute the SQL command to insert this data to the tblOrders table **
                objConn = oMySQLConnection()
                oSQLCommand = New MySqlCommand(strSQL, objConn)
                oSQLCommand.ExecuteReader()
                oSQLCommand = Nothing
                DestroyConnection(objConn)

                '** Now add the basket contents to the tblOrderProducts table, one line at a time **
                strThisEntry = strCart
                While strThisEntry.Length > 0
                    '** Extract the Quantity and Product from the list of "x of y," entries in the cart **
                    intQuantity = cleanInput(Left(strThisEntry, 1), "Number")
                    intProductID = cleanInput(Mid(strThisEntry, 6, InStr(strThisEntry, ",") - 6), "Number")

                    '** Look up the current price of the items in the database **
                    objConn = oMySQLConnection()
                    strSQL = "SELECT Price FROM tblProducts where ProductID=" & intProductID & ";"
                    oSQLReader = New MySqlCommand(strSQL, objConn).ExecuteReader
                    oSQLReader.Read()
                    If oSQLReader.HasRows Then
                        decPrice = CSng(oSQLReader("Price"))
                    End If
                    oSQLReader.Close()
                    oSQLReader = Nothing
                    DestroyConnection(objConn)

                    '** Save the basket contents with price included so we know the price at the time of order **
                    '** so that subsequent price changes will not affect the price paid for items in this order **
                    strSQL = "INSERT INTO tblOrderProducts(VendorTxCode,ProductID,Price,Quantity) VALUES("
                    strSQL = strSQL & "'" & SQLSafe(strVendorTxCode) & "',"
                    strSQL = strSQL & intProductID & ","
                    strSQL = strSQL & FormatNumber(decPrice, 2, -1, 0, 0) & ","
                    strSQL = strSQL & intQuantity & ")"

                    objConn = oMySQLConnection()
                    oSQLCommand = New MySqlCommand(strSQL, objConn)
                    oSQLCommand.ExecuteReader()
                    oSQLCommand = Nothing
                    DestroyConnection(objConn)

                    '** Move to the next cart entry, if there is one **
                    If InStr(strThisEntry, ",") = 0 Then
                        strThisEntry = ""
                    Else
                        strThisEntry = Mid(strThisEntry, InStr(strThisEntry, ",") + 1)
                    End If
                End While

                '** Finally, if we're not in Simulator Mode, redirect the page to the NextURL **
                '** In Simulator mode, we allow this page to display and ask for Proceed to be clicked **
                If strConnectTo <> "SIMULATOR" Then
                    Response.Clear()
                    Response.Redirect(strNextURL)
                    Response.End()
                Else
                    pnlNoError.Visible = True
                    pnlResult.Visible = True
                    pnlTxResponse.Visible = True
                    Proceed.Visible = True
                End If

            ElseIf strStatus = "MALFORMED" Then
                '** A MALFORMED status occurs when the POST sent above is not correctly formatted **
                '** or is missing compulsory fields.  You will normally only see these during **
                '** development and early testing **
                strPageError = "Sage Pay returned an MALFORMED status. " & _
                "The POST was Malformed because """ & findField("StatusDetail", strResponse) & """"
                pnlError.Visible = True
                pnlResult.Visible = True

            ElseIf strStatus = "INVALID" Then
                '** An INVALID status occurs when the structure of the POST was correct, but **
                '** one of the fields contains incorrect or invalid data.  These may happen when live **
                '** but you should modify your code to format all data correctly before sending **
                '** the POST to Server **
                strPageError = "Sage Pay returned an INVALID status. " & _
                "The data sent was Invalid because """ & findField("StatusDetail", strResponse) & """"
                pnlError.Visible = True
                pnlResult.Visible = True
            Else
                '** The only remaining status is ERROR **
                '** This occurs extremely rarely when there is a system level error at Sage Pay **
                '** If you receive this status the payment systems may be unavailable **<br>
                '** You could redirect your customer to a page offering alternative methods of payment here **
                strPageError = "Sage Pay returned an ERROR status. " & _
                "The description of the error was """ & findField("StatusDetail", strResponse) & """"
                pnlError.Visible = True
                pnlResult.Visible = True
            End If

        End If

        Dim ShpCartTable As DataTable
        Dim ShpCartRow As DataRow
        strThisEntry = strCart
        ShpCartTable = New System.Data.DataTable("ShpCart")

        ShpCartTable.Columns.Add("Productid", GetType(Integer))
        ShpCartTable.Columns.Add("description", GetType(String))
        ShpCartTable.Columns.Add("quantity", GetType(Integer))

        While strThisEntry.Length > 0
            '** Extract the quantity and Product from the list of "x of y," entries in the cart **
            intQuantity = cleanInput(Left(strThisEntry, 1), "Number")
            intProductID = cleanInput(Mid(strThisEntry, 6, InStr(strThisEntry, ",") - 6), "Number")

            strSQL = "select productid, description from tblproducts where ProductId=" & intProductID & ";"

            objConn = oMySQLConnection()
            oSQLCommand = New MySqlCommand(strSQL, objConn)
            oSQLReader = oSQLCommand.ExecuteReader()
            oSQLReader.Read()
            ShpCartRow = ShpCartTable.NewRow()

            ShpCartRow("Productid") = oSQLReader("productid")
            ShpCartRow("description") = oSQLReader("description")
            ShpCartRow("quantity") = intQuantity

            ShpCartTable.Rows.Add(ShpCartRow)

            oSQLReader.Close()
            oSQLReader = Nothing
            oSQLCommand = Nothing
            DestroyConnection(objConn)
            '** Move to the next cart entry, if there is one **
            If InStr(strThisEntry, ",") = 0 Then
                strThisEntry = ""
            Else
                strThisEntry = Mid(strThisEntry, InStr(strThisEntry, ",") + 1)
            End If

        End While
        dataRepBasket.DataSource = ShpCartTable
        dataRepBasket.DataBind()

        lblVendorTxCode.Text = strVendorTxCode
        lblVPSTxId.Text = strVPSTxId
        lblSecurityKey.Text = strSecurityKey
        lblTotal.Text = FormatNumber(decTotal, 2, TriState.True)
        NextURL.Value = strNextURL
        lblStatus.Text = strStatus
        lblPost.Text = strPost
        lblReply.Text = strResponse
        lblError.Text = strPageError

        On Error GoTo 0
    End Sub
    Protected Sub proceed_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles Proceed.Click, Proceed.Click
        Response.Clear()
        Response.Redirect(cleanInput(Request.Form("NextURL"), "Text"))
        Response.End()
    End Sub
    Protected Sub back_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles Back.Click, Back.Click
        Server.Transfer("orderConfirmation.aspx")
        Response.End()
    End Sub
    'to return the appropriate css class depending on the status returned
    Function getStyle() As String
        If strStatus = "OK" Or strStatus = "AUTHENTICATED" Or strStatus = "REGISTERED" Then
            getStyle = "infoheader"
        Else
            getStyle = "errorheader"
        End If
        Return getStyle
    End Function

End Class
