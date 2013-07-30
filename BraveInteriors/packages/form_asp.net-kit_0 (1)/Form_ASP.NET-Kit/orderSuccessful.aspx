<%@ Page Language="VB" ValidateRequest="false" AutoEventWireup="false" CodeFile="orderSuccessful.aspx.vb" Inherits="orderSuccessful" %>

<html>
<head runat="server">
	<title>Form ASP.Net Order Successful Page</title>
	<link rel="STYLESHEET" type="text/css" href="images/formKitStyle.css">
</head>

<body>
<div id="pageContainer">
<!-- #include file="header.html" -->
<!-- #include file="resourceBar.html" -->
<div id="content">
	<div id="contentHeader">Your order has been Successful</div>
    <p>
        The Form transaction has completed successfully and the customer has been returned to this order completion page<br>
        <br>
        The order number, for your customer's reference is: <strong><asp:Label ID="lblVendorTxCodeReference" runat="server" Text="Label"></asp:Label></strong><br>
        <br>
        They should quote this in all correspondence with you, and likewise you should use this reference when sending queries to Sage Pay about this transaction (along with your Vendor Name).<br>
        <br>
        The table below shows everything in the database about this order.  You would not normally show this level of detail to your customers, but it is useful during development.<br>
        <br>
        You can customise this page to send confirmation e-mails, display delivery times, present download pages, whatever is appropriate for your application.  The code is in orderSuccessful.aspx.
    </p>
    <div class="greyHzShadeBar">&nbsp;</div>
    <asp:Panel ID=pnlTest runat="server" Visible="false">
		<table class="formTable">
			<tr>
			  <td colspan="2"><div class="subheader">Details sent back by Form</div></td>
			</tr>
			<tr>
				<td class="fieldLabel">VendorTxCode:</td>
				<td class="fieldData"><asp:Label ID="lblVendorTxCode" runat="server" Text="Label"></asp:Label></td>
			</tr>
			<tr>
				<td class="fieldLabel">Status:</td>
				<td class="fieldData"><asp:Label ID="lblStatus" runat="server" Text="Label"></asp:Label></td>
			</tr>
			<tr>
				<td class="fieldLabel">Status Detail:</td>
				<td class="fieldData"><asp:Label ID="lblStatusDetail" runat="server" Text="Label"></asp:Label></td>
			</tr>
			<tr>
				<td class="fieldLabel">Amount:</td>
				<td class="fieldData"><asp:Label ID="lblAmount" runat="server" Text="Label"></asp:Label></td>
			</tr>
			<tr>
				<td class="fieldLabel">VPSTxId:</td>
				<td class="fieldData"><asp:Label ID="lblVPSTxId" runat="server" Text="Label"></asp:Label></td>
			</tr>
			<tr>
				<td class="fieldLabel">VPSAuthCode (TxAuthNo):</td>
				<td class="fieldData"><asp:Label ID="lblVPSAuthCode" runat="server" Text="Label"></asp:Label></td>
			</tr>
			<tr>
				<td class="fieldLabel">AVSCV2 Result:</td>
				<td class="fieldData"><asp:Label CssClass="smalltext" ID="lblAVSCV2Result" runat="server" Text="Label"></asp:Label></td>
			</tr>
			<tr>
				<td class="fieldLabel">Gift Aid Transaction?:</td>
				<td class="fieldData"><asp:Label ID="lblGiftAid" runat="server" Text="Label"></asp:Label></td>
			</tr>
			<tr>
				<td class="fieldLabel">3D-Secure Status:</td>
				<td class="fieldData"><asp:Label ID="lbl3DSecure" runat="server" Text="Label"></asp:Label></td>
			</tr>
			<tr>
				<td class="fieldLabel">CAVV:</td>
				<td class="fieldData"><asp:Label ID="lblCAVV" runat="server" Text="Label"></asp:Label></td>
			</tr>
			<tr>
				<td class="fieldLabel">CardType:</td>
				<td class="fieldData"><asp:Label ID="lblCardType" runat="server" Text="" /></td>
			</tr>
			<tr>
				<td class="fieldLabel">Last4Digits:</td>
				<td class="fieldData"><asp:Label ID="lblLast4Digits" runat="server" Text="" /></td>
			</tr>
			<tr>
				<td class="fieldLabel">AddressStatus:</td>
				<td class="fieldData"><span style="float:right; font-size: smaller;">&nbsp;*PayPal transactions only</span><asp:Label ID="lblPPAddressStatus" runat="server" text=""/></td>
			</tr>
			<tr>
				<td class="fieldLabel">PayerStatus:</td>
				<td class="fieldData"><span style="float:right; font-size: smaller;">&nbsp;*PayPal transactions only</span><asp:Label ID="lblPPPayerStatus" runat="server" text="" /></td>
			</tr>
		</table>
	</asp:panel>
	<div class="greyHzShadeBar">&nbsp;</div>
	<form name="completionform" method="POST" runat="server">
	<table border="0" width="100%" class="formFooter">
		<tr>
			<td colspan="2">Click Proceed to go back to the Home Page to start another transaction.</td>
		</tr>
		<tr>
		    <td width="50%" align="left">&nbsp;</td>
			<td width="50%" align="right"><asp:ImageButton ID="proceed" ImageUrl="images/proceed.gif"  runat="server" /></td>
		</tr>
	</table>
	</form>
</div>
</div>
</body>
</html>