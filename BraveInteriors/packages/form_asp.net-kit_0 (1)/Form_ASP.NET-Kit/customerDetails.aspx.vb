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
            txtBillingFirstName.Text = Session("strBillingFirstname")
            txtBillingSurname.Text = Session("strBillingSurname")
            txtBillingAddressLine1.Text = Session("strBillingAddressLine1")
            txtBillingAddressLine2.Text = Session("strBillingAddressLine2")
            txtBillingCity.Text = Session("strBillingCity")
            txtBillingPostCode.Text = Session("strBillingPostCode")
            txtBillingPhone.Text = Session("strBillingPhoneNumber")
            txtBillingEmail.Text = Session("strCustomerEMail")

            txtDeliveryFirstname.Text = Session("strDeliveryFirstname")
            txtDeliverySurname.Text = Session("strDeliverySurname")
            txtDeliveryAddressLine1.Text = Session("strDeliveryAddressLine1")
            txtDeliveryAddressLine2.Text = Session("strDeliveryAddressLine2")
            txtDeliveryCity.Text = Session("strDeliveryCity")
            txtDeliveryPostCode.Text = Session("strDeliveryPostCode")
            txtDeliveryPhone.Text = Session("strDeliveryPhoneNumber")
        End If
    End Sub

    Protected Sub proceed_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles proceed.Click

        Dim strBillingFirstName, strDeliveryFirstName As String
        Dim strBillingSurname, strDeliverySurname As String
        Dim strBillingAddress1, strDeliveryAddress1 As String
        Dim strBillingAddress2, strDeliveryAddress2 As String
        Dim strBillingPostCode, strDeliveryPostCode As String
        Dim strBillingPhone, strDeliveryPhone As String
        Dim strBillingCity, strDeliveryCity As String
        Dim strBillingState, strDeliveryState As String
        Dim strBillingCountry, strDeliveryCountry As String

        Dim strCustomerEMail As String
        Dim bIsDeliverySame As Boolean
        Dim validationResult As FieldValidationResult
        Dim strPageError As String

        '** Validate and clean the user input here **

        '** Billing Details **
        strBillingFirstName = cleanInput(Request.Form("txtBillingFirstName"))
        strBillingSurname = cleanInput(Request.Form("txtBillingSurname"))
        strBillingAddress1 = cleanInput(Request.Form("txtBillingAddressLine1"))
        strBillingAddress2 = cleanInput(Request.Form("txtBillingAddressLine2"))
        strBillingPostCode = cleanInput(Request.Form("txtBillingPostCode"))
        strBillingCity = cleanInput(Request.Form("txtBillingCity"))
        strBillingPhone = cleanInput(Request.Form("txtBillingPhone"))
        strBillingCountry = cleanInput(Request.Form("txtBillingCountry"))
        strBillingState = cleanInput(Request.Form("txtBillingState"))
        strCustomerEMail = cleanInput(Request.Form("txtBillingEmail"))

        '** Delivery Details **
        strDeliveryFirstName = cleanInput(Request.Form("txtDeliveryFirstName"))
        strDeliverySurname = cleanInput(Request.Form("txtDeliverySurname"))
        strDeliveryAddress1 = cleanInput(Request.Form("txtDeliveryAddressLine1"))
        strDeliveryAddress2 = cleanInput(Request.Form("txtDeliveryAddressLine2"))
        strDeliveryPostCode = cleanInput(Request.Form("txtDeliveryPostCode"))
        strDeliveryCity = cleanInput(Request.Form("txtDeliveryCity"))
        strDeliveryPhone = cleanInput(Request.Form("txtDeliveryPhone"))
        strDeliveryCountry = cleanInput(Request.Form("txtDeliveryCountry"))
        strDeliveryState = cleanInput(Request.Form("txtDeliveryState"))


        If Request.Form("IsDeliverySame") = "YES" Then
            bIsDeliverySame = True
            '*** Copy values from billing details to delivery strings ***
            strDeliveryFirstName = strBillingFirstName
            strDeliverySurname = strBillingSurname
            strDeliveryAddress1 = strBillingAddress1
            strDeliveryAddress2 = strBillingAddress2
            strDeliveryPostCode = strBillingPostCode
            strDeliveryState = strBillingState
            strDeliveryPhone = strBillingPhone
            strDeliveryCountry = strBillingCountry
            strDeliveryCity = strBillingCity

            '*** Disable delivery details text boxes ***
            txtDeliveryFirstname.Enabled = False
            txtDeliverySurname.Enabled = False
            txtDeliveryAddressLine1.Enabled = False
            txtDeliveryAddressLine2.Enabled = False
            txtDeliveryPostCode.Enabled = False
            txtDeliveryPhone.Enabled = False
            txtDeliveryCity.Enabled = False
        Else
            bIsDeliverySame = False
        End If


        '** Validate the  fields **

        If Not isValidNameField(strBillingFirstName, validationResult) Then
            strPageError = getValidationMessage(validationResult, "Billing First Name(s)")

        ElseIf Not isValidNameField(strBillingSurname, validationResult) Then
            strPageError = getValidationMessage(validationResult, "Billing Surname")

        ElseIf Not isValidAddressField(strBillingAddress1, True, validationResult) Then
            strPageError = getValidationMessage(validationResult, "Billing Address Line 1")

        ElseIf Not isValidAddressField(strBillingAddress2, False, validationResult) Then
            strPageError = getValidationMessage(validationResult, "Billing Address Line 2")

        ElseIf Not isValidCityField(strBillingCity, validationResult) Then
            strPageError = getValidationMessage(validationResult, "Billing City")

        ElseIf Not isValidPostcodeField(strBillingPostCode, validationResult) Then
            strPageError = getValidationMessage(validationResult, "Billing Post/Zip Code")

        ElseIf String.IsNullOrEmpty(strBillingCountry) Then
            strPageError = "Please select your Billing Country where requested below."

        ElseIf String.IsNullOrEmpty(strBillingState) And strBillingCountry = "US" Then
            strPageError = "Please select your State code as you have selected United States for billing country."

        ElseIf Not isValidPhoneField(strBillingPhone, validationResult) Then
            strPageError = getValidationMessage(validationResult, "Billing Phone")

        ElseIf Not isValidEmailField(strCustomerEMail, validationResult) Then
            strPageError = getValidationMessage(validationResult, "e-mail Address")

        ElseIf Not bIsDeliverySame Then

            If Not isValidNameField(txtDeliveryFirstname.Text, validationResult) Then
                strPageError = getValidationMessage(validationResult, "Delivery First Name(s)")

            ElseIf Not isValidNameField(txtDeliverySurname.Text, validationResult) Then
                strPageError = getValidationMessage(validationResult, "Delivery Surname")

            ElseIf Not isValidAddressField(strDeliveryAddress1, True, validationResult) Then
                strPageError = getValidationMessage(validationResult, "Delivery Address Line 1")

            ElseIf Not isValidAddressField(strDeliveryAddress2, False, validationResult) Then
                strPageError = getValidationMessage(validationResult, "Delivery Address Line 2")

            ElseIf Not isValidCityField(txtDeliveryCity.Text, validationResult) Then
                strPageError = getValidationMessage(validationResult, "Delivery City")

            ElseIf Not isValidPostcodeField(txtDeliveryPostCode.Text, validationResult) Then
                strPageError = getValidationMessage(validationResult, "Delivery Post/Zip Code")

            ElseIf String.IsNullOrEmpty(strDeliveryCountry) Then
                strPageError = "Please select your Delivery Country where requested below."

            ElseIf String.IsNullOrEmpty(strDeliveryState) And strDeliveryCountry = "US" Then
                strPageError = "Please enter your State code as you have selected United States for Delivery country."

            ElseIf Not isValidPhoneField(txtDeliveryPhone.Text, validationResult) Then
                strPageError = getValidationMessage(validationResult, "Delivery Phone")
            End If
        End If

        If strPageError = "" Then
            '** All validations have passed, so store the details in the session **
            Session("strBillingFirstname") = strBillingFirstName
            Session("strBillingSurname") = strBillingSurname
            Session("strBillingAddressLine1") = strBillingAddress1
            Session("strBillingAddressLine2") = strBillingAddress2
            Session("strBillingCity") = strBillingCity
            Session("strBillingPostCode") = strBillingPostCode
            Session("strBillingCountry") = strBillingCountry
            Session("strBillingState") = strBillingState
            Session("strBillingPhoneNumber") = strBillingPhone
            Session("strCustomerEMail") = strCustomerEMail
            Session("bIsDeliverySame") = bIsDeliverySame

            If bIsDeliverySame = True Then
                Session("strDeliveryFirstname") = strBillingFirstName
                Session("strDeliverySurname") = strBillingSurname
                Session("strDeliveryAddressLine1") = strBillingAddress1
                Session("strDeliveryAddressLine2") = strBillingAddress2
                Session("strDeliveryCity") = strBillingCity
                Session("strDeliveryPostCode") = strBillingPostCode
                Session("strDeliveryCountry") = strBillingCountry
                Session("strDeliveryState") = strBillingState
                Session("strDeliveryPhoneNumber") = strBillingPhone
            Else
                Session("strDeliveryFirstname") = strDeliveryFirstName
                Session("strDeliverySurname") = strDeliverySurname
                Session("strDeliveryAddressLine1") = strDeliveryAddress1
                Session("strDeliveryAddressLine2") = strDeliveryAddress2
                Session("strDeliveryCity") = strDeliveryCity
                Session("strDeliveryPostCode") = strDeliveryPostCode
                Session("strDeliveryCountry") = strDeliveryCountry
                Session("strDeliveryState") = strDeliveryState
                Session("strDeliveryPhoneNumber") = strDeliveryPhone
            End If

            '** Now go to the order confirmation page **
            Response.Clear()
            Response.Redirect("orderConfirmation.aspx")
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
