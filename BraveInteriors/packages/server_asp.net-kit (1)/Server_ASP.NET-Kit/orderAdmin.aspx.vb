Imports includes
Imports MySql.Data.MySqlClient
Imports System.Data
Partial Class orderAdmin
    Inherits System.Web.UI.Page
    '**************************************************************************************************
    ' Server ASP Kit Order Administration Menu
    '**************************************************************************************************
    ' Description
    ' ===========
    '
    ' The Order Administration Menu page.  This page provides a link to the back-office features such
    ' as REFUND and REPEAT that you may wish to carry out from your own server (although they can be
    ' performed via the Admin system on the Sage Pay servers if you prefer)
    ' 
    ' IMPORTANT! Although these pages are provided as part of the kit, they should NOT be hosted in
    ' the same virtual directory as the order pages.  These perform back office functions that your
    ' customer should NOT have access too.  You you put these pages in a secure area only accessible
    ' to your staff.
    '**************************************************************************************************
    Public objConn As MySqlConnection = oMySQLConnection()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim oSQLReader As MySqlDataReader
        Dim TxListTable As DataTable
        Dim TxListRow As DataRow
        Dim strTxType As String = ""
        Dim strStatus As String = ""
        Dim strTxState As String = ""
        Dim straction As String = ""

        '** Check for the action button clicks **
        If (Request.Form("back.x") > 0) Then
            Session("Redirected") = True
            Session("strVendorTxCode") = cleanInput(Request.Form("VendorTxCode"), "VendorTxCode")
            Response.Clear()
            Server.Transfer("welcome.aspx")
            Response.End()
        ElseIf (Request.Form("release.x") > 0) Then
            Session("Redirected") = True
            Session("strVendorTxCode") = cleanInput(Request.Form("VendorTxCode"), "VendorTxCode")
            Response.Clear()
            Server.Transfer("release.aspx")
            Response.End()
        ElseIf (Request.Form("abort.x") > 0) Then
            Session("Redirected") = True
            Session("strVendorTxCode") = cleanInput(Request.Form("VendorTxCode"), "VendorTxCode")
            Response.Clear()
            Server.Transfer("abort.aspx")
            Response.End()
        ElseIf (Request.Form("refund.x") > 0) Then
            Session("Redirected") = True
            Session("strVendorTxCode") = cleanInput(Request.Form("VendorTxCode"), "VendorTxCode")
            Response.Clear()
            Response.Redirect("refund.aspx")
            Response.End()
        ElseIf (Request.Form("repeat.x") > 0) Then
            Session("Redirected") = True
            Session("strVendorTxCode") = cleanInput(Request.Form("VendorTxCode"), "VendorTxCode")
            Response.Clear()
            Server.Transfer("repeat.aspx")
            Response.End()
        ElseIf (Request.Form("void.x") > 0) Then
            Session("Redirected") = True
            Session("strVendorTxCode") = cleanInput(Request.Form("VendorTxCode"), "VendorTxCode")
            Response.Clear()
            Server.Transfer("void.aspx")
            Response.End()
        ElseIf (Request.Form("authorise.x") > 0) Then
            Session("Redirected") = True
            Session("strVendorTxCode") = cleanInput(Request.Form("VendorTxCode"), "VendorTxCode")
            Response.Clear()
            Server.Transfer("authorise.aspx")
            Response.End()
        ElseIf (Request.Form("cancel.x") > 0) Then
            Session("Redirected") = True
            Session("strVendorTxCode") = cleanInput(Request.Form("VendorTxCode"), "VendorTxCode")
            Response.Clear()
            Server.Transfer("cancel.aspx")
            Response.End()
        End If

        TxListTable = New System.Data.DataTable("TxList")

        TxListTable.Columns.Add("vendorTxCode", GetType(String))
        TxListTable.Columns.Add("TxType", GetType(String))
        TxListTable.Columns.Add("Amount", GetType(Decimal))
        TxListTable.Columns.Add("Date", GetType(String))
        TxListTable.Columns.Add("Status", GetType(String))
        TxListTable.Columns.Add("Actions", GetType(String))

        Dim strSQL As String = "SELECT * FROM tblOrders order by lastupdated desc;"

        Dim oSQLCommand = New MySqlCommand(strSQL, objConn)
        oSQLReader = oSQLCommand.ExecuteReader()


        '**loop through the record set.
        While oSQLReader.Read()
            straction = ""
            strTxType = oSQLReader("TxType")
            strStatus = nullToStr(oSQLReader("Status"))

            If Len(strStatus) > 0 Then
                strTxState = Left(strStatus, InStr(strStatus, " ") - 1)
            Else
                strTxState = "UNKNOWN"
            End If

            '** Display REFUND, REPEAT and VOID for Authorised PAYMENTS, AUTHORISES, REPEAT and Released DEFERREDs **
            If ((strTxType = "PAYMENT" Or strTxType = "AUTHORISE" Or strTxType = "REPEAT") And strTxState = "AUTHORISED") Or (strTxType = "DEFERRED" And strTxState = "RELEASED") Then
                straction = straction & "<input type=""image"" name=""refund"" src=""images/refund.gif"" value=""Refund this transaction""> "
                straction = straction & "<input type=""image"" name=""repeat"" src=""images/repeat.gif"" value=""Repeat this transaction""> "
                straction = straction & "<input type=""image"" name=""void"" src=""images/void.gif"" value=""Void this transaction""> "
            End If

            '** Display RELEASE and ABORT for any authorised DEFERRED transaction **
            If (strTxType = "DEFERRED" And strTxState = "AUTHORISED") Then
                straction = straction & "<input type=""image"" name=""release"" src=""images/release.gif"" value=""Release this transaction""> "
                straction = straction & "<input type=""image"" name=""abort"" src=""images/abort.gif"" value=""Abort this transaction""> "
            End If

            '** Display AUTHORISE and CANCEL for any AUTHENTICATED or REGISTERED transaction **
            If (strTxState = "AUTHENTICATED" Or strTxState = "REGISTERED") Then
                straction = straction & "<input type=""image"" name=""authorise"" src=""images/authorise.gif"" value=""Authorise this transaction""> "
                straction = straction & "<input type=""image"" name=""cancel"" src=""images/cancel.gif"" value=""Cancel this transaction""> "
            End If

            TxListRow = TxListTable.NewRow()

            TxListRow("vendorTxCode") = oSQLReader("vendorTxCode")
            TxListRow("TxType") = strTxType
            TxListRow("Amount") = oSQLReader("Amount")
            TxListRow("Date") = oSQLReader("LastUpdated").ToString
            TxListRow("Status") = strStatus
            TxListRow("Actions") = straction

            TxListTable.Rows.Add(TxListRow)

        End While
        oSQLReader.Close()
        oSQLReader = Nothing
        oSQLCommand = Nothing

        dataRepTransactionList.DataSource = TxListTable
        dataRepTransactionList.DataBind()
    End Sub
    Protected Sub back_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles back.Click
        Response.Clear()
        Response.Redirect("welcome.aspx")
        Response.End()
    End Sub

    Protected Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Unload
        DestroyConnection(objConn)
    End Sub
End Class
