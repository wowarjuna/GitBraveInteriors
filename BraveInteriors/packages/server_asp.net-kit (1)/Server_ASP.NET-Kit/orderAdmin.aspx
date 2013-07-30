<%@ Page Language="VB" AutoEventWireup="false" CodeFile="orderAdmin.aspx.vb" Inherits="orderAdmin" %>

<html>
<head runat="server">
	<title>Server ASP.Net Kit Order Administration Menu</title>
	<link rel="STYLESHEET" type="text/css" href="images/serverKitStyle.css">
</head>

<body>
<div id="pageContainer">
<!-- #include file="header.html" -->
<!-- #include file="resourceBar.html" -->
<div id="content">
	<div id="contentHeader">Order Administration Menu </div>
	<p><span class="warning">Although these pages are provided as part of the kit, they should NOT be hosted in the same virtual directory as the order pages.  These perform back office functions that your customers should NOT have access too.  You should put these pages in a secure area only accessible to your staff.</span><br>
	  <br>
	  The table belows lists all your transactions in reverse date order. Click the buttons in the action column of the table to perform the specified function.<br>
	  <br>
	  For a full list of back office functions see the <a href="http://www.sagepay.com/documents/SagePayServerandDirectSharedProtocols.pdf" target="_blank">Server and Direct Shared Protocols</a> document. <br>
	  <br>
	This code for this simple menu page is in the orderAdmin.aspx file. 
	</p>
	<div class="greyHzShadeBar">&nbsp;</div>
	<table class="formTable">
		<tr>
			<td colspan="6"><div class="subheader">Your transactions</div></td>
		</tr>
		<tr>
			<td class="greyBar" align="left" width="25%">VendorTxCode</td>
			<td class="greyBar" align="left" width="10%">TxType</td>
			<td class="greyBar" align="right" width="10%">Amount</td>
			<td class="greyBar" align="left" width="10%">Date</td>
			<td class="greyBar" align="left" width="35%">Status</td>
			<td class="greyBar" align="left" width="10%">Actions</td>
		</tr>
		<asp:Repeater ID="dataRepTransactionList" runat="server"> 
		<ItemTemplate>
		<form action="orderAdmin.aspx" method="POST">
		<tr>
		    <td class="smalltext" align="left"><input type="hidden" name="vendorTxCode" value="<%# DataBinder.Eval(Container, "DataItem.VendorTxCode") %>" /><%# DataBinder.Eval(Container, "DataItem.VendorTxCode") %></td>
			<td class="smalltext" align="left"><%# DataBinder.Eval(Container, "DataItem.TxType") %>&nbsp;</td>
			<td class="smalltext" align="right"><%#FormatCurrency(DataBinder.Eval(Container, "DataItem.Amount"))%>&nbsp;</td>
			<td class="smalltext" align="center"><%#DataBinder.Eval(Container, "DataItem.Date")%>&nbsp;</td>
			<td class="smalltext" align="left" style="word-wrap: break-word; word-break: break-all;" ><%#DataBinder.Eval(Container, "DataItem.Status")%>&nbsp;</td>
		    <td class="smalltext" align="center"><%#DataBinder.Eval(Container, "DataItem.Actions")%> </td>
		</tr>
		</form>
		</ItemTemplate>
		</asp:Repeater>
	</table>
	<div class="greyHzShadeBar">&nbsp;</div>
	<form action="orderAdmin.aspx" method="POST" runat=server>
	<table border="0" width="100%" class="formFooter">
		<tr>
			<td width="100%" align="left"><asp:ImageButton ID="back" ImageUrl="images/back.gif"  runat="server" /></td>
		</tr>
	</table>
	</form>
</div>
</div>
</body>
</html>