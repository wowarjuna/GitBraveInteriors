Imports includes
Imports MySql.Data.MySqlClient
Imports System.IO
Imports System.Net
Partial Class repeat
    Inherits System.Web.UI.Page
    '**************************************************************************************************
    ' Server ASP Repeat Transaction Page
    '**************************************************************************************************
    ' Description
    ' ===========
    '
    ' This page perform builds a REPEAT POST based on the details you enter here.  The contents of the
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
        Dim decRepeatAmount As Decimal
        Dim strRepeatVendorTxCode As String
        Dim strRepeatDescription As String

        Dim oSQLReader As MySqlDataReader
        '** Check we have a vendortxcode in the session.  If not, or of the user clicked back then return to the orderAdmin screen. **
        strVendorTxCode = Session("strVendorTxCode")
        If (strVendorTxCode.Length = 0) Or (Request.Form("back.x") > 0) Then
            Response.Clear()
            Response.Redirect("orderAdmin.aspx")
            Response.End()
        End If

        If strStatus.Length = 0 Then
            pnlPreRepeat.Visible = True
            pnlRepeatDetails.Visible = True
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

        '** Since no buttons have been clicked, generate a random VendorTxCode for this Repeat, based on on the original VendorTxCode **
        '** You can edit this in the boxes provided, but you probably wouldn't in a production environment **
        Randomize()
        strRepeatVendorTxCode = Left("REP-" & CStr(Decimal.Round(Rnd() * 100000)) & "-" & strVendorTxCode, 40)
        strRepeatDescription = "Repeat against " & strVendorTxCode

        decRepeatAmount = 0.0

        lblvendorTxCode.Text = strVendorTxCode
        lblVPSTxId.Text = strVPSTxId
        lblSecurityKey.Text = strSecurityKey
        lblTxAuthNo.Text = strTxAuthNo
        RepeatVendorTxCode.Text = strRepeatVendorTxCode
        RepeatDescription.Text = "Repeat against " & strVendorTxCode
        RepeatAmount.Text = decRepeatAmount
        lblResults.Text = strResult

    End Sub
    Protected Sub proceed_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles proceed.Click
        Dim strRepeatAmount As String
        Dim strRepeatDescription As String
        Dim strRepeatVendorTxCode As String
        Dim strResult As String
        Dim decRepeatAmount As Decimal
        Dim strPost As String
        Dim strSQL As String
        Dim strResponse As String
        Dim strPageError As String
        Dim strRepeatVPSTxId As String
        Dim strRepeatTxAuthNo As String
        Dim strRepeatSecurityKey As String
        Dim oSQLReader As MySqlDataReader

        strRepeatAmount = cleanInput(Request.Form("RepeatAmount"), "Number")
        strRepeatDescription = cleanInput(Request.Form("RepeatDescription"), "Text")
        strRepeatVendorTxCode = cleanInput(Request.Form("RepeatVendorTxCode"), "VendorTxCode")

        '** Validate the Repeat amount.  It must be a number, greater than 0  **
        If Len(strRepeatAmount) = 0 Or Not (IsNumeric(strRepeatAmount)) Then
            strstatus = "ERROR"
            strResult = "ERROR : You need to specify an amount to Repeat.  This can be any amount greater than zero."
        ElseIf Len(strRepeatVendorTxCode) = 0 Then
            strstatus = "ERROR"
            strResult = "ERROR : You need to specify a new VendorTxCode for this Repeat transaction."
        ElseIf Len(strRepeatDescription) = 0 Then
            strstatus = "ERROR"
            strResult = "ERROR : You need to enter a Description of this Repeat transaction."
        Else
            decRepeatAmount = CSng(strRepeatAmount)

            '** Now let's check we're not exceeding the amount of the orginal transaction minus any other Repeats **
            decAmount = decAmount - decRepeatAmount
            strSQL = "SELECT Amount FROM tblOrders where RelatedVendorTxCode='" & strVendorTxCode & "' and TxType='Repeat'"
            Dim oSQLCommand = New MySqlCommand(strSQL, objConn)
            oSQLReader = oSQLCommand.ExecuteReader()

            While oSQLReader.Read()
                decAmount = decAmount - CSng(oSQLReader("Amount"))
            End While

            oSQLReader.Close()
            oSQLReader = Nothing
            oSQLCommand = Nothing

            If (decRepeatAmount <= 0.0) Then
                strstatus = "ERROR"
                strResult = "ERROR : You need to specify an amount to Repeat.  This can be any amount greater than zero."
            Else
                '** Build the Repeat message **
                strPost = "VPSProtocol=" & strProtocol
                strPost = strPost & "&TxType=REPEAT"
                strPost = strPost & "&Vendor=" & strVendorName
                strPost = strPost & "&VendorTxCode=" & strRepeatVendorTxCode
                strPost = strPost & "&Amount=" & FormatNumber(decRepeatAmount, 2, -1, 0, 0)
                strPost = strPost & "&Currency=" & strCurrency
                strPost = strPost & "&Description=" & strRepeatDescription
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
                Dim objUri As New Uri(SystemURL(strConnectTo, "repeat"))

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
                        '** An OK status mean that the transaction has been successfully Repeated **
                        strResult = "SUCCESS : The transaction was Repeated successfully and a new Repeat transaction was created."

                        '** Get the other values from the POST for storage in the database **
                        strRepeatVPSTxId = findField("VPSTxId", strResponse)
                        strRepeatTxAuthNo = findField("TxAuthNo", strResponse)
                        strRepeatSecurityKey = findField("SecurityKey", strResponse)

                        '** Create the new Repeat transaction in the database, linked through to the original transaction **
                        strSQL = "INSERT INTO tblOrders(VendorTxCode,TxType,Amount,Currency,VPSTxId,SecurityKey," & _
                        "TxAuthNo,RelatedVendorTxCode,Status) VALUES("

                        strSQL = strSQL & "'" & strRepeatVendorTxCode & "',"
                        strSQL = strSQL & "'Repeat',"
                        strSQL = strSQL & FormatNumber(strRepeatAmount, 2, -1, 0, 0) & ","
                        strSQL = strSQL & "'" & strCurrency & "',"
                        strSQL = strSQL & "'" & strRepeatVPSTxId & "',"
                        strSQL = strSQL & "'" & strRepeatSecurityKey & "',"
                        strSQL = strSQL & strRepeatTxAuthNo & ","
                        strSQL = strSQL & "'" & strVendorTxCode & "',"
                        strSQL = strSQL & "'AUTHORISED - Repeat transaction taken through Order Admin area.'"

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

        If strstatus.Length > 0 Then
            pnlStatus.Visible = True
            pnlResponse.Visible = True
            pnlRepeatDetails.Visible = False
        End If

    End Sub

    Protected Sub back_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles back.Click
        Response.Clear()
        Server.Transfer("orderAdmin.aspx")
        Response.End()
    End Sub

    Function getStyle() As String
        If strstatus = "OK" Or strstatus = "Authenticated" Or strstatus = "Registered" Then
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
