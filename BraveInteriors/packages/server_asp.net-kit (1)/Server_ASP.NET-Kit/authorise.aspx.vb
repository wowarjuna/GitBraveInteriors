Imports includes
Imports MySql.Data.MySqlClient
Imports System.IO
Imports System.Net

Partial Class authorise
    Inherits System.Web.UI.Page
    '**************************************************************************************************
    ' Server ASP Authorise Transaction Page
    '**************************************************************************************************
    ' Description
    ' ===========
    '
    ' This page perform builds a Authorise POST based on the details you enter here.  The contents of the
    ' POST and the response are then displayed on screen and the database updated accordingly
    '**************************************************************************************************
    Public strVPSTxId As String = ""
    Public strSecurityKey As String = ""
    Public strTxAuthNo As String = ""
    Public decAmount As Decimal
    Public strVendorTxCode As String = ""
    Public strStatus As String = ""
    Public objConn As MySqlConnection = oMySQLConnection()
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim strResult As String = ""
        Dim strPost As String = ""
        Dim strSQL As String = ""
        Dim decAuthoriseAmount As Decimal
        Dim strAuthoriseVendorTxCode As String
        Dim strAuthoriseDescription As String
        Dim oSQLReader As MySqlDataReader
        '** Check we have a vendortxcode in the session.  If not, or of the user clicked back then return to the orderAdmin screen. **
        strVendorTxCode = Session("strVendorTxCode")
        If (String.IsNullOrEmpty(strVendorTxCode) Or (Request.Form("back.x") > 0)) Then
            Response.Clear()
            Response.Redirect("orderAdmin.aspx")
            Response.End()
        End If

        If strStatus.Length > 0 Then
            pnlStatus.Visible = True
            pnlStatus2.Visible = True
            pnlResponse.Visible = True
        Else
            pnlPreAuthorise.Visible = True
            pnlAuthoriseDetails.Visible = True
        End If

        strSQL = "SELECT * FROM tblOrders where VendorTxCode='" & SQLSafe(strVendorTxCode) & "'"

        Dim oSQLCommand = New MySqlCommand(strSQL, objConn)
        oSQLReader = oSQLCommand.ExecuteReader()
        oSQLReader.Read()

        If oSQLReader.HasRows Then
            strVPSTxId = oSQLReader("VPSTxId")
            strSecurityKey = oSQLReader("SecurityKey")
            decAmount = CSng(oSQLReader("Amount"))
        Else
            strStatus = "ERROR"
            strResult = "ERROR : Cannot retrieve the original transaction data from the database."
        End If

        oSQLCommand = Nothing
        oSQLReader.Close()
        oSQLReader = Nothing

        '** Since no buttons have been clicked, generate a random VendorTxCode for this Repeat, based on on the original VendorTxCode **
        '** You can edit this in the boxes provided, but you probably wouldn't in a production environment **
        Randomize()
        strAuthoriseVendorTxCode = Left("AUTH-" & CStr(Decimal.Round(Rnd() * 100000)) & "-" & strVendorTxCode, 40)
        strAuthoriseDescription = "AUTHORISE against " & strVendorTxCode

        '** We can work out the default amount to Repeat as the original transaction value - the total valuue of all Repeats against it **
        '** If this is Zero, then we can Repeat no more.  Show an error message in these circumstance **
        decAuthoriseAmount = decAmount
        strSQL = "SELECT Amount FROM tblOrders where RelatedVendorTxCode='" & strVendorTxCode & "' and TxType='AUTHORISE'"
        oSQLCommand = New MySqlCommand(strSQL, objConn)
        oSQLReader = oSQLCommand.ExecuteReader()
        oSQLReader.Read()

        If oSQLReader.HasRows Then
            decAuthoriseAmount = decAuthoriseAmount - CSng(oSQLReader("Amount"))
        End If
        oSQLReader.Close()
        oSQLReader = Nothing
        oSQLCommand = Nothing

        If decAuthoriseAmount <= 0.0 Then
            strStatus = "ERROR"
            strResult = "You cannot Authorise this transaction.  You've already fully Authoriseed it."
        End If

        lblvendorTxCode.Text = strVendorTxCode
        lblVPSTxId.Text = strVPSTxId
        lblSecurityKey.Text = strSecurityKey
        AuthoriseVendorTxCode.Text = strAuthoriseVendorTxCode
        AuthoriseDescription.Text = strAuthoriseDescription
        AuthoriseAmount.Text = decAuthoriseAmount
        lblResults.Text = strResult

    End Sub
    Protected Sub proceed_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles proceed.Click
        Dim strAuthoriseAmount As String
        Dim strAuthoriseDescription As String
        Dim strAuthoriseVendorTxCode As String
        Dim strResult As String
        Dim decAuthoriseAmount As Decimal
        Dim strPost As String
        Dim strSQL As String
        Dim strResponse As String
        Dim strPageError As String
        Dim strAuthoriseVPSTxId As String
        Dim strAuthoriseTxAuthNo As String
        Dim strAuthoriseSecurityKey As String
        Dim oSQLReader As MySqlDataReader
        Dim strAuthoriseAVSCV2 As String
        Dim strAuthoriseAddressResult As String
        Dim strAuthorisePostCodeResult As String
        Dim strAuthoriseCV2Result As String

        strAuthoriseAmount = cleanInput(Request.Form("AuthoriseAmount"), "Number")
        strAuthoriseDescription = cleanInput(Request.Form("AuthoriseDescription"), "Text")
        strAuthoriseVendorTxCode = cleanInput(Request.Form("AuthoriseVendorTxCode"), "VendorTxCode")

        '** Validate the Authorise amount.  It must be a number, greater than 0  **
        If Len(strAuthoriseAmount) = 0 Or Not (IsNumeric(strAuthoriseAmount)) Then
            strstatus = "ERROR"
            strResult = "ERROR : You need to specify an amount to Authorise. This can be any amount greater than zero."
        ElseIf Len(strAuthoriseVendorTxCode) = 0 Then
            strstatus = "ERROR"
            strResult = "ERROR : You need to specify a new VendorTxCode for this Authorise transaction."
        ElseIf Len(strAuthoriseDescription) = 0 Then
            strstatus = "ERROR"
            strResult = "ERROR : You need to enter a Description of this Authorise transaction."
        Else
            decAuthoriseAmount = CSng(strAuthoriseAmount)
            decAmount = (decAmount * 1.15)

            decAmount = decAmount - decAuthoriseAmount
            strSQL = "SELECT Amount FROM tblOrders where RelatedVendorTxCode='" & strVendorTxCode & "' and TxType='AUTHORISE'"
            Dim oSQLCommand = New MySqlCommand(strSQL, objConn)
            oSQLReader = oSQLCommand.ExecuteReader()

            While oSQLReader.Read()
                decAmount = decAmount - CSng(oSQLReader("Amount"))
            End While

            oSQLReader.Close()
            oSQLReader = Nothing
            oSQLCommand = Nothing

            If (decAuthoriseAmount <= 0.0) Then
                strstatus = "ERROR"
                strResult = "ERROR : You need to specify an amount to Authorise.  This can be any amount greater than zero."
            ElseIf (decAmount < 0.0) Then
                strstatus = "ERROR"
                strResult = "ERROR : The Authorise would exceed 115% of the Amount of the AUTHENTICATE.  Cannot perform the Authorise."
            Else
                '** Build the Authorise message **
                strPost = "VPSProtocol=" & strProtocol
                strPost = strPost & "&TxType=Authorise"
                strPost = strPost & "&Vendor=" & strVendorName
                strPost = strPost & "&Amount=" & FormatNumber(decAuthoriseAmount, 2, -1, 0, 0)
                strPost = strPost & "&VendorTxCode=" & strAuthoriseVendorTxCode
                strPost = strPost & "&Description=" & strAuthoriseDescription
                strPost = strPost & "&RelatedVPSTxId=" & strVPSTxId
                strPost = strPost & "&RelatedVendorTxCode=" & strVendorTxCode
                strPost = strPost & "&RelatedSecurityKey=" & strSecurityKey
                strPost = strPost & "&ApplyAVSCV2=0"

                '** The full transaction registration POST has now been built **
                Dim objUTFEncode As New UTF8Encoding
                Dim arrRequest As Byte()
                Dim objStreamReq As Stream
                Dim objStreamRes As StreamReader
                Dim objHttpRequest As HttpWebRequest
                Dim objHttpResponse As HttpWebResponse
                Dim objUri As New Uri(SystemURL(strConnectTo, "authorise"))

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
                Else
                    '** No transport level errors, so the message got the Sage Pay **
                    '** Analyse the response from Server to check that everything is okay **
                    strstatus = findField("Status", strResponse)

                    If Left(strstatus, 2) = "OK" Then
                        '** An OK status mean that the transaction has been successfully Authoriseed **
                        strResult = "SUCCESS : The transaction was AuthoriseED successfully and a new Authorise transaction was created."

                        '** Get the other values from the POST for storage in the database **
                        strAuthoriseVPSTxId = findField("VPSTxId", strResponse)
                        strAuthoriseTxAuthNo = findField("TxAuthNo", strResponse)
                        strAuthoriseSecurityKey = findField("SecurityKey", strResponse)
                        strAuthoriseAVSCV2 = findField("AVSCV2", strResponse)
                        strAuthoriseAddressResult = findField("AddressResult", strResponse)
                        strAuthorisePostCodeResult = findField("PostCodeResult", strResponse)
                        strAuthoriseCV2Result = findField("CV2Result", strResponse)

                        '** Create the new Authorise transaction in the database, linked through to the original transaction **
                        strSQL = "INSERT INTO tblOrders(VendorTxCode,TxType,Amount,Currency,VPSTxId,SecurityKey," & _
                        "TxAuthNo,AVSCV2,AddressResult,PostCodeResult,CV2Result,RelatedVendorTxCode,Status) VALUES("

                        strSQL = strSQL & "'" & strAuthoriseVendorTxCode & "',"
                        strSQL = strSQL & "'Authorise',"
                        strSQL = strSQL & FormatNumber(strAuthoriseAmount, 2, -1, 0, 0) & ","
                        strSQL = strSQL & "'" & strCurrency & "',"
                        strSQL = strSQL & "'" & strAuthoriseVPSTxId & "',"
                        strSQL = strSQL & "'" & strAuthoriseSecurityKey & "',"
                        strSQL = strSQL & strAuthoriseTxAuthNo & ","
                        strSQL = strSQL & "'" & strAuthoriseAVSCV2 & "',"
                        strSQL = strSQL & "'" & strAuthoriseAddressResult & "',"
                        strSQL = strSQL & "'" & strAuthorisePostCodeResult & "',"
                        strSQL = strSQL & "'" & strAuthoriseCV2Result & "',"
                        strSQL = strSQL & "'" & strVendorTxCode & "',"
                        strSQL = strSQL & "'AUTHORISED - Authorise transaction taken through Order Admin area.'"

                        strSQL = strSQL & ")"

                        oSQLCommand = New MySqlCommand(strSQL, objConn)
                        oSQLCommand.ExecuteReader()
                        oSQLCommand = Nothing
                    Else
                        '** All other Statuses are errors of one form or another.  Display them on the screen with no database updates **
                        strResult = strStatus & " : " & findField("StatusDetail", strResponse)

                    End If

                End If

                On Error GoTo 0

            End If
        End If

        lblResults.Text = strResult
        lblStatus.Text = strstatus
        lblPost.Text = strPost
        lblResponse.Text = strResponse
        pnlAuthoriseDetails.Visible = False

        If strStatus.Length > 0 Then
            pnlStatus.Visible = True
            pnlStatus2.Visible = True
            pnlResponse.Visible = True
        End If

    End Sub

    Protected Sub back_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles back.Click
        Response.Clear()
        Server.Transfer("orderAdmin.aspx")
        Response.End()
    End Sub

    Function getStyle() As String
        If strStatus = "OK" Or strStatus = "Authenticated" Or strStatus = "Registered" Then
            getStyle = "infoheader"
        Else
            getStyle = "errorheader"
        End If
        Return getStyle
    End Function

    Protected Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Unload
        DestroyConnection(objConn)
    End Sub
End Class
