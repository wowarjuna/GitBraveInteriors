Imports includes
Imports MySql.Data.MySqlClient
Imports System.IO
Imports System.Net
Partial Class cancel
    Inherits System.Web.UI.Page
    '**************************************************************************************************
    ' Server ASP Release Transaction Page
    '**************************************************************************************************
    ' Description
    ' ===========
    '
    ' This page perform builds a CANCEL POST first for display on screen, for you to view to contents,
    ' then POSTs that data to the Gateway to VOID the transaction you selected, displaying the
    ' results and updating the database accordingly
    '**************************************************************************************************
    Public strVPSTxId As String = ""
    Public strSecurityKey As String = ""
    Public strTxAuthNo As String = ""
    Public decAmount As Decimal
    Public strVendorTxCode As String = ""
    Public strPost As String = ""
    Public strStatus As String = ""
    Public objConn As MySqlConnection = oMySQLConnection()


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim strResult As String = ""
        Dim strSQL As String = ""

        Dim oSQLReader As MySqlDataReader
        '** Check we have a vendortxcode in the session.  If not, or of the user clicked back then return to the orderAdmin screen. **
        strVendorTxCode = Session("strVendorTxCode")
        If strVendorTxCode.Length = 0 Then
            Response.Clear()
            Response.Redirect("orderAdmin.aspx")
            Response.End()
        End If

        If strStatus.Length > 0 Then
            pnlStatus.Visible = True
            pnlStatus2.Visible = True
            pnlResponse.Visible = True
        Else
            pnlPreCancel.Visible = True
        End If
        strSQL = "SELECT * FROM tblOrders where VendorTxCode='" & SQLSafe(strVendorTxCode) & "'"

        Dim oSQLCommand = New MySqlCommand(strSQL, objConn)
        oSQLReader = oSQLCommand.ExecuteReader()
        oSQLReader.Read()

        If oSQLReader.HasRows Then
            strVPSTxId = oSQLReader("VPSTxId")
            strSecurityKey = oSQLReader("SecurityKey")
            strTxAuthNo = oSQLReader("TxAuthNo")
        Else
            strStatus = "ERROR"
            strResult = "ERROR : Cannot retrieve the original transaction data from the database."
        End If

        oSQLReader.Close()
        oSQLReader = Nothing
        oSQLCommand = Nothing

        '** Build the Cancel message **
        strPost = "VPSProtocol=" & strProtocol
        strPost = strPost & "&TxType=CANCEL"
        strPost = strPost & "&Vendor=" & strVendorName
        strPost = strPost & "&VendorTxCode=" & strVendorTxCode
        strPost = strPost & "&VPSTxId=" & strVPSTxId
        strPost = strPost & "&SecurityKey=" & strSecurityKey

        lblPost.Text = strPost
        lblResults.Text = strResult

    End Sub

    Protected Sub proceed_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles proceed.Click
        Dim strReleaseAmount As String
        Dim strReleaseDescription As String
        Dim strReleaseVendorTxCode As String
        Dim strResult As String
        Dim decReleaseAmount As Decimal
        Dim strSQL As String
        Dim strResponse As String
        Dim strPageError As String
        Dim strReleaseVPSTxId As String
        Dim strReleaseTxAuthNo As String
        Dim strReleaseSecurityKey As String
        Dim oSQLReader As MySqlDataReader

        strReleaseAmount = cleanInput(Request.Form("ReleaseAmount"), "Number")
        '** The full transaction registration POST has now been built **
        Dim objUTFEncode As New UTF8Encoding
        Dim arrRequest As Byte()
        Dim objStreamReq As Stream
        Dim objStreamRes As StreamReader
        Dim objHttpRequest As HttpWebRequest
        Dim objHttpResponse As HttpWebResponse
        Dim objUri As New Uri(SystemURL(strConnectTo, "cancel"))

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
                '** An OK status mean that the transaction has been successfully Released **
                strResult = "SUCCESS : The transaction was CANCELled successfully"

                '** Update the original transaction to mark that it has been Cancelled.  
                strSQL = "UPDATE tblOrders SET Status='CANCELLED - Authentication Cancelled to prevent further Authorisation' WHERE VendorTxCode='" & strVendorTxCode & "'"

                Dim oSQLCommand = New MySqlCommand(strSQL, objConn)
                oSQLCommand.ExecuteReader()
                oSQLCommand = Nothing
            Else
                '** All other Statuses are errors of one form or another.  Display them on the screen with no database updates **
                strResult = strStatus & " : " & findField("StatusDetail", strResponse)

            End If

        End If

        On Error GoTo 0

        lblResults.Text = strResult
        lblStatus.Text = strStatus
        lblPost.Text = strPost
        lblResponse.Text = strResponse

        If strStatus.Length > 0 Then
            pnlStatus.Visible = True
            pnlStatus2.Visible = True
            pnlResponse.Visible = True
            If Left(strStatus, 2) = "OK" Then
                proceed.Visible = False
            End If
        End If

    End Sub
    Function clearSessions()
        Session("VendorTxCode") = ""
        Session("strCart") = ""
    End Function

    Protected Sub back_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles back.Click
        Response.Clear()
        clearSessions()
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
