Imports includes
Imports MySql.Data.MySqlClient
Imports System.Web.Security.FormsAuthentication
Partial Class notificationPage
    Inherits System.Web.UI.Page
    '**************************************************************************************************
    ' Description
    ' ===========
    '
    ' This page handles the notification POSTs from Server.  It should be made externally visible
    ' so that Server can send messages to over either HTTP or HTTPS.
    ' The code validates the Server POST using MD5 hashing, updates the database accordingly,
    ' and replies with a RedirectURL to which Server will send your customer.  This is normally your
    ' order completion page, or a page to handle failures or cancellations.
    '**************************************************************************************************
    Public objConn As MySqlConnection = oMySQLConnection()
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim strStatus As String
        Dim strVendorTxCode As String
        Dim strVPSTxId As String
        Dim strSecurityKey As String
        Dim strSQL As String
        Dim oSQLReader As MySqlDataReader
        Dim oSQLCommand As MySqlCommand
        Dim strStatusDetail As String
        Dim strTxAuthNo As String
        Dim strAVSCV2 As String
        Dim strAddressResult As String
        Dim strPostCodeResult As String
        Dim strCV2Result As String
        Dim strGiftAid As String
        Dim str3DSecureStatus As String
        Dim strCAVV As String
        Dim strAddressStatus As String
        Dim strPayerStatus As String
        Dim strCardType As String
        Dim strLast4Digits As String
        Dim strMySignature As String
        Dim strVPSSignature As String
        Dim strMessage As String
        Dim strDBStatus As String
        Dim strRedirectPage As String

        strStatus = cleanInput(Request.Form("Status"), "Text")
        strVendorTxCode = cleanInput(Request.Form("VendorTxCode"), "VendorTxCode")
        strVPSTxId = cleanInput(Request.Form("VPSTxId"), "Text")

        '** Using the VPSTxId and VendorTxCode, we can retrieve our SecurityKey from our database **
        '** This enables us to validate the POST to ensure it came from the Sage Pay Systems **
        objConn = oMySQLConnection()
        strSQL = "SELECT SecurityKey FROM tblOrders where VendorTxCode='" & SQLSafe(strVendorTxCode) & "' " & _
                "and VPSTxId='" & SQLSafe(strVPSTxId) & "'"
        oSQLReader = New MySqlCommand(strSQL, objConn).ExecuteReader
        oSQLReader.Read()
        If oSQLReader.HasRows Then
            strSecurityKey = oSQLReader("SecurityKey")
        End If
        oSQLReader.Close()
        oSQLReader = Nothing
        DestroyConnection(objConn)

        If strSecurityKey.Length = 0 Then
            '** We cannot find a record of this order in the database, so something isn't right **
            '** To protect the customer, we should send back an INVALID response.  This will prevent **
            '** the systems from settling any authorised transactions.  We will also send a **
            '** RedirectURL that points to our orderFailure page, passing details of the error **
            '** in the Query String so that the page knows how to respond to the customer **
            Response.Clear()
            Response.ContentType = "text/plain"
            Response.Write("Status=INVALID" & vbCrLf)

            '** Only use the Internal FQDN value during development.  In LIVE systems, always use the actual FQDN **
            If strConnectTo = "LIVE" Then
                Response.Write("RedirectURL=" & strYourSiteFQDN & strVirtualDir & "/orderFailed.aspx?reasonCode=001" & vbCrLf)
            Else
                Response.Write("RedirectURL=" & strYourSiteInternalFQDN & strVirtualDir & "/orderFailed.aspx?reasonCode=001" & vbCrLf)
            End If

            Response.Write("StatusDetail=Unable to find the transaction in our database." & vbCrLf)
            Response.End()

        Else
            '** We've found the order in the database, so now we can validate the message **
            '** First blank out our result variables **
            strStatusDetail = ""
            strTxAuthNo = ""
            strAVSCV2 = ""
            strAddressResult = ""
            strPostCodeResult = ""
            strCV2Result = ""
            strGiftAid = ""
            str3DSecureStatus = ""
            strCAVV = ""
            strAddressStatus = ""
            strPayerStatus = ""
            strCardType = ""
            strLast4Digits = ""
            strMySignature = ""

            '** Now get the VPSSignature value from the POST, and the StatusDetail in case we need it **
            strVPSSignature = cleanInput(Request.Form("VPSSignature"), "Text")
            strStatusDetail = cleanInput(Request.Form("StatusDetail"), "Text")

            '** Retrieve the other fields, from the POST if they are present **
            If Len(Request.Form("TxAuthNo")) > 0 Then strTxAuthNo = cleanInput(Request.Form("TxAuthNo"), "Number")
            If Len(Request.Form("AVSCV2")) > 0 Then strAVSCV2 = cleanInput(Request.Form("AVSCV2"), "Text")
            If Len(Request.Form("AddressResult")) > 0 Then strAddressResult = cleanInput(Request.Form("AddressResult"), "Text")
            If Len(Request.Form("PostCodeResult")) > 0 Then strPostCodeResult = cleanInput(Request.Form("PostCodeResult"), "Text")
            If Len(Request.Form("CV2Result")) > 0 Then strCV2Result = cleanInput(Request.Form("CV2Result"), "Text")
            If Len(Request.Form("GiftAid")) > 0 Then strGiftAid = cleanInput(Request.Form("GiftAid"), "Number")
            If Len(Request.Form("3DSecureStatus")) > 0 Then str3DSecureStatus = cleanInput(Request.Form("3DSecureStatus"), "Text")
            If Len(Request.Form("CAVV")) > 0 Then strCAVV = cleanInput(Request.Form("CAVV"), "Text")
            If Len(Request.Form("AddressStatus")) > 0 Then strAddressStatus = cleanInput(Request.Form("AddressStatus"), "Text")
            If Len(Request.Form("PayerStatus")) > 0 Then strPayerStatus = cleanInput(Request.Form("PayerStatus"), "Text")
            If Len(Request.Form("CardType")) > 0 Then strCardType = cleanInput(Request.Form("CardType"), "Text")
            If Len(Request.Form("Last4Digits")) > 0 Then strLast4Digits = cleanInput(Request.Form("Last4Digits"), "Number")

            '** Now we rebuilt the POST message, including our security key, and use the MD5 Hash **
            '** component that ships with the kit to create our own signature to compare with **
            '** the contents of the VPSSignature field in the POST.  Check the Server protocol **
            '** if you need clarification on this process **
            strMessage = strVPSTxId & strVendorTxCode & strStatus & strTxAuthNo & strVendorName & strAVSCV2 & strSecurityKey & _
               strAddressResult & strPostCodeResult & strCV2Result & strGiftAid & str3DSecureStatus & strCAVV & _
               strAddressStatus & strPayerStatus & strCardType & strLast4Digits

            strMySignature = HashPasswordForStoringInConfigFile(strMessage, "MD5")

            '** We can now compare our MD5 Hash signature with that from Server **
            If strMySignature <> strVPSSignature Then

                '** If the signatures DON'T match, we should mark the order as tampered with, and **
                '** send back a Status of INVALID and failure page RedirectURL **

                objConn = oMySQLConnection()
                strSQL = "UPDATE tblOrders set Status='TAMPER WARNING! Signatures do not match for this Order.  The Order was Cancelled.' " & _
                    "where VendorTxCode='" & SQLSafe(strVendorTxCode) & "'"
                oSQLCommand = New MySqlCommand(strSQL, objConn)
                oSQLCommand.ExecuteReader()
                oSQLCommand = Nothing
                DestroyConnection(objConn)

                Response.Clear()
                Response.ContentType = "text/plain"
                Response.Write("Status=INVALID" & vbCrLf)

                '** Only use the Internal FQDN value during development.  In LIVE systems, always use the actual FQDN **
                If strConnectTo = "LIVE" Then
                    Response.Write("RedirectURL=" & strYourSiteFQDN & strVirtualDir & "/orderFailed.aspx?reasonCode=002" & vbCrLf)
                Else
                    Response.Write("RedirectURL=" & strYourSiteInternalFQDN & strVirtualDir & "/orderFailed.aspx?reasonCode=002" & vbCrLf)
                End If

                Response.Write("StatusDetail=Cannot match the MD5 Hash. Order might be tampered with." & vbCrLf)
                Response.End()

            Else

                '** Great, the signatures DO match, so we can update the database and redirect the user appropriately **
                If strStatus = "OK" Then
                    strDBStatus = "AUTHORISED - The transaction was successfully authorised with the bank."
                ElseIf strStatus = "NOTAUTHED" Then
                    strDBStatus = "DECLINED - The transaction was not authorised by the bank."
                ElseIf strStatus = "ABORT" Then
                    strDBStatus = "ABORTED - The customer clicked Cancel on the payment pages, or the transaction was timed out due to customer inactivity."
                ElseIf strStatus = "REJECTED" Then
                    strDBStatus = "REJECTED - The transaction was failed by your 3D-Secure or AVS/CV2 rule-bases."
                ElseIf strStatus = "AUTHENTICATED" Then
                    strDBStatus = "AUTHENTICATED - The transaction was successfully 3D-Secure Authenticated and can now be Authorised."
                ElseIf strStatus = "REGISTERED" Then
                    strDBStatus = "REGISTERED - The transaction was could not be 3D-Secure Authenticated, but has been registered to be Authorised."
                ElseIf strStatus = "ERROR" Then
                    strDBStatus = "ERROR - There was an error during the payment process.  The error details are: " & SQLSafe(strStatusDetail)
                Else
                    strDBStatus = "UNKNOWN - An unknown status was returned from Sage Pay.  The Status was: " & SQLSafe(strStatus) & _
                    ", with StatusDetail:" & SQLSafe(strStatusDetail)
                End If

                '** Update our database with the results from the Notification POST **
                strSQL = "UPDATE tblOrders set Status='" & strDBStatus & "'"
                If Len(strTxAuthNo) > 0 Then strSQL = strSQL & ",TxAuthNo=" & SQLSafe(strTxAuthNo)
                If Len(strAVSCV2) > 0 Then strSQL = strSQL & ",AVSCV2='" & SQLSafe(strAVSCV2) & "'"
                If Len(strAddressResult) > 0 Then strSQL = strSQL & ",AddressResult='" & SQLSafe(strAddressResult) & "'"
                If Len(strPostCodeResult) > 0 Then strSQL = strSQL & ",PostCodeResult='" & SQLSafe(strPostCodeResult) & "'"
                If Len(strCV2Result) > 0 Then strSQL = strSQL & ",CV2Result='" & SQLSafe(strCV2Result) & "'"
                If Len(strGiftAid) > 0 Then strSQL = strSQL & ",GiftAid=" & SQLSafe(strGiftAid)
                If Len(str3DSecureStatus) > 0 Then strSQL = strSQL & ",ThreeDSecureStatus='" & SQLSafe(str3DSecureStatus) & "'"
                If Len(strCAVV) > 0 Then strSQL = strSQL & ",CAVV='" & SQLSafe(strCAVV) & "'"
                If Len(strAddressStatus) > 0 Then strSQL = strSQL & ",AddressStatus='" & SQLSafe(strAddressStatus) & "'"
                If Len(strPayerStatus) > 0 Then strSQL = strSQL & ",PayerStatus='" & SQLSafe(strPayerStatus) & "'"
                If Len(strCardType) > 0 Then strSQL = strSQL & ",CardType='" & SQLSafe(strCardType) & "'"
                If Len(strLast4Digits) > 0 Then strSQL = strSQL & ",Last4Digits='" & SQLSafe(strLast4Digits) & "'"
                strSQL = strSQL & " where VendorTxCode='" & SQLSafe(strVendorTxCode) & "'"

                objConn = oMySQLConnection()
                oSQLCommand = New MySqlCommand(strSQL, objConn)
                oSQLCommand.ExecuteReader()
                oSQLCommand = Nothing
                DestroyConnection(objConn)

                '** New reply to Server to let the system know we've received the Notification POST **
                Response.Clear()
                Response.ContentType = "text/plain"

                '** Always send a Status of OK if we've read everything correctly.  Only INVALID for messages with a Status of ERROR **
                If strStatus = "ERROR" Then
                    Response.Write("Status=INVALID" & vbCrLf)
                Else
                    Response.Write("Status=OK" & vbCrLf)
                End If

                '** Now decide where to redirect the customer **
                If (strStatus = "OK") Or (strStatus = "AUTHENTICATED") Or (strStatus = "REGISTERED") Then
                    '** If a transaction status is OK, AUTHENTICATED or REGISTERED, we should send the customer to the success page **
                    strRedirectPage = "/orderSuccessful.aspx?VendorTxCode=" & strVendorTxCode
                Else
                    '** The status indicates a failure of one state or another, so send the customer to orderFailed instead **
                    strRedirectPage = "/orderFailed.aspx?VendorTxCode=" & strVendorTxCode
                End If

                '** Only use the Internal FQDN value during development.  In LIVE systems, always use the actual FQDN **
                If strConnectTo = "LIVE" Then
                    Response.Write("RedirectURL=" & strYourSiteFQDN & strVirtualDir & strRedirectPage & vbCrLf)
                Else
                    Response.Write("RedirectURL=" & strYourSiteInternalFQDN & strVirtualDir & strRedirectPage & vbCrLf)
                End If

                '** No need to send a StatusDetail, since we're happy with the POST **
                Response.End()

            End If

        End If

    End Sub
End Class
