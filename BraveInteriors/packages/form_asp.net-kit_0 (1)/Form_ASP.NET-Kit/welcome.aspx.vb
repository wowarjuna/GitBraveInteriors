Imports includes
Imports System.Data
Partial Class welcome
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        '**Display the correct panel according to includes configuration
        If includes.strVendorName = "[your Sage Pay Vendor Name]" Or includes.strYourSiteFQDN = "http://[your web site]/" Then
            pnlUnconfigured.Visible = True
        Else
            pnlConfigured.Visible = True
        End If

        If includes.strConnectTo = "LIVE" Then
            pnlLive.Visible = True
        ElseIf includes.strConnectTo = "TEST" Then
            pnlTest.Visible = True
        Else
            pnlSimulator.Visible = True
        End If

        lblVendorName.Text = strVendorName
        lblCurrency.Text = strCurrency
        lblFullInternalURL.Text = strYourSiteFQDN & strVirtualDir
        lblLocalUrl.Text = "http://" & Request.ServerVariables("SERVER_NAME") & Request.ServerVariables("SCRIPT_NAME")


    End Sub

    Protected Sub proceed_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles proceed.Click

        Response.Clear()
        Server.Transfer("buildOrder.aspx")
        Response.End()
    End Sub

End Class
