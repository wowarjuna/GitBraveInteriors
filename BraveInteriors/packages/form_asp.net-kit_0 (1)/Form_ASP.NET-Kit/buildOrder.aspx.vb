Imports System.Data
Imports includes
Partial Class buildOrder
    Inherits System.Web.UI.Page
    '**************************************************************************************************
    ' Description
    ' ===========
    '
    ' Retrieves product information from the database, displays details of the products and allows the
    ' user to enter a number of each item to buy.  It then validates the selection and forwards the
    ' user to the customer details page.
    '**************************************************************************************************
    Public strCart As String = ""

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim strCart As String = ""
        Dim TxBasketTable As DataTable
        Dim txbasketrow As DataRow
        Dim iIndex As Integer
        '** If we have a cart in the session, then we'll show the selected items here **
        strCart = Session("strCart")

        TxBasketTable = New System.Data.DataTable("TxList")
        TxBasketTable.Columns.Add("index", GetType(Integer))
        TxBasketTable.Columns.Add("image", GetType(String))
        TxBasketTable.Columns.Add("title", GetType(String))
        TxBasketTable.Columns.Add("price", GetType(Decimal))

        setCart()

        For iIndex = LBound(includes.arrProducts) + 1 To UBound(includes.arrProducts)
            txbasketrow = TxBasketTable.NewRow()
            txbasketrow("index") = iIndex
            txbasketrow("image") = Right("00" & iIndex, 2)
            txbasketrow("title") = includes.arrProducts(iIndex, 1)
            txbasketrow("price") = includes.arrProducts(iIndex, 2)
            TxBasketTable.Rows.Add(txbasketrow)
        Next

        dataRepProduct.DataSource = TxBasketTable
        dataRepProduct.DataBind()
    End Sub

    Function QuantityOptions(ByVal iIndex As Integer) As String
        Dim iLoop As Integer
        Dim strThisItem As String
        Dim strOptionList As String = ""

        For iLoop = 1 To 5
            strThisItem = iLoop & " of " & iIndex.ToString
            strOptionList = strOptionList & "<option value=""" & strThisItem & """"
            '** If this is in our cart, show it selected **
            If InStr(strCart, strThisItem) <> 0 Then
                strOptionList = strOptionList & " SELECTED"
            End If
            strOptionList = strOptionList & ">" & iLoop & "</option>"
        Next

        Return strOptionList

    End Function

    Protected Sub proceed_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles proceed.Click
        Dim strThisQuantity As String
        Dim iLoop As Integer
        Dim strPageError As String
        Dim strSplitQuantity As String
        Dim iRowCount As Integer
        Dim strRow() As String

        '** We need the user to have selected at least one item, so let's see what they've chosen **
        '** by looping through the submitted Quantity fields **
        strCart = ""
        strSplitQuantity = Request.Form("Quantity")
        iRowCount = strSplitQuantity.Split(",").Length
        strRow = strSplitQuantity.Split(",")
        For iLoop = 0 To iRowCount - 1
            strThisQuantity = strRow(iLoop)
            If strThisQuantity <> "0" Then
                '** Build a cart if any items have been selected **
                strCart = strCart & strThisQuantity & ","
            End If
        Next
        If strCart.Length = 0 Then
            '** Nothing was selected, so simply redesiplay the page with an error **
            strPageError = "You did not select any items to buy.  Please select at least 1 DVD."
            lblError.Text = strPageError
            pnlError.Visible = True
            Session("strCart") = ""
        Else
            '** Save the cart to the session object **
            Session("strCart") = strCart
            '** Proceed to the customer details acreen **
            Response.Clear()
            Response.Redirect("customerDetails.aspx")
            Response.End()
        End If
    End Sub

    Protected Sub back_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles back.Click
        Server.Transfer("welcome.aspx")
        Response.End()
    End Sub
End Class
