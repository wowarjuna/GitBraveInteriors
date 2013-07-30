Imports includes
Imports System.Data
Partial Class orderSuccessful
    Inherits System.Web.UI.Page
    '**************************************************************************************************
    ' Form ASP Kit Order Successful Page
    '**************************************************************************************************
    ' Description
    ' ===========
    '
    ' This is a placeholder for your Successful Order Completion Page.  It retrieves the VendorTxCode
    ' from the crypt string and displays the transaction results on the screen.  You wouldn't display 
    ' all the information in a live application, but during development this page shows everything
    ' sent back in the confirmation screen.
    '**************************************************************************************************
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim strDecoded As String
        Dim strCrypt As String
        Dim strGiftAid As String = "No"
        Dim strVendorTxCode As String
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

        strVendorTxCode = getToken(strDecoded, "VendorTxCode")
        lblVendorTxCode.Text = Server.HtmlEncode(strVendorTxCode)
        lblVendorTxCodeReference.Text = Server.HtmlEncode(strVendorTxCode)
        lblStatus.Text = Server.HtmlEncode(getToken(strDecoded, "Status"))
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
        lblPPAddressStatus.Text = Server.HtmlEncode(getToken(strDecoded, "AddressStatus"))
        lblPPPayerStatus.Text = Server.HtmlEncode(getToken(strDecoded, "PayerStatus"))
								
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
