<%@ Page Language="VB" AutoEventWireup="false" CodeFile="orderConfirmation.aspx.vb" Inherits="orderConfirmation" %>

<html>
<head>
	<title>Server ASP.Net Kit Order Confirmation Page</title>
	<link rel="STYLESHEET" type="text/css" href="images/serverKitStyle.css">
    <script type="text/javascript" language="javascript" src="scripts/countrycodes.js"></script>
</head>

<body>
<div id="pageContainer">
<!-- #include file="header.html" -->
<!-- #include file="resourceBar.html" -->
<div id="content">
	<div id="contentHeader">Order Confirmation Page  </div>
    <p>This page summarises the order details and customer information gathered on the previous screens. It is always a good idea to show your customers a page like this to allow them to go back and edit either basket or contact details.<br>
      <br>
      The page also shows how to calculate the order total using the basket contents and the database (you should never store order totals in the session or in hidden fields, since these can be modified by the user). The code for this page can be found in orderConfirmation.aspx.<BR><BR>
      <asp:panel id="pnlSimulator" runat="server" Visible="False">
     <p>Because you are using Simulator, clicking Proceed will show you the contents of the POST sent to Server, and the reply sent back.  This will illustrate how your system should register details with the real Sage Pay Server systems.  When you are in Test or Live modes, this page will not display results or wait for your input, it will simply redirect the customer to the payment pages, or handle any registration errors sent back by Server.</p> 
      </asp:panel>
      <asp:panel id="pnlNotSimulator" runat="server" Visible="False">
      <p>Since you are in mode, clicking Proceed will register your transaction with Server and automatically redirect you to the payment page, or handle any registration errors.  The code to do this can be found in transactionRegistration.aspx</p>
      </asp:panel>
    </p>
    <div class="greyHzShadeBar">&nbsp;</div>
	<table class="formTable">
		<tr>
		  <td colspan="5" ><div class="subheader">Your Basket Contents</div></td>
		</tr>
		<tr class="greybar">
			<td width="15%" align="center">Image</td>
			<td width="45%" align="left">Title</td>
			<td width="15%" align="right">Price</td>
			<td width="10%" align="right">Quantity</td>
			<td width="15%" align="right">Total</td>
		</tr>
        <asp:Repeater ID="dataRepBasket" runat="server"> 
		<ItemTemplate>
		<tr>
		    <td align="center"><img src="images/dvd<%# right("00"&DataBinder.Eval(Container, "DataItem.ProductId"), 2) %>.gif" alt="DVD box"></td>
			<td align="left"><%# DataBinder.Eval(Container, "DataItem.Description") %></td>
			<td align="right"><%#FormatNumber(DataBinder.Eval(Container, "DataItem.Price"),2,TriState.True)%> <%#includes.strCurrency%></td>
			<td align="center"><%#DataBinder.Eval(Container, "DataItem.Quantity")%></td>
			<td align="right"><%#FormatNumber(DataBinder.Eval(Container, "DataItem.Total"),2,TriState.True)%> <%#includes.strCurrency%></td>
		</tr>
		</ItemTemplate>
		</asp:Repeater>
		<tr>
			<td colspan="4" align="right">Delivery:</td>
			<td align="right"><asp:Label ID="lblDelivery" runat="server" Text="Label"></asp:Label> <%=includes.strCurrency%></td>
		</tr>
		<tr>
			<td colspan="4" align="right"><strong>Total:</strong></td>
			<td align="right"><strong><asp:Label ID="lblTotal" runat="server" Text="Label"></asp:Label> <%=includes.strCurrency%></strong></td>
		</tr>
    </table>
	<table class="formTable">
		<tr>
		  <td colspan="2"><div class="subheader">Your Billing Details</div></td>
		</tr>
		<tr>
			<td class="fieldLabel">Name:</td>
			<td class="fieldData"><%=Session("strBillingFirstnames")%>&nbsp;<%=Session("strBillingSurname")%></td>
		</tr>
		<tr>
			<td class="fieldLabel">Address Details:</td>
			<td class="fieldData">
			    <%=Session("strBillingAddress1")%><BR>
			    <%If Not String.IsNullOrEmpty(Session("strBillingAddress2")) Then Response.Write(Session("strBillingAddress2") & "<BR>")%>
			    <%=Session("strBillingCity")%>&nbsp;
			    <%If Not String.IsNullOrEmpty(Session("strBillingState")) Then Response.Write(Session("strBillingState"))%><BR>
			    <%=Session("strBillingPostCode")%><BR>
			    <script type="text/javascript" language="javascript">
			        document.write( getCountryName( "<%= Session("strBillingCountry") %>" ));
			    </script>
			</td>
		</tr>
		<tr>
			<td class="fieldLabel">Phone Number:</td>
			<td class="fieldData"><%=Session("strBillingPhone")%>&nbsp;</td>
		</tr>
		<tr>
			<td class="fieldLabel">e-Mail Address:</td>
			<td class="fieldData"><%=Session("strCustomerEMail")%>&nbsp;</td>
		</tr>
	</table>
	<table class="formTable">
		<tr>
		  <td colspan="2"><div class="subheader">Your Delivery Details</div></td>
		</tr>
		<tr>
			<td class="fieldLabel">Name:</td>
			<td class="fieldData"><%=Session("strDeliveryFirstnames")%>&nbsp;<%=Session("strDeliverySurname")%></td>
		</tr>
		<tr>
			<td class="fieldLabel">Address Details:</td>
			<td class="fieldData">
			    <%=Session("strDeliveryAddress1")%><BR>
			    <%If Not String.IsNullOrEmpty(Session("strDeliveryAddress2")) Then Response.Write(Session("strDeliveryAddress2") & "<BR>")%>
			    <%=Session("strDeliveryCity")%>&nbsp;
			    <%If Not String.IsNullOrEmpty(Session("strDeliveryState")) Then Response.Write(Session("strDeliveryState"))%><BR>
			    <%=Session("strDeliveryPostCode")%><BR>
			    <script type="text/javascript" language="javascript">
			        document.write( getCountryName( "<%=Session("strDeliveryCountry")%>" ));
			    </script>
			</td>
		</tr>
		<tr>
			<td class="fieldLabel">Phone Number:</td>
			<td class="fieldData"><%=Session("strDeliveryPhone")%>&nbsp;</td>
		</tr>
	</table>
    <div class="greyHzShadeBar">&nbsp;</div>
	<form name="customerform" action="orderConfirmation.aspx" method="POST" runat="server">
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