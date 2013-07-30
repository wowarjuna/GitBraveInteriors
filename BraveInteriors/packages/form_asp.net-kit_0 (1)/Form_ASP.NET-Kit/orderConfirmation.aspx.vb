Imports includes
Imports System.Data

Partial Class orderConfirmation
    Inherits System.Web.UI.Page
    '**************************************************************************************************
    ' Description
    ' ===========
    '
    ' Displays a summary of the order items and customer details and builds the Form Crypt field
    ' that will be sent along with the user to the Sage Pay payment pages.  In SIMULATOR and TEST mode
    ' the decoded version of this field will be displayed on screen for you to check.
    '**************************************************************************************************
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim strCart As String = ""

        '** Customer Details
        Dim strBillingFirstName, strDeliveryFirstName As String
        Dim strBillingSurname, strDeliverySurname As String
        Dim strBillingAddressLine1, strDeliveryAddressLine1 As String
        Dim strBillingAddressLine2, strDeliveryAddressLine2 As String
        Dim strBillingPostCode, strDeliveryPostCode As String
        Dim strBillingPhoneNumber, strDeliveryPhoneNumber As String
        Dim strBillingCity, strDeliveryCity As String
        Dim strBillingStateCode, strDeliveryStateCode As String
        Dim strBillingCountry, strDeliveryCountry As String

        Dim strCustomerEMail As String
        Dim bolDeliverySame As Boolean
        Dim strPost As String
        Dim strVendorTxCode As String
        Dim strCrypt As String

        '** Check we have a cart in the session.  If not, go back to the buildOrder page to get one **
        strCart = Session("strCart")
        If String.IsNullOrEmpty(strCart) Then
            Response.Clear()
            Server.Transfer("buildOrder.aspx")
            Response.End()
        End If

        If strConnectTo = "LIVE" Then
            pnlNotSimulator.Visible = True
            pnlSimulator.Visible = False
            pnlTest.Visible = False
        Else
            pnlSimulator.Visible = True
            pnlTest.Visible = True
        End If

        '** Check if Delivery Details match Billing
        bolDeliverySame = Session("bIsDeliverySame")

        '** Collect Billing Details
        strBillingFirstName = Session("strBillingFirstname")
        strBillingSurname = Session("strBillingSurname")
        strBillingAddressLine1 = Session("strBillingAddressLine1")
        strBillingAddressLine2 = Session("strBillingAddressLine2")
        strBillingCity = Session("strBillingCity")
        strBillingPostCode = Session("strBillingPostCode")
        strBillingCountry = Session("strBillingCountry")
        strBillingStateCode = Session("strBillingStateCode")
        strBillingPhoneNumber = Session("strBillingPhoneNumber")
        strCustomerEMail = Session("strCustomerEMail")

        '** Collect Delivery Details
        If bolDeliverySame = True Then
            strDeliveryFirstName = strBillingFirstName
            strDeliverySurname = strBillingSurname
            strDeliveryAddressLine1 = strBillingAddressLine1
            strDeliveryAddressLine2 = strBillingAddressLine2
            strDeliveryCity = strBillingCity
            strDeliveryPostCode = strBillingPostCode
            strDeliveryCountry = strBillingCountry
            strDeliveryStateCode = strBillingStateCode
            strDeliveryPhoneNumber = strBillingPhoneNumber
        Else
            strDeliveryFirstName = Session("strDeliveryFirstname")
            strDeliverySurname = Session("strDeliverySurname")
            strDeliveryAddressLine1 = Session("strDeliveryAddressLine1")
            strDeliveryAddressLine2 = Session("strDeliveryAddressLine2")
            strDeliveryCity = Session("strDeliveryCity")
            strDeliveryPostCode = Session("strDeliveryPostCode")
            strDeliveryCountry = Session("strDeliveryCountry")
            strDeliveryStateCode = Session("strDeliveryStateCode")
            strDeliveryPhoneNumber = Session("strDeliveryPhoneNumber")
        End If

        If String.IsNullOrEmpty(strBillingAddressLine1) Then
            Response.Clear()
            Server.Transfer("customerDetails.aspx")
            Response.End()
        End If

        '** Okay, build the crypt field for Form using the information in our session **
        '** First we need to generate a unique VendorTxCode for this transaction **
        '** We're using VendorName, time stamp and a random element.  You can use different mehtods if you wish **
        '** but the VendorTxCode MUST be unique for each transaction you send to Server **
        Randomize()
        strVendorTxCode = strVendorName & "-" & Right(DatePart("yyyy", Now()), 2) & _
                            Right("00" & DatePart("m", Now()), 2) & Right("00" & DatePart("d", Now()), 2) & _
                            Right("00" & DatePart("h", Now()), 2) & Right("00" & DatePart("n", Now()), 2) & _
                            Right("00" & DatePart("s", Now()), 2) & "-" & CStr(Math.Round(Rnd() * 100000))

        '** Now to calculate the transaction total based on basket contents.  For security **
        '** we recalculate it here rather than relying on totals stored in the session or hidden fields **
        '** We'll also create the basket contents to pass to Form. See the Form Protocol for **
        '** the full valid basket format.  The code below converts from our "x of y" style into **
        '** the system basket format (using a 17.5% VAT calculation for the tax columns) **

        lblBillingName.Text = Server.HtmlEncode(strBillingFirstName & " " & strBillingSurname)

        lblBillingAddressPC.Text = Server.HtmlEncode(strBillingAddressLine1) & "<br />" & _
                                   Server.HtmlEncode(strBillingAddressLine2) & "<br />" & _
                                   Server.HtmlEncode(strBillingCity & " " & strBillingStateCode) & "<br />" & _
                                   Server.HtmlEncode(strBillingPostCode) & "<br />" & _
                                    "<script type=""text/javascript"" language=""javascript"">" & _
                                    "document.write( getCountryName( """ & Server.HtmlEncode(strBillingCountry) & """ ));" & _
                                    "</script>"

        If bolDeliverySame = True Then
            lblDeliveryName.Text = Server.HtmlEncode(strBillingFirstName & " " & strBillingSurname)
            lblDeliveryAddressPC.Text = Server.HtmlEncode(strBillingAddressLine1) & "<br />" & _
                                        Server.HtmlEncode(strBillingAddressLine2) & "<br />" & _
                                        Server.HtmlEncode(strBillingCity & " " & strBillingStateCode) & "<br />" & _
                                        Server.HtmlEncode(strBillingPostCode) & "<br />" & _
                                        "<script type=""text/javascript"" language=""javascript"">" & _
                                        "document.write( getCountryName( """ & Server.HtmlEncode(strBillingCountry) & """ ));" & _
                                        "</script>"
        Else
            lblDeliveryName.Text = Server.HtmlEncode(strDeliveryFirstName & " " & strDeliverySurname)
            lblDeliveryAddressPC.Text = Server.HtmlEncode(strDeliveryAddressLine1) & "<br />" & _
                                        Server.HtmlEncode(strDeliveryAddressLine2) & "<br />" & _
                                        Server.HtmlEncode(strDeliveryCity & " " & strDeliveryStateCode) & "<br />" & _
                                        Server.HtmlEncode(strDeliveryPostCode) & "<br />" & _
                                        "<script type=""text/javascript"" language=""javascript"">" & _
                                        "document.write( getCountryName( """ & Server.HtmlEncode(strDeliveryCountry) & """ ));" & _
                                        "</script>"
        End If

        lblBillingPhoneNumber.Text = Server.HtmlEncode(strBillingPhoneNumber)
        lblDeliveryPhoneNumber.Text = Server.HtmlEncode(strDeliveryPhoneNumber)
        lblEmailAddress.Text = Server.HtmlEncode(strCustomerEMail)

        Dim decTotal As Decimal = 0.0
        Dim decDelivery As Decimal = 1.5
        Dim strThisEntry As String = strCart
        Dim strBasket As String
        Dim intQuantity As Integer
        Dim intProductID As Integer
        Dim intBasketItems As Integer = 0
        Dim ShpCartTable As DataTable
        Dim ShpCartRow As DataRow

        While Len(strThisEntry) > 0
            '** Extract the Quantity and Product from the list of "x of y," entries in the cart **
            intQuantity = cleanInput(Left(strThisEntry, 1), CleanInputFilterType.Numeric)
            intProductID = cleanInput(Mid(strThisEntry, 6, InStr(strThisEntry, ",") - 6), CleanInputFilterType.Numeric)

            '** Add another item to our Form basket **
            intBasketItems = intBasketItems + 1

            decTotal = decTotal + (CInt(intQuantity) * CSng(arrProducts(intProductID, 2)))
            strBasket = strBasket & ":" & arrProducts(intProductID, 1) & ":" & intQuantity
            strBasket = strBasket & ":" & FormatNumber(CSng(arrProducts(intProductID, 2)) / 1.2, 2, -1, 0, 0) '** Price ex-Vat **
            strBasket = strBasket & ":" & FormatNumber(CSng(arrProducts(intProductID, 2)) / 6, 2, -1, 0, 0) '** VAT component **
            strBasket = strBasket & ":" & FormatNumber(CSng(arrProducts(intProductID, 2)), 2, -1, 0, 0) '** Item price **
            strBasket = strBasket & ":" & FormatNumber(CSng(arrProducts(intProductID, 2)) * intQuantity, 2, -1, 0, 0) '** Line total **			

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

        '** Now to build the Form crypt field.  For more details see the  Form Protocol 2.22 **
        strPost = "VendorTxCode=" & strVendorTxCode '** As generated above **
        '** Add the ReferrerID to the Post
        strPost = strPost & "&ReferrerID=" & strPartnerID

        strPost = strPost & "&Amount=" & FormatNumber(decTotal, 2, -1, 0, 0) '** Formatted to 2 decimal places with leading digit **
        strPost = strPost & "&Currency=" & strCurrency
        '** Up to 100 chars of free format description **
        strPost = strPost & "&Description=The best DVDs from " & strVendorName

        '** The SuccessURL is the page to which Form returns the customer if the transaction is successful **
        '** You can change this for each transaction, perhaps passing a session ID or state flag if you wish **
        strPost = strPost & "&SuccessURL=" & strYourSiteFQDN & strVirtualDir & "/orderSuccessful.aspx"

        '** The FailureURL is the page to which Form returns the customer if the transaction is unsuccessful **
        '** You can change this for each transaction, perhaps passing a session ID or state flag if you wish **
        strPost = strPost & "&FailureURL=" & strYourSiteFQDN & strVirtualDir & "/orderFailed.aspx"

        '** Pass the Customer's name for use within confirmation emails and the Sage Pay Admin area.
        strPost = strPost & "&CustomerName=" & strBillingFirstName & " " & strBillingSurname
        strPost = strPost & "&CustomerEMail=" & strCustomerEMail
        If strVendorEMail <> "[your e-mail address]" Then
            strPost = strPost & "&VendorEMail=" & strVendorEMail
        End If

        strPost = strPost & "&SendEMail=" & iSendEMail
        '** You can specify any custom message to send to your customers in their confirmation e-mail here **
        '** The field can contain HTML if you wish, and be different for each order.  The field is optional **
        strPost = strPost & "&eMailMessage=Thank you so very much for your order."

        '** Populate Customer Details for crypt string
        '** Billing Details
        strPost = strPost & "&BillingSurname=" & strBillingSurname
        strPost = strPost & "&BillingFirstnames=" & strBillingFirstName
        strPost = strPost & "&BillingAddress1=" & strBillingAddressLine1
        If Not String.IsNullOrEmpty(strBillingAddressLine2) Then
            strPost = strPost & "&BillingAddress2=" & strBillingAddressLine2
        End If
        strPost = strPost & "&BillingCity=" & strBillingCity
        strPost = strPost & "&BillingPostCode=" & strBillingPostCode
        strPost = strPost & "&BillingCountry=" & strBillingCountry
        If Not String.IsNullOrEmpty(strBillingStateCode) Then
            strPost = strPost & "&BillingState=" & strBillingStateCode
        End If
        If Not String.IsNullOrEmpty(strBillingPhoneNumber) Then
            strPost = strPost & "&BillingPhone=" & strBillingPhoneNumber
        End If

        '** Delivery Details
        If bolDeliverySame Then
            strPost = strPost & "&DeliverySurname=" & strBillingSurname
            strPost = strPost & "&DeliveryFirstnames=" & strBillingFirstName
            strPost = strPost & "&DeliveryAddress1=" & strBillingAddressLine1
            If Not String.IsNullOrEmpty(strBillingAddressLine2) Then
                strPost = strPost & "&DeliveryAddress2=" & strBillingAddressLine2
            End If
            strPost = strPost & "&DeliveryCity=" & strBillingCity
            strPost = strPost & "&DeliveryPostCode=" & strBillingPostCode
            strPost = strPost & "&DeliveryCountry=" & strBillingCountry
            If Not String.IsNullOrEmpty(strBillingStateCode) Then
                strPost = strPost & "&DeliveryState=" & strBillingStateCode
            End If
            If Not String.IsNullOrEmpty(strBillingPhoneNumber) Then
                strPost = strPost & "&DeliveryPhone=" & strBillingPhoneNumber
            End If
        Else '** delivery details differ from billing details
            strPost = strPost & "&DeliverySurname=" & strDeliverySurname
            strPost = strPost & "&DeliveryFirstnames=" & strDeliveryFirstName
            strPost = strPost & "&DeliveryAddress1=" & strDeliveryAddressLine1
            If Not String.IsNullOrEmpty(strDeliveryAddressLine2) Then
                strPost = strPost & "&DeliveryAddress2=" & strDeliveryAddressLine2
            End If
            strPost = strPost & "&DeliveryCity=" & strDeliveryCity
            strPost = strPost & "&DeliveryPostCode=" & strDeliveryPostCode
            strPost = strPost & "&DeliveryCountry=" & strDeliveryCountry
            If Not String.IsNullOrEmpty(strBillingStateCode) Then
                strPost = strPost & "&DeliveryState=" & strDeliveryStateCode
            End If
            If Not String.IsNullOrEmpty(strBillingPhoneNumber) Then
                strPost = strPost & "&DeliveryPhone=" & strDeliveryPhoneNumber
            End If
        End If

        strPost = strPost & "&Basket=" & strBasket '** As created above **

        '** For charities registered for Gift Aid, set to 1 to display the Gift Aid check box on the payment pages **
        strPost = strPost & "&AllowGiftAid=0"

        '** Allow fine control over AVS/CV2 checks and rules by changing this value. 0 is Default **
        '** It can be changed dynamically, per transaction, if you wish.  See the Server Protocol document **
        If strTransactionType <> "AUTHENTICATE" Then strPost = strPost & "&ApplyAVSCV2=0"

        '** Allow fine control over 3D-Secure checks and rules by changing this value. 0 is Default **
        '** It can be changed dynamically, per transaction, if you wish.  See the Server Protocol document **
        strPost = strPost & "&Apply3DSecure=0"

        '*** setup post string for display
        lblPostString.Text = Server.HtmlEncode(strPost)
        '*** Determine if posting to Live connection string, if so hide the Post String panel
        If strConnectTo <> "LIVE" Then
            CryptContents.Visible = True
        Else
            CryptContents.Visible = False
        End If

        ' ** Encrypt the plaintext string for inclusion in the hidden field **
        strCrypt = EncryptAndEncode(strPost)

        'create a shopping basket table for displaying the information
        ShpCartTable = New System.Data.DataTable("ShpCart")

        ShpCartTable.Columns.Add("Productid", GetType(Integer))
        ShpCartTable.Columns.Add("description", GetType(String))
        ShpCartTable.Columns.Add("price", GetType(String))
        ShpCartTable.Columns.Add("quantity", GetType(Integer))
        ShpCartTable.Columns.Add("total", GetType(String))
        strThisEntry = strCart
        While Not String.IsNullOrEmpty(strThisEntry)
            '** Extract the quantity and Product from the list of "x of y," entries in the cart **
            intQuantity = cleanInput(Left(strThisEntry, 1), CleanInputFilterType.Numeric)
            intProductID = cleanInput(Mid(strThisEntry, 6, InStr(strThisEntry, ",") - 6), CleanInputFilterType.Numeric)
            ShpCartRow = ShpCartTable.NewRow
            ShpCartRow("Productid") = intProductID
            ShpCartRow("description") = Server.HtmlEncode(arrProducts(intProductID, 1))
            ShpCartRow("price") = FormatNumber(arrProducts(intProductID, 2), 2, TriState.True) & " " & strCurrency
            ShpCartRow("quantity") = intQuantity
            ShpCartRow("total") = FormatNumber(CInt(intQuantity) * CSng(arrProducts(intProductID, 2)), 2, TriState.True) & " " & strCurrency

            ShpCartTable.Rows.Add(ShpCartRow)

            '** Move to the next cart entry, if there is one **
            If InStr(strThisEntry, ",") = 0 Then
                strThisEntry = ""
            Else
                strThisEntry = Mid(strThisEntry, InStr(strThisEntry, ",") + 1)
            End If
        End While

        dataRepBasket.DataSource = ShpCartTable
        dataRepBasket.DataBind()

        'set delivery and price and total price
        lblDelivery.Text = FormatNumber(decDelivery, 2, TriState.True) & " " & includes.strCurrency
        lblTotal.Text = FormatNumber(decTotal, 2, TriState.True) & " " & includes.strCurrency

        Dim strForm As String = ""

        strForm = strForm & String.Format("<form name=""SagePayForm"" method=""POST"" action=""{0}"" >", SystemURL(strConnectTo)) & ControlChars.CrLf
        strForm = strForm & String.Format("<input type=""hidden"" name=""VPSProtocol"" value=""{0}"">", strProtocol) & ControlChars.CrLf
        strForm = strForm & String.Format("<input type=""hidden"" name=""TxType"" value=""{0}"">", strTransactionType) & ControlChars.CrLf
        strForm = strForm & String.Format("<input type=""hidden"" name=""Vendor"" value=""{0}"">", strVendorName) & ControlChars.CrLf
        strForm = strForm & String.Format("<input type=""hidden"" name=""Crypt"" value=""{0}"">", strCrypt) & ControlChars.CrLf
        strForm = strForm & "<img align=""left"" type=""image"" name=""back"" src=""images/back.gif"" alt="""" onclick=""Javascript:window.location='customerdetails.aspx'""/>"
        strForm = strForm & "<input align=""right"" type=""image"" name=""proceed"" src=""images/proceed.gif"" value=""Proceed to Form registration"">"
        strForm = strForm & "</form>"

        lblForm.Text = strForm

    End Sub

End Class
