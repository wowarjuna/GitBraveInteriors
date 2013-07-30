Imports includes
Imports System.Data
Partial Class orderFailed
    Inherits System.Web.UI.Page
    '**************************************************************************************************
    ' Form ASP Kit Order Successful Page
    '**************************************************************************************************
    ' Description
    ' ===========
    ' This is a placeholder for your Unsuccessful Order Completion Page.  It retrieves the VendorTxCode
    ' from the crypt string and displays the transaction results on the screen, along with an explanation
    ' of the reason for failure.  You wouldn't display all the information in a live application, 
    ' but during development this page shows everything sent back in the confirmation screen.
    '**************************************************************************************************
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim strDecoded As String
        Dim strCrypt As String
        Dim strGiftAid As String = "No"
        Dim strVendorTxCode As String
        Dim strStatus As String
        Dim strReason As String
        '** Now check we have a Crypt field passed to this page **
        strCrypt = Request.QueryString("Crypt")
        If Len(strCrypt) = 0 Then
            Response.Clear()
            Server.Transfer("welcome.aspx")
            Response.End()
        End If

        '** Now decode the Crypt field and extract the results **
        strDecoded = DecodeAndDecrypt(strCrypt)

        If getToken(strDecoded, "GiftAid") = "1" Then
            strGiftAid = "Yes"
        End If

        strStatus = getToken(strDecoded, "Status")
        '** Determine the reason this transaction was unsuccessful **
        If strStatus = "NOTAUTHED" Then
            strReason = "You payment was declined by the bank.  This could be due to insufficient funds, or incorrect card details."
        ElseIf strStatus = "ABORT" Then
            strReason = "You chose to Cancel your order on the payment pages.  If you wish to change your order and resubmit it you " & _
            "can do so here. If you have questions or concerns about ordering online, please contact us at [your number]."
        ElseIf strStatus = "REJECTED" Then
            strReason = "Your order did not meet our minimum fraud screening requirements." & _
            " If you have questions about our fraud screening rules, or wish to contact us to discuss this, please call [your number]."
        ElseIf strStatus = "INVALID" Or strStatus = "MALFORMED" Then
            strReason = "We could not process your order because we have been unable to register your transaction with our Payment Gateway." & _
            " You can place the order over the telephone instead by calling [your number]."
        ElseIf strStatus = "ERROR" Then
            strReason = "We could not process your order because our Payment Gateway service was experiencing difficulties." & _
            " You can place the order over the telephone instead by calling [your number]."
        Else
            strReason = "The transaction process failed.  We please contact us with the date and time of your order and we will investigate."
        End If

        strVendorTxCode = getToken(strDecoded, "VendorTxCode")
        lblVendorTxCode.Text = Server.HtmlEncode(strVendorTxCode)
        lblVendorTxCodeReference.Text = Server.HtmlEncode(strVendorTxCode)
        lblStatus.Text = Server.HtmlEncode(strStatus)
        lblStatusDetail.Text = Server.HtmlEncode(getToken(strDecoded, "StatusDetail"))
        lblAmount.Text = Server.HtmlEncode(getToken(strDecoded, "Amount") & " " & strCurrency)
        lblVPSTxId.Text = Server.HtmlEncode(getToken(strDecoded, "VPSTxId"))
        lblVPSAuthCode.Text = Server.HtmlEncode(getToken(strDecoded, "TxAuthNo"))
        lblAVSCV2Result.Text = Server.HtmlEncode("- Address: " & getToken(strDecoded, "AddressResult") & ", Post Code: " & getToken(strDecoded, "PostCodeResult") & ", CV2: " & getToken(strDecoded, "CV2Result"))
        lblGiftAid.Text = Server.HtmlEncode(strGiftAid)
        lbl3DSecure.Text = Server.HtmlEncode(getToken(strDecoded, "3DSecureStatus"))
        lblCAVV.Text = Server.HtmlEncode(getToken(strDecoded, "CAVV"))
        lblCardType.Text = Server.HtmlEncode(getToken(strDecoded, "CardType"))
        lblLast4Digits.Text = Server.HtmlEncode(getToken(strDecoded, "Last4Digits"))
        lblPPAddressStatus.Text = Server.HtmlEncode(getToken(strDecoded, "AddressStatus")) '** PayPal transactions only
        lblPPPayerStatus.Text = Server.HtmlEncode(getToken(strDecoded, "PayerStatus"))     '** PayPal transactions only

        lblReason.Text = strReason

        If strConnectTo <> "LIVE" Then
            pnlTest.Visible = "true"
        End If

        '** Empty the cart, we're done with it now because the order is successful **
        clearSessions()

    End Sub
    Function clearSessions()
        Session("VendorTxCode") = ""
        Session("strCart") = ""
    End Function

    Protected Sub proceed_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles proceed.Click
        Response.Clear()
        Response.Redirect("welcome.aspx")
        Response.End()
    End Sub

End Class
