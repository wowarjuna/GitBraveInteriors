Imports includes
Imports MySql.Data.MySqlClient
Imports System.IO
Imports System.Net
Partial Class refund
    Inherits System.Web.UI.Page
    '**************************************************************************************************
    ' Server ASP Refund Transaction Page
    '**************************************************************************************************
    ' Description
    ' ===========
    ' This page perform builds a Refund POST based on the details you enter here.  The contents of the
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
        Dim decRefundAmount As Decimal
        Dim strRefundVendorTxCode As String
        Dim strRefundDescription As String

        Dim oSQLReader As MySqlDataReader
        '** Check we have a vendortxcode in the session.  If not, or of the user clicked back then return to the orderAdmin screen. **
        strVendorTxCode = Session("strVendorTxCode")
        If (strVendorTxCode.Length = 0) Or (Request.Form("back.x") > 0) Then
            Response.Clear()
            Response.Redirect("orderAdmin.aspx")
            Response.End()
        End If

        If strStatus.Length > 0 Then
            pnlStatus.Visible = True
            pnlResponse.Visible = True
        Else
            pnlPreRefund.Visible = True
            pnlRefundDetails.Visible = True
        End If
        strSQL = "SELECT * FROM tblOrders where VendorTxCode='" & SQLSafe(strVendorTxCode) & "'"

        Dim oSQLCommand = New MySqlCommand(strSQL, objConn)
        oSQLReader = oSQLCommand.ExecuteReader()
        oSQLReader.Read()

        If oSQLReader.HasRows Then
            strVPSTxId = oSQLReader("VPSTxId")
            strSecurityKey = oSQLReader("SecurityKey")
            strTxAuthNo = oSQLReader("TxAuthNo")
            decAmount = CSng(oSQLReader("Amount"))
        Else
            strStatus = "ERROR"
            strResult = "ERROR : Cannot retrieve the original transaction data from the database."
        End If

        oSQLReader.Close()
        oSQLReader = Nothing
        oSQLCommand = Nothing

        '** Since no buttons have been clicked, generate a random VendorTxCode for this Refund, based on on the original VendorTxCode **
        '** You can edit this in the boxes provided, but you probably wouldn't in a production environment **
        Randomize()
        strRefundVendorTxCode = Left("REF-" & CStr(Decimal.Round(Rnd() * 100000)) & "-" & strVendorTxCode, 40)
        strRefundDescription = "Refund against " & strVendorTxCode

        '** We can work out the default amount to refund as the original transaction value - the total valuue of all refunds against it **
        '** If this is Zero, then we can refund no more.  Show an error message in these circumstance **
        decRefundAmount = decAmount
        strSQL = "SELECT Amount FROM tblOrders where RelatedVendorTxCode='" & strVendorTxCode & "' and TxType='REFUND'"
        oSQLCommand = New MySqlCommand(strSQL, objConn)
        oSQLReader = oSQLCommand.ExecuteReader()
        oSQLReader.Read()

        If oSQLReader.HasRows Then
            decRefundAmount = decRefundAmount - CSng(oSQLReader("Amount"))
        End If
        oSQLReader.Close()
        oSQLReader = Nothing
        oSQLCommand = Nothing

        If decRefundAmount <= 0.0 Then
            strStatus = "ERROR"
            strResult = "You cannot REFUND this transaction.  You've already fully refunded it."
            pnlStatus.Visible = True
        End If

        lblvendorTxCode.Text = strVendorTxCode
        lblVPSTxId.Text = strVPSTxId
        lblSecurityKey.Text = strSecurityKey
        lblTxAuthNo.Text = strTxAuthNo
        RefundVendorTxCode.Text = strRefundVendorTxCode
        RefundDescription.Text = "Refund against " & strVendorTxCode
        RefundAmount.Text = decRefundAmount
        lblResults.Text = strResult

    End Sub
    Protected Sub proceed_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles proceed.Click
        Dim strRefundAmount As String
        Dim strRefundDescription As String
        Dim strRefundVendorTxCode As String
        Dim strResult As String
        Dim decRefundAmount As Decimal
        Dim strPost As String
        Dim strSQL As String
        Dim strResponse As String
        Dim strPageError As String
        Dim strRefundVPSTxId As String
        Dim strRefundTxAuthNo As String
        Dim strRefundSecurityKey As String
        Dim oSQLReader As MySqlDataReader

        strRefundAmount = cleanInput(Request.Form("RefundAmount"), "Number")
        strRefundDescription = cleanInput(Request.Form("RefundDescription"), "Text")
        strRefundVendorTxCode = cleanInput(Request.Form("RefundVendorTxCode"), "VendorTxCode")

        '** Validate the Refund amount.  It must be a number, greater than 0  **
        If Len(strRefundAmount) = 0 Or Not (IsNumeric(strRefundAmount)) Then
            strStatus = "ERROR"
            strResult = "ERROR : You need to specify an amount to Refund.  This can be any amount greater than zero."
        ElseIf Len(strRefundVendorTxCode) = 0 Then
            strStatus = "ERROR"
            strResult = "ERROR : You need to specify a new VendorTxCode for this Refund transaction."
        ElseIf Len(strRefundDescription) = 0 Then
            strStatus = "ERROR"
            strResult = "ERROR : You need to enter a Description of this Refund transaction."
        Else
            decRefundAmount = CSng(strRefundAmount)

            '** Now let's check we're not exceeding the amount of the orginal transaction minus any other refunds **
            decAmount = decAmount - decRefundAmount
            strSQL = "SELECT Amount FROM tblOrders where RelatedVendorTxCode='" & strVendorTxCode & "' and TxType='REFUND'"
            Dim oSQLCommand = New MySqlCommand(strSQL, objConn)
            oSQLReader = oSQLCommand.ExecuteReader()

            While oSQLReader.Read()
                decAmount = decAmount - CSng(oSQLReader("Amount"))
            End While

            oSQLReader.Close()
            oSQLReader = Nothing
            oSQLCommand = Nothing

            If (decRefundAmount <= 0.0) Then
                strStatus = "ERROR"
                strResult = "ERROR : You need to specify an amount to Refund.  This can be any amount greater than zero."
            ElseIf (decAmount < 0.0) Then
                strStatus = "ERROR"
                strResult = "ERROR : The Refund would exceed the Amount of the original transaction.  Cannot perform the refund."
            Else
                '** Build the Refund message **
                strPost = "VPSProtocol=" & strProtocol
                strPost = strPost & "&TxType=REFUND"
                strPost = strPost & "&Vendor=" & strVendorName
                strPost = strPost & "&VendorTxCode=" & strRefundVendorTxCode
                strPost = strPost & "&Amount=" & FormatNumber(decRefundAmount, 2, -1, 0, 0)
                strPost = strPost & "&Currency=" & strCurrency
                strPost = strPost & "&Description=" & strRefundDescription
                strPost = strPost & "&RelatedVPSTxId=" & strVPSTxId
                strPost = strPost & "&RelatedVendorTxCode=" & strVendorTxCode
                strPost = strPost & "&RelatedSecurityKey=" & strSecurityKey
                strPost = strPost & "&RelatedTxAuthNo=" & strTxAuthNo

                '** The full transaction registration POST has now been built **
                Dim objUTFEncode As New UTF8Encoding
                Dim arrRequest As Byte()
                Dim objStreamReq As Stream
                Dim objStreamRes As StreamReader
                Dim objHttpRequest As HttpWebRequest
                Dim objHttpResponse As HttpWebResponse
                Dim objUri As New Uri(SystemURL(strConnectTo, "refund"))

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
                    strStatus = findField("Status", strResponse)

                    If Left(strStatus, 2) = "OK" Then
                        '** An OK status mean that the transaction has been successfully Refunded **
                        strResult = "SUCCESS : The transaction was REFUNDed successfully and a new Refund transaction was created."

                        '** Get the other values from the POST for storage in the database **
                        strRefundVPSTxId = findField("VPSTxId", strResponse)
                        strRefundTxAuthNo = findField("TxAuthNo", strResponse)
                        strRefundSecurityKey = findField("SecurityKey", strResponse)

                        '** Create the new Refund transaction in the database, linked through to the original transaction **
                        strSQL = "INSERT INTO tblOrders(VendorTxCode,TxType,Amount,Currency,VPSTxId,SecurityKey," & _
                        "TxAuthNo,RelatedVendorTxCode,Status) VALUES("

                        strSQL = strSQL & "'" & strRefundVendorTxCode & "',"
                        strSQL = strSQL & "'REFUND',"
                        strSQL = strSQL & FormatNumber(strRefundAmount, 2, -1, 0, 0) & ","
                        strSQL = strSQL & "'" & strCurrency & "',"
                        strSQL = strSQL & "'" & strRefundVPSTxId & "',"
                        strSQL = strSQL & "'" & strRefundSecurityKey & "',"
                        strSQL = strSQL & strRefundTxAuthNo & ","
                        strSQL = strSQL & "'" & strVendorTxCode & "',"
                        strSQL = strSQL & "'AUTHORISED - REFUND transaction taken through Order Admin area.'"

                        strSQL = strSQL & ")"

                        oSQLCommand = New MySqlCommand(strSQL, objConn)
                        oSQLCommand.ExecuteReader()
                        oSQLCommand = Nothing
                    Else
                        '** All other Statuses are errors of one form or another.  Display them on the screen with no database updates **
                        strResult = strstatus & " : " & findField("StatusDetail", strResponse)

                    End If

                End If

                On Error GoTo 0

            End If
        End If
        If strStatus.Length > 0 Then
            lblResults.Text = strResult
            lblStatus.Text = strStatus
            pnlStatus.Visible = True
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
