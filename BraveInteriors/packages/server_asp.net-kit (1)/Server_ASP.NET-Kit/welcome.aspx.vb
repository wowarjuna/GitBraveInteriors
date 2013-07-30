Imports includes
Imports System.Data
Imports MySql.Data.MySqlClient
Partial Class welcome
    Inherits System.Web.UI.Page
    Public objConn As MySqlConnection = oMySQLConnection()
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim strSQL As String

        '**Display the correct panel according to includes configuration
        If includes.strVendorName = "[your Sage Pay Vendor Name]" Or includes.strYourSiteFQDN = "http://[your web site]/" Or includes.strDatabasePassword = "[your database user password]" Then
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
        lblFullExternalURL.Text = strYourSiteFQDN & strVirtualDir
        lblFullInternalURL.Text = strYourSiteInternalFQDN & strVirtualDir
        lblDatabaseUser.Text = strDatabaseUser
        lblLocalUrl.Text = "http://" & Request.ServerVariables("SERVER_NAME") & Request.ServerVariables("SCRIPT_NAME")

        strSQL = "SELECT count(ProductID) as 'Total' FROM tblProducts;"
        Dim oSQLReader As MySqlDataReader
        Dim iTotal As String = ""

        Dim oSQLCommand = New MySqlCommand(strSQL, objConn)

        'return data to reader. 
        oSQLReader = oSQLCommand.ExecuteReader()

        'loop through record set
        While oSQLReader.Read()
            iTotal = oSQLReader("Total")
        End While
        lblProductsCount.Text = iTotal

        oSQLReader.Close()
        oSQLReader = Nothing
        oSQLCommand = Nothing

        Dim OrderNumber As Integer = 0
        strSQL = "SELECT COUNT(VendorTxCode) as 'OrderNumber' FROM tblOrders;"
        oSQLCommand = New MySqlCommand(strSQL, objConn)
        oSQLReader = oSQLCommand.ExecuteReader()
        oSQLReader.Read()

        'loop through record set
        If oSQLReader.HasRows Then
            OrderNumber = oSQLReader("OrderNumber")
        End If

        If OrderNumber > 0 Then
            pnlGoToAdmin.Visible = True
        End If

        'close reader and connection
        oSQLReader.Close()
        oSQLReader = Nothing
        oSQLCommand = Nothing


    End Sub
    Protected Sub proceed_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles proceed.Click
        Response.Clear()
        Server.Transfer("buildOrder.aspx")
        Response.End()
    End Sub
    Protected Sub admin_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles admin.Click
        Response.Clear()
        Server.Transfer("orderAdmin.aspx")
        Response.End()
    End Sub

    Protected Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Unload
        DestroyConnection(objConn)
    End Sub
End Class
