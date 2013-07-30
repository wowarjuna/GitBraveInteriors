Imports includes
Imports MySql.Data.MySqlClient
Imports System.Data
Partial Class orderSuccessful
    Inherits System.Web.UI.Page
    '**************************************************************************************************
    ' Server ASP Kit Order Successful Page
    '**************************************************************************************************
    ' Description
    ' ===========
    '
    ' This is a placeholder for your Successful Order Completion Page.  It retrieves the VendorTxCode
    ' from the Querystring and displays the transaction results on the screen.  You wouldn't display 
    ' all the information in a live application, but during development this page shows how the database
    ' has been updated during the order.
    '**************************************************************************************************
    Public objConn As MySqlConnection = oMySQLConnection()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim strVendorTxCode As String = ""
        Dim oSQLReader As MySqlDataReader
        Dim strTemp As String

        '** Now check we have a VendorTxCode passed to this page **
        strVendorTxCode = cleanInput(Request.QueryString("VendorTxCode"), "VendorTxCode")
        If strVendorTxCode.Length = 0 Then
            '** No VendorTxCode, so take the customer to the home page **
            Response.Clear()
            Server.Transfer("welcome.aspx")
            Response.End()
        End If

        lblVendorTxCodeReference.Text = Session("VendorTxCode")

        If strConnectTo <> "LIVE" Then
            pnlTest.Visible = True
            '** Set the transaction values for the page. 
            Dim strSQL As String = "SELECT * FROM tblOrders where VendorTxCode='" & SQLSafe(strVendorTxCode) & "'"

            Dim oSQLCommand = New MySqlCommand(strSQL, objConn)
            oSQLReader = oSQLCommand.ExecuteReader()
            oSQLReader.Read()

            lblBillingName.Text = nullToStr(oSQLReader("BillingFirstnames")) & " " & nullToStr(oSQLReader("BillingSurname"))
            lblBillingPhone.Text = nullToStr(oSQLReader("BillingPhone"))
            lblBillingEmail.Text = nullToStr(oSQLReader("CustomerEmail"))
            lblBillingAddress.Text = nullToStr(oSQLReader("BillingAddress1")) & "<BR>"
            strTemp = nullToStr(oSQLReader("BillingAddress2"))
            If strTemp <> "" Then lblBillingAddress.Text &= strTemp
            lblBillingAddress.Text &= nullToStr(oSQLReader("BillingCity")) & "&nbsp;"
            strTemp = nullToStr(oSQLReader("BillingState"))
            If strTemp <> "" Then lblBillingAddress.Text &= strTemp
            lblBillingAddress.Text &= nullToStr(oSQLReader("BillingPostCode")) & "<BR>"
            litBillingCountry.Text = nullToStr(oSQLReader("BillingCountry"))

            lblDeliveryName.Text = nullToStr(oSQLReader("DeliveryFirstnames")) & " " & nullToStr(oSQLReader("DeliverySurname"))
            lblDeliveryPhone.Text = nullToStr(oSQLReader("DeliveryPhone"))
            lblDeliveryAddress.Text = nullToStr(oSQLReader("DeliveryAddress1")) & "<BR>"
            strTemp = nullToStr(oSQLReader("DeliveryAddress2"))
            If strTemp <> "" Then lblDeliveryAddress.Text &= strTemp
            lblDeliveryAddress.Text &= nullToStr(oSQLReader("DeliveryCity")) & "&nbsp;"
            strTemp = nullToStr(oSQLReader("DeliveryState"))
            If strTemp <> "" Then lblDeliveryAddress.Text &= strTemp
            lblDeliveryAddress.Text &= nullToStr(oSQLReader("DeliveryPostCode")) & "<BR>"
            litDeliveryCountry.Text = nullToStr(oSQLReader("DeliveryCountry"))

            lblVendorTxCode.Text = oSQLReader("VendorTxCode")
            lblTransactionType.Text = oSQLReader("TxType")
            lblStatus.Text = oSQLReader("Status")
            lblAmount.Text = FormatNumber(oSQLReader("Amount"), 2, TriState.True) & " " & includes.strCurrency

            lblVPSTxId.Text = oSQLReader("VPSTxId")
            lblSecurityKey.Text = nullToStr(oSQLReader("SecurityKey"))
            lblVPSAuthCode.Text = nullToStr(oSQLReader("TxAuthNo"))
            lblAVSCV2.Text = nullToStr(oSQLReader("AVSCV2"))
            lblGiftAid.Text = oSQLReader("GiftAid")
            lbl3DSecure.Text = nullToStr(oSQLReader("ThreeDSecureStatus"))
            lblCAVV.Text = nullToStr(oSQLReader("CAVV"))
            lblAddressStatus.Text = nullToStr(oSQLReader("AddressStatus"))
            lblPayerStatus.Text = nullToStr(oSQLReader("PayerStatus"))
            lblLast4Digits.Text = nullToStr(oSQLReader("Last4Digits"))
            lblCardType.Text = nullToStr(oSQLReader("CardType"))

            oSQLReader.Close()
            oSQLReader = Nothing
            oSQLCommand = Nothing

            '**build basket from database
            strSQL = "SELECT op.Price,op.Quantity,p.ProductId, p.Description FROM tblOrderProducts op" & _
                              " inner join tblProducts p on op.ProductID=p.ProductID " & _
                              " where op.VendorTxCode='" & SQLSafe(strVendorTxCode) & "'"

            oSQLCommand = New MySqlCommand(strSQL, objConn)

            '**return data to repeater control
            dataRepBasket.DataSource = oSQLCommand.ExecuteReader()
            dataRepBasket.DataBind()

            oSQLCommand = Nothing

        End If

    End Sub
    Sub clearSessions()
        Session("VendorTxCode") = ""
        Session("strCart") = ""
    End Sub

    Protected Sub proceed_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles proceed.Click
        Response.Clear()
        clearSessions()
        Response.Redirect("welcome.aspx")
        Response.End()
    End Sub
    Protected Sub admin_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles admin.Click
        Response.Clear()
        clearSessions()
        Server.Transfer("orderAdmin.aspx")
        Response.End()
    End Sub

    Protected Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Unload
        DestroyConnection(objConn)
    End Sub
End Class
