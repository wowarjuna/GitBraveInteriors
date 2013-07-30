Imports includes
Imports MySql.Data.MySqlClient
Partial Class orderFailed
    Inherits System.Web.UI.Page
    '**************************************************************************************************
    ' Server ASP Kit Order Failure Page
    '**************************************************************************************************
    ' Description
    ' ===========
    '
    ' This is a placeholder for your Failed Order Completion Page.  It displays the reason for failure
    ' to the customer and retrieves the VendorTxCode from the Querystring to display the transaction 
    ' information on the screen.  You wouldn't display all the information in a live application, 
    ' but during development this page shows how the database has been updated during the order.
    '**************************************************************************************************
    Public objConn As MySqlConnection = oMySQLConnection()
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim strVendorTxCode As String = ""
        Dim strStatus As String = ""
        Dim strReason As String = ""
        Dim strReasonCode As String = ""
        Dim strTemp As String = ""
        Dim oSQLReader As MySqlDataReader
        '** Now check we have a VendorTxCode passed to this page **

        If Len(Request.QueryString("reasonCode")) > 0 Then
            strReasonCode = cleanInput(Request.QueryString("reasonCode"), "Number")
            '** Work out what to tell the customer **
            If strReasonCode = 1 Then
                strReason = "We were unable to locate your transaction in our database. Please try your order again.  You have NOT been charged for this order."
            ElseIf strReasonCode = 2 Then
                strReason = "There was a problem validating the result from our Payment Gateway. To protect you we have cancelled the payment.  Please try your order again."
            Else
                strReason = "The transaction process failed.  We please contact us with the date and time of your order and we will investigate."
            End If

        ElseIf Request.QueryString("VendorTxCode").Length > 0 Then
            pnlTransactionDetails.Visible = True
            strVendorTxCode = cleanInput(Request.QueryString("VendorTxCode"), "VendorTxCode")

            If strVendorTxCode.Length = 0 Then
                '** No VendorTxCode, so take the customer to the home page **
                Response.Clear()
                Server.Transfer("welcome.aspx")
                Response.End()
            End If
            lblVendorTxCodeReference.Text = strVendorTxCode

            '** Set the transaction values for the page. 
            Dim strSQL As String = "SELECT * FROM tblOrders where VendorTxCode='" & SQLSafe(strVendorTxCode) & "'"

            Dim oSQLCommand = New MySqlCommand(strSQL, objConn)
            oSQLReader = oSQLCommand.ExecuteReader()
            oSQLReader.Read()

            strStatus = oSQLReader("Status")

            '** Work out what to tell the customer **
            If Left(strStatus, 8) = "DECLINED" Then
                strReason = "You payment was declined by the bank.  This could be due to insufficient funds, or incorrect card details."
            ElseIf Left(strStatus, 7) = "ABORTED" Then
                strReason = "You chose to Cancel your order on the payment pages.  If you wish to change your order and resubmit it you can do so here." & _
                            " If you have questions or concerns about ordering online, please contact us at [your number]."
            ElseIf Left(strStatus, 8) = "REJECTED" Then
                strReason = "Your order did not meet our minimum fraud screening requirements." & _
                            " If you have questions about our fraud screening rules, or wish to contact us to discuss this, please call [your number]."
            ElseIf Left(strStatus, 5) = "ERROR" Then
                strReason = "We could not process your order because our Payment Gateway service was experiencing difficulties." & _
                            " You can place the order over the telephone instead by calling [your number]."
            Else
                strReason = "The transaction process failed.  We please contact us with the date and time of your order and we will investigate."
            End If

            If strConnectTo <> "LIVE" Then
                pnlTest.Visible = True

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
            Else
                oSQLReader.Close()
                oSQLReader = Nothing
                oSQLCommand = Nothing
            End If
        Else
            '** No VendorTxCode or reason code, so take the customer to the home page **
            Response.Clear()
            Server.Transfer("welcome.aspx")
            Response.End()
        End If
        lblFailureReason.Text = strReason
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
