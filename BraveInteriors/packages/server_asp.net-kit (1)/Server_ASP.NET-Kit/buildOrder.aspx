<%@ Page Language="VB" AutoEventWireup="false" EnableViewStateMac="false" CodeFile="buildOrder.aspx.vb" Inherits="buildOrder" %>

<html>
<head runat="server">
    <title>Server ASP.Net Welcome Page</title>
    <link rel="STYLESHEET" type="text/css" href="images/serverKitStyle.css">
</head>
<body>
<div id="pageContainer">
<!-- #include file="header.html" -->
<!-- #include file="resourceBar.html" -->
<div id="content">
	<div id="contentHeader">Creating an Example Order</div>
		<p>This page demonstrates how to retrieve information about products from your database and create a very simple basket of goods. Use the form below to select the number of each DVD title you wish to buy, then hit Proceed. You have to select at lest 1 DVD to continue. </p>
		<p>The title, price and DVD ID (used to display the correct image) are extracted from the database to build the table. You can view this code in the buildOrder.aspx page.</p>
		<form action="buildOrder.aspx" method="POST" runat="server">
        <div class="greyHzShadeBar">&nbsp;</div>
        <asp:panel id="pnlError" runat="server" Visible="False" CssClass="errorheader">					
            <asp:Label Font-Bold=false ID='lblError' runat=server></asp:Label>
        </asp:panel>
		<table  class="formTable">	
            <tr>
              <td colspan="4"><div class="subheader">Please select the quantity of each item you wish to buy</div></td>
            </tr>
			<tr class="greybar">
				<td width="15%" align="center">Image</td>
				<td width="60%" align="left">Title</td>
				<td width="15%" align="right">Price</td>
				<td width="10%" align="center">Quantity</td>
			</tr> 
			<asp:Repeater ID="dataRepProduct" runat="server"> 
            <ItemTemplate>
				<tr>
					<td align="center"><img src="images/dvd<%# right("00"&DataBinder.Eval(Container, "DataItem.ProductId"), 2) %>.gif" alt="DVD box"></td>
					<td align="left"><%# DataBinder.Eval(Container, "DataItem.Description") %></td>
					<td align="right"><%#FormatNumber(DataBinder.Eval(Container, "DataItem.Price"), 2, TriState.True)%> <%#includes.strCurrency%></td>
					<td align="center"><select name="Quantity" size="1">
					<option value="0">None</option>
					  <%#QuantityOptions(DataBinder.Eval(Container, "DataItem.ProductId"))%>
					</select></td>
				</tr>
            </ItemTemplate>
            </asp:Repeater> 
		</table>
		<div class="greyHzShadeBar">&nbsp;</div>
		<div align="right">
		<table border="0" width="100%" class="formFooter">
			<tr>
				<td width="50%" align="left"><asp:ImageButton ID="back" ImageUrl="images/back.gif"  runat="server" /></td>
				<td width="50%" align="right"><asp:ImageButton ID="proceed" ImageUrl="images/proceed.gif"  runat="server" /></td>
			</tr>
		</table>
		</div>
		</form>
</div>
</div>
</body>
</html>
