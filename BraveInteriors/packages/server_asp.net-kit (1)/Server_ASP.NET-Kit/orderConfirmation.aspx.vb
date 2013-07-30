Imports includes
Imports MySql.Data.MySqlClient
Imports System.Data
Partial Class orderConfirmation
    Inherits System.Web.UI.Page
    '**************************************************************************************************
    ' Description
    ' ===========
    '
    ' Displays a summary of the order items and customer details.  If the user clicks Proceed the
    ' process of registering with Server begins by redirecting to the placeOrder page.
    '**************************************************************************************************
    Public objConn As MySqlConnection = oMySQLConnection()
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '** Extract customer details from the session **
        Dim strCart As String = ""

        '** Check we have a cart in the session.  If not, go back to the buildOrder page to get one **
        strCart = Session("strCart")
        If String.IsNullOrEmpty(strCart) Then
            Response.Clear()
            Server.Transfer("buildOrder.aspx")
            Response.End()
        End If

        If String.IsNullOrEmpty(Session("strBillingAddress1")) Then
            Response.Clear()
            Server.Transfer("customerDetails.aspx")
            Response.End()
        End If

        If strConnectTo = "SIMULATOR" Then
            pnlSimulator.Visible = True
        Else
            pnlNotSimulator.Visible = True
        End If

        Dim decTotal As Decimal = 0.0
        Dim decDelivery As Decimal = 1.5
        Dim strThisEntry As String = strCart
        Dim intQuantity As Integer
        Dim intProductID As Integer
        Dim oSQLReader As MySqlDataReader
        Dim ShpCartTable As DataTable
        Dim ShpCartRow As DataRow

        'create a shopping basket table for displaying the information
        ShpCartTable = New System.Data.DataTable("ShpCart")

        ShpCartTable.Columns.Add("Productid", GetType(Integer))
        ShpCartTable.Columns.Add("description", GetType(String))
        ShpCartTable.Columns.Add("price", GetType(Decimal))
        ShpCartTable.Columns.Add("quantity", GetType(Integer))
        ShpCartTable.Columns.Add("total", GetType(Decimal))

        While strThisEntry.Length > 0
            '** Extract the quantity and Product from the list of "x of y," entries in the cart **
            intQuantity = cleanInput(Left(strThisEntry, 1), "Number")
            intProductID = cleanInput(Mid(strThisEntry, 6, InStr(strThisEntry, ",") - 6), "Number")

            Dim strSQL As String = "select productid, description, price from tblproducts where ProductId=" & intProductID & ";"

            Dim oSQLCommand = New MySqlCommand(strSQL, objConn)
            oSQLReader = oSQLCommand.ExecuteReader()
            oSQLReader.Read()
            ShpCartRow = ShpCartTable.NewRow()

            ShpCartRow("Productid") = oSQLReader("productid")
            ShpCartRow("description") = oSQLReader("description")
            ShpCartRow("price") = oSQLReader("price")
            ShpCartRow("quantity") = intQuantity
            ShpCartRow("total") = oSQLReader("price") * intQuantity

            ShpCartTable.Rows.Add(ShpCartRow)

            '** Move to the next cart entry, if there is one **
            If InStr(strThisEntry, ",") = 0 Then
                strThisEntry = ""
            Else
                strThisEntry = Mid(strThisEntry, InStr(strThisEntry, ",") + 1)
            End If
            'increment the total
            decTotal = decTotal + oSQLReader("price") * intQuantity
            oSQLReader.Close()
            oSQLReader = Nothing
            oSQLCommand = Nothing
        End While
        dataRepBasket.DataSource = ShpCartTable
        dataRepBasket.DataBind()
        'set delivery and price and total price
        lblDelivery.Text = FormatNumber(decDelivery, 2, TriState.True)
        lblTotal.Text = FormatNumber(decTotal + decDelivery, 2, TriState.True)

    End Sub
    Protected Sub proceed_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles proceed.Click
        Response.Clear()
        Response.Redirect("transactionRegistration.aspx")
        Response.End()
    End Sub
    Protected Sub back_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles back.Click
        Response.Clear()
        Server.Transfer("customerDetails.aspx")
        Response.End()
    End Sub
    Protected Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Unload
        DestroyConnection(objConn)
    End Sub
End Class
