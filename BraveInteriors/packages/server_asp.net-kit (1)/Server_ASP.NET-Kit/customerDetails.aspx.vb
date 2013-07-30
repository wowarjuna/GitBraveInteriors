Imports includes
Partial Class customerDetails
    Inherits System.Web.UI.Page
    '**************************************************************************************************
    ' Description
    ' ===========
    '
    ' Asks for customer details, such as name, address and contact details.  It checks the session
    ' object to autocomplete fields where possible.  If the customer wishes to proceed, the required
    ' fields are validated and if everything is okay, the session object is populated and the
    ' order confirmation screen displayed.
    '**************************************************************************************************

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreLoad
        Dim strCart As String

        '** Check we have a cart in the session.  If not, go back to the buildOrder page to get one **
        strCart = Session("strCart")
        If String.IsNullOrEmpty(strCart) Then
            Response.Clear()
            Server.Transfer("buildOrder.aspx")
            Response.End()
        End If

        If Not Page.IsPostBack Then
            txtBillingFirstnames.Text = Session("strBillingFirstnames")
            txtBillingSurname.Text = Session("strBillingSurname")
            txtBillingAddress1.Text = Session("strBillingAddress1")
            txtBillingAddress2.Text = Session("strBillingAddress2")
            txtBillingCity.Text = Session("strBillingCity")
            txtBillingPostCode.Text = Session("strBillingPostCode")
            txtBillingState.Text = Session("strBillingState")
            txtBillingPhone.Text = Session("strBillingPhone")
            txtCustomerEMail.Text = Session("strCustomerEMail")

            txtDeliveryFirstnames.Text = Session("strDeliveryFirstnames")
            txtDeliverySurname.Text = Session("strDeliverySurname")
            txtDeliveryAddress1.Text = Session("strDeliveryAddress1")
            txtDeliveryAddress2.Text = Session("strDeliveryAddress2")
            txtDeliveryCity.Text = Session("strDeliveryCity")
            txtDeliveryPostCode.Text = Session("strDeliveryPostCode")
            txtDeliveryState.Text = Session("strDeliveryState")
            txtDeliveryPhone.Text = Session("strDeliveryPhone")
        End If

    End Sub
    Protected Sub proceed_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles proceed.Click

        Dim bIsDeliverySame As Boolean
        Dim txtBillingCountry As String
        Dim txtDeliveryCountry As String
        Dim strPageError As String

        '** Validate and clean the user input here **
        txtBillingFirstnames.Text = cleanInput(txtBillingFirstnames.Text, "Text")
        txtBillingSurname.Text = cleanInput(txtBillingSurname.Text, "Text")
        txtBillingAddress1.Text = cleanInput(txtBillingAddress1.Text, "Text")
        txtBillingAddress2.Text = cleanInput(txtBillingAddress2.Text, "Text")
        txtBillingCity.Text = cleanInput(txtBillingCity.Text, "Text")
        txtBillingPostCode.Text = cleanInput(txtBillingPostCode.Text, "Text")
        txtBillingCountry = cleanInput(Request.Form("txtBillingCountry"), "Text")
        txtBillingState.Text = cleanInput(txtBillingState.Text, "Text")
        txtBillingPhone.Text = cleanInput(txtBillingPhone.Text, "Text")
        txtCustomerEMail.Text = cleanInput(txtCustomerEMail.Text, "Text")
        txtDeliveryFirstnames.Text = cleanInput(txtDeliveryFirstnames.Text, "Text")
        txtDeliverySurname.Text = cleanInput(txtDeliverySurname.Text, "Text")
        txtDeliveryAddress1.Text = cleanInput(txtDeliveryAddress1.Text, "Text")
        txtDeliveryAddress2.Text = cleanInput(txtDeliveryAddress2.Text, "Text")
        txtDeliveryCity.Text = cleanInput(txtDeliveryCity.Text, "Text")
        txtDeliveryPostCode.Text = cleanInput(txtDeliveryPostCode.Text, "Text")
        txtDeliveryCountry = cleanInput(Request.Form("txtDeliveryCountry"), "Text")
        txtDeliveryState.Text = cleanInput(txtDeliveryState.Text, "Text")
        txtDeliveryPhone.Text = cleanInput(txtDeliveryPhone.Text, "Text")

        If Request.Form("IsDeliverySame") = "YES" Then
            bIsDeliverySame = True
        Else
            bIsDeliverySame = False
        End If

        '** Validate the compulsory fields **
        If String.IsNullOrEmpty(txtBillingFirstnames.Text) Then
            strPageError = "Please enter your Billing First Names(s) where requested below."
        ElseIf String.IsNullOrEmpty(txtBillingSurname.Text) Then
            strPageError = "Please enter your Billing Surname where requested below."
        ElseIf String.IsNullOrEmpty(txtBillingAddress1.Text) Then
            strPageError = "Please enter your Billing Address Line 1 where requested below."
        ElseIf String.IsNullOrEmpty(txtBillingCity.Text) Then
            strPageError = "Please enter your Billing City where requested below."
        ElseIf String.IsNullOrEmpty(txtBillingPostCode.Text) Then
            strPageError = "Please enter your Billing Post Code where requested below."
        ElseIf String.IsNullOrEmpty(txtBillingCountry) Then
            strPageError = "Please select your Billing Country where requested below."
        ElseIf String.IsNullOrEmpty(txtBillingState.Text) And txtBillingCountry = "US" Then
            strPageError = "Please enter your State code as you have selected United States for billing country."
        ElseIf (Not String.IsNullOrEmpty(txtCustomerEMail.Text)) And (InStr(txtCustomerEMail.Text, "@") = 0 Or InStr(txtCustomerEMail.Text, ".") = 0) Then
            strPageError = "The email address entered was invalid."
        ElseIf Not (bIsDeliverySame) And String.IsNullOrEmpty(txtDeliveryFirstnames.Text) Then
            strPageError = "Please enter your Delivery First Names(s) where requested below."
        ElseIf Not (bIsDeliverySame) And String.IsNullOrEmpty(txtDeliverySurname.Text) Then
            strPageError = "Please enter your Delivery Surname where requested below."
        ElseIf Not (bIsDeliverySame) And String.IsNullOrEmpty(txtDeliveryAddress1.Text) Then
            strPageError = "Please enter your Delivery Address Line 1 where requested below."
        ElseIf Not (bIsDeliverySame) And String.IsNullOrEmpty(txtDeliveryCity.Text) Then
            strPageError = "Please enter your Delivery City where requested below."
        ElseIf Not (bIsDeliverySame) And String.IsNullOrEmpty(txtDeliveryPostCode.Text) Then
            strPageError = "Please enter your Delivery Post Code where requested below."
        ElseIf Not (bIsDeliverySame) And String.IsNullOrEmpty(txtDeliveryCountry) Then
            strPageError = "Please select your Delivery Country where requested below."
        ElseIf Not (bIsDeliverySame) And (String.IsNullOrEmpty(txtDeliveryState.Text)) And (txtDeliveryCountry = "US") Then
            strPageError = "Please enter your State code as you have selected United States for delivery country."
        Else
            '** All validations have passed, so store the details in the session **
            Session("strBillingFirstnames") = txtBillingFirstnames.Text
            Session("strBillingSurname") = txtBillingSurname.Text
            Session("strBillingAddress1") = txtBillingAddress1.Text
            Session("strBillingAddress2") = txtBillingAddress2.Text
            Session("strBillingCity") = txtBillingCity.Text
            Session("strBillingPostCode") = txtBillingPostCode.Text
            Session("strBillingCountry") = txtBillingCountry
            Session("strBillingState") = txtBillingState.Text
            Session("strBillingPhone") = txtBillingPhone.Text
            Session("strCustomerEMail") = txtCustomerEMail.Text
            Session("bIsDeliverySame") = bIsDeliverySame

            If bIsDeliverySame = True Then
                Session("strDeliveryFirstnames") = txtBillingFirstnames.Text
                Session("strDeliverySurname") = txtBillingSurname.Text
                Session("strDeliveryAddress1") = txtBillingAddress1.Text
                Session("strDeliveryAddress2") = txtBillingAddress2.Text
                Session("strDeliveryCity") = txtBillingCity.Text
                Session("strDeliveryPostCode") = txtBillingPostCode.Text
                Session("strDeliveryCountry") = txtBillingCountry
                Session("strDeliveryState") = txtBillingState.Text
                Session("strDeliveryPhone") = txtBillingPhone.Text
            Else
                Session("strDeliveryFirstnames") = txtDeliveryFirstnames.Text
                Session("strDeliverySurname") = txtDeliverySurname.Text
                Session("strDeliveryAddress1") = txtDeliveryAddress1.Text
                Session("strDeliveryAddress2") = txtDeliveryAddress2.Text
                Session("strDeliveryCity") = txtDeliveryCity.Text
                Session("strDeliveryPostCode") = txtDeliveryPostCode.Text
                Session("strDeliveryCountry") = txtDeliveryCountry
                Session("strDeliveryState") = txtDeliveryState.Text
                Session("strDeliveryPhone") = txtDeliveryPhone.Text
            End If

            '** Now go to the order confirmation page **
            Response.Clear()
            Server.Transfer("orderConfirmation.aspx")
            Response.End()
        End If

        '** if there is an error display it 
        If Not String.IsNullOrEmpty(strPageError) Then
            lblError.Text = strPageError
            pnlError.Visible = True
        End If
    End Sub
    Protected Sub back_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles back.Click
        Response.Clear()
        Server.Transfer("buildOrder.aspx")
        Response.End()
    End Sub
End Class
