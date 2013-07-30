<%@ Page Language="VB" ValidateRequest="false" AutoEventWireup="false" EnableViewStateMac="false" CodeFile="buildOrder.aspx.vb" Inherits="buildOrder" %>

<html>
<head runat="server">
    <title>Form ASP.Net Welcome Page</title>
    <link rel="STYLESHEET" type="text/css" href="images/formKitStyle.css">
</head>
<body>
<div id="pageContainer">
<!-- #include file="header.html" -->
<!-- #include file="resourceBar.html" -->
<div id="content">
	<div id="contentHeader">Creating an Example Order</div>
    <p>This page demonstrates how to retrieve information about products from your database and create a very simple basket of goods. Use the form below to select the number of each DVD title you wish to buy, then hit Proceed. You have to select at lest 1 DVD to continue. </p>
    <p>The title, price and DVD ID (used to display the correct image) are extracted from the database to build the table. You can view this code in the buildOrder.asp page.</p>
    <div class="greyHzShadeBar">&nbsp;</div>
    <asp:panel id="pnlError" runat="server" Visible="False" CssClass="errorheader">					
		<asp:Label Font-Bold=false ID='lblError' runat=server></asp:Label>
	</asp:panel>
    <form action="buildOrder.aspx" method="POST" runat="server">
	<table class="formTable">	
		<tr>
		  <td colspan="4"><div class="subheader">Please select the quantity of each item you wish to buy</div></td>
		</tr>
		<tr class="greybar">
			<td width="15%" align="center">Image</td>
			<td width="55%" align="left">Title</td>
			<td width="20%" align="right">Price</td>
			<td width="10%" align="center">Quantity</td>
		</tr>
		<asp:Repeater ID="dataRepProduct" runat="server"> 
        <ItemTemplate>
			<tr>
				<td align="center"><img src="images/dvd<%# DataBinder.Eval(Container, "DataItem.image") %>.gif" alt="DVD box"></td>
				<td align="left"><%#DataBinder.Eval(Container, "DataItem.title")%></td>
				<td align="right"><%#FormatNumber(DataBinder.Eval(Container, "DataItem.Price"),2,TriState.True)%> <%#includes.strCurrency%></td>
				<td align="center"><select name="Quantity" size="1">
				<option value="0">None</option>
				  <%#QuantityOptions(DataBinder.Eval(Container, "DataItem.index"))%>
				</select></td>
			</tr>
        </ItemTemplate>
        </asp:Repeater> 
	</table>
    <div class="greyHzShadeBar">&nbsp;</div>
	<table border="0" width="100%" class="formFooter">
		<tr>
			<td width="50%" align="left"><asp:ImageButton ID="back" ImageUrl="images/back.gif"  runat="server" /></td>
			<td width="50%" align="right"><asp:ImageButton ID="proceed" ImageUrl="images/proceed.gif"  runat="server" /></td>
		</tr>
	</table>
	</form>
</div>
</div>
</body>
</html>
