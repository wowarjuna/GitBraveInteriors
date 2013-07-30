<%@ Page Language="VB" ValidateRequest="false" AutoEventWireup="false" CodeFile="orderConfirmation.aspx.vb" Inherits="orderConfirmation" %>

<html>
<head>
	<title>Form ASP.Net Kit Order Confirmation Page</title>
	<link rel="STYLESHEET" type="text/css" href="images/formKitStyle.css">
	<script type="text/javascript" language="javascript" src="scripts/countrycodes.js"></script>
</head>

<body>
<div id="pageContainer">
<!-- #include file="header.html" -->
<!-- #include file="resourceBar.html" -->
<div id="content">
	<div id="contentHeader">Order Confirmation Page</div>
	<p>
	  This page summarises the order details and customer information gathered on the previous screens.
	  It is always a good idea to show your customers a page like this to allow them to go back and edit
	  either basket or contact details.<br>
	  <br>
	  This page also creates the Form crypt field and creates a form to POST this information to
	  the Sage Pay Gateway when the Proceed button is clicked. The code for this page can be found in
	  orderConfirmation.asp.<BR><BR>
      <asp:literal id="pnlSimulator" runat="server" Visible="False">
      Because you are in SIMULATOR mode, the unencrypted contents of the crypt field 
	  are also displayed below, allowing you to check the contents. When you are in Live mode, you will
	  only see the order confirmation boxes.
      </asp:literal>
      <asp:literal id="pnlTest" runat="server" Visible="False">
      Because you are in TEST mode, the unencrypted contents of the crypt field 
	  are also displayed below, allowing you to check the contents. When you are in Live mode, you will
	  only see the order confirmation boxes.
      </asp:literal>
      <asp:Label ID="lblMode" runat="server" Text="Label"></asp:Label>
      <asp:literal id="pnlNotSimulator" runat="server" Visible="False">
      Since you are in LIVE mode, clicking Proceed will register your transaction with Form 
	  and automatically redirect you to the payment page, or handle any registration errors.  
	  The code to do this can be found in transactionRegistration.aspx
      </asp:literal>
    </p>
    <div class="greyHzShadeBar">&nbsp;</div>
	<table class="formTable">
		<tr>
		  <td colspan="5"><div class="subheader">Your Basket Contents</div></td>
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
			<td align="left"><%#DataBinder.Eval(Container, "DataItem.Description")%></td>
			<td align="right"><%#DataBinder.Eval(Container, "DataItem.Price")%></td>
			<td align="center"><%#DataBinder.Eval(Container, "DataItem.Quantity")%></td>
			<td align="center"><%#DataBinder.Eval(Container, "DataItem.Total")%></td>
		</tr>
		</ItemTemplate>
		</asp:Repeater>
		<tr>
			<td colspan="4" align="right">Delivery:</td>
			<td align="right"><asp:Label ID="lblDelivery" runat="server" Text="Label"></asp:Label></td>
		</tr>
		<tr>
			<td colspan="4" align="right"><strong>Total:</strong></td>
			<td align="right"><strong><asp:Label ID="lblTotal" runat="server" Text="Label"></asp:Label></strong></td>
		</tr>
    </table>
	<table class="formTable">
		<tr>
		  <td colspan="5"><div class="subheader">Your Contact and Delivery Details</div></td>
		</tr>
		<tr>
			<td class="fieldLabel">e-Mail Address:</td>
			<td class="fieldData"><asp:Label ID="lblEmailAddress" runat="server" Text="Label"></asp:Label>&nbsp;</td>
		</tr>
		<tr>
		    <td colspan="5"><div class="subheader">Billing Details</div></td>
		</tr>
		<tr>
		    <td class="fieldLabel">Name:</td>
			<td class="fieldData"><asp:Label ID="lblBillingName" runat="server" Text="Label"></asp:Label></td>
		</tr>
		<tr>
			<td class="fieldLabel">Phone Number:</td>
			<td class="fieldData"><asp:Label ID="lblBillingPhoneNumber" runat="server" Text="Label"></asp:Label>&nbsp;</td>
		</tr>
		<tr>
			<td class="fieldLabel" valign="top">Billing Address:</td>
			<td class="fieldData"><asp:Label ID="lblBillingAddressPC" runat="server" Text="Label"></asp:Label></td>
		</tr>
		<tr>
		    <td colspan="5"><div class="subheader">Delivery Details</div></td>
		</tr>
		<tr>
		    <td class="fieldLabel">Delivery Name:</td>
			<td class="fieldData"><asp:Label ID="lblDeliveryName" runat="server" Text="Label"></asp:Label></td>
		</tr>
		<tr>
			<td class="fieldLabel">Phone Number:</td>
			<td class="fieldData"><asp:Label ID="lblDeliveryPhoneNumber" runat="server" Text="Label"></asp:Label>&nbsp;</td>
		</tr>
		<tr>
			<td class="fieldLabel" valign="top">Address:</td>
			<td class="fieldData"><asp:Label ID="lblDeliveryAddressPC" runat="server" Text="Label"></asp:Label></td>
		</tr>
	</table>
	<asp:Panel runat="server" ID="CryptContents" >
	<table class="formTable">
		<tr>
		  <td><div class="subheader">Your Form Crypt Post Contents</div></td>
		</tr>
		<tr>
		  <td >The box below shows the unencrypted contents of the Form
		  Crypt field.  This will not be displayed in LIVE mode.  If you wish to view the encrypted and encoded
		  contents view the source of this page and scroll to the bottom.  You'll find the submission FORM there.</td>
		</tr>
		<tr>
		  <td align="left" style="word-wrap:break-word; word-break: break-all;" width="600" class="code">
		  <asp:Label ID="lblPostString" runat="server" text=""/>
		  </td>
		</tr>
	</table>
	</asp:Panel>
    <div class="greyHzShadeBar">&nbsp;</div>
	<div class="formFooter">
	    <asp:Label ID="lblForm" runat="server" Text="Label" />
	</div>
</div>
</div>
</body>