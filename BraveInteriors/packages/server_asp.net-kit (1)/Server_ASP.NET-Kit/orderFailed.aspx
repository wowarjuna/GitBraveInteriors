<%@ Page Language="VB" AutoEventWireup="false" CodeFile="orderFailed.aspx.vb" Inherits="orderFailed" %>

<html>
<head id="Head1" runat="server">
	<title>Server ASP.Net Order Unsuccessful Page</title>
	<link rel="STYLESHEET" type="text/css" href="images/serverKitStyle.css">
    <script type="text/javascript" language="javascript" src="scripts/countrycodes.js"></script>
</head>

<body>
<div id="pageContainer">
<!-- #include file="header.html" -->
<!-- #include file="resourceBar.html" -->
<div id="content">
	<div id="contentHeader">Your order has NOT been successful</div>
	<p>Your transaction was not successful for the following reason:<br>
	  <br>
	  <span class="warning"><strong><asp:Label ID="lblFailureReason" runat="server" Text="Label"></asp:Label></strong></span>
	</p>
    <asp:panel id="pnlTransactionDetails" runat="server" Visible="False">
        <p>The order number, for your customer's reference is: <strong><asp:Label ID="lblVendorTxCodeReference" runat="server" Text="Label"></asp:Label></strong><br>
		    <br>
			They should quote this in all correspondence with you, and likewise you should use this reference when sending queries to Sage Pay about this transaction (along with your Vendor Name).<br>
			<br>
			The table below shows everything in the database about this order.  You would not normally show this level of detail to your customers, but it is useful during development.<br>
			<br>
			You can customise this page to offer alternative payment methods, links to customer support numbers, help and advice for online shopper, whatever is appropriate for your application.  The code is in orderFailed.aspx.
	    </p>
        <div class="greyHzShadeBar">&nbsp;</div>
	  	<asp:panel id="pnlTest" runat="server" Visible="False">
			<table class="formTable">
				<tr>
				  <td colspan="2"><div class="subheader">Order Details stored in your Database</div></td>
				</tr>
				<tr>
					<td class="fieldlabel">VendorTxCode:</td>
					<td class="fielddata"><asp:Label ID="lblVendorTxCode" runat="server" Text="Label"></asp:Label></td>
				</tr>
				<tr>
					<td class="fieldlabel">Transaction Type:</td>
					<td class="fielddata"><asp:Label ID="lblTransactionType" runat="server" Text="Label"></asp:Label></td>
				</tr>
				<tr>
					<td class="fieldlabel">Status:</td>
					<td class="fielddata"><asp:Label ID="lblStatus" runat="server" Text="Label"></asp:Label></td>
				</tr>
				<tr>
					<td class="fieldlabel">Amount:</td>
					<td class="fielddata"><asp:Label ID="lblAmount" runat="server" Text="Label"></asp:Label></td>
				</tr>
				<tr>
					<td class="fieldlabel">Billing Name:</td>
					<td class="fielddata"><asp:Label ID="lblBillingName" runat="server"></asp:Label></td>
				</tr>
				<tr>
					<td class="fieldlabel">Billing Address:</td>
					<td class="fielddata"><asp:Label ID="lblBillingAddress" runat="server"></asp:Label>
				        <script type="text/javascript" language="javascript">
				            document.write( getCountryName( "<asp:Literal ID="litBillingCountry" runat="server"></asp:Literal>" ));
				        </script>
					</td>
				</tr>
				<tr>
					<td class="fieldlabel">Billing Phone:</td>
					<td class="fielddata"><asp:Label ID="lblBillingPhone" runat="server"></asp:Label>&nbsp;</td>
				</tr>
				<tr>
					<td class="fieldlabel">Billing e-Mail:</td>
					<td class="fielddata" ><asp:Label ID="lblBillingEmail" runat="server"></asp:Label>&nbsp;</td>
				</tr>
				<tr>
					<td class="fieldlabel">Delivery Name:</td>
					<td class="fielddata"><asp:Label ID="lblDeliveryName" runat="server"></asp:Label></td>
				</tr>
				<tr>
					<td class="fieldlabel">Delivery Address:</td>
					<td class="fielddata"><asp:Label ID="lblDeliveryAddress" runat="server"></asp:Label>
				        <script type="text/javascript" language="javascript">
				            document.write( getCountryName( "<asp:Literal ID="litDeliveryCountry" runat="server"></asp:Literal>" ));
				        </script>
					</td>
				</tr>
				<tr>
					<td class="fieldlabel">Delivery Phone:</td>
					<td class="fielddata"><asp:Label ID="lblDeliveryPhone" runat="server"></asp:Label>&nbsp;</td>
				</tr>
				<tr>
					<td class="fieldlabel">VPSTxId:</td>
					<td class="fielddata"><asp:Label ID="lblVPSTxId" runat="server" Text="Label"></asp:Label></td>
				</tr>
				<tr>
					<td class="fieldlabel">SecurityKey:</td>
					<td class="fielddata"><asp:Label ID="lblSecurityKey" runat="server" Text="Label"></asp:Label></td>
				</tr>
				<tr>
					<td class="fieldlabel">VPSAuthCode (TxAuthNo):</td>
					<td class="fielddata"><asp:Label ID="lblVPSAuthCode" runat="server" Text="Label"></asp:Label></td>
				</tr>
				<tr>
					<td class="fieldlabel">AVSCV2 Results:</td>
					<td class="fielddata"><asp:Label ID="lblAVSCV2" runat="server" Text="Label"></asp:Label></td>
				</tr>
				<tr>
					<td class="fieldlabel">Gift Aid Transaction?:</td>
					<td class="fielddata"><asp:Label ID="lblGiftAid" runat="server" Text="Label"></asp:Label></td>
				</tr>
				<tr>
					<td class="fieldlabel">3D-Secure Status:</td>
					<td class="fielddata"><asp:Label ID="lbl3DSecure" runat="server" Text="Label"></asp:Label></td>
				</tr>
				<tr>
					<td class="fieldlabel">CAVV:</td>
					<td class="fielddata"><asp:Label ID="lblCAVV" runat="server" Text="Label"></asp:Label></td>
				</tr>
				<tr>
					<td class="fieldlabel">Card Type:</td>
					<td class="fielddata"><asp:Label ID="lblCardType" runat="server" Text="Label"></asp:Label></td>
				</tr>
				<tr>
					<td class="fieldlabel">Last 4 Digits:</td>
					<td class="fielddata"><asp:Label ID="lblLast4Digits" runat="server" Text="Label"></asp:Label></td>
				</tr>
				<tr>
					<td class="fieldlabel">Address Status:</td>
					<td class="fielddata"><span style="float:right; font-size: smaller;">&nbsp;*PayPal transactions only</span><asp:Label ID="lblAddressStatus" runat="server" Text="Label"></asp:Label></td>
				</tr>
				<tr>
					<td class="fieldlabel">Payer Status:</td>
					<td class="fielddata"><span style="float:right; font-size: smaller;">&nbsp;*PayPal transactions only</span><asp:Label ID="lblPayerStatus" runat="server" Text="Label"></asp:Label></td>
				</tr>
				<tr>
					<td class="fieldlabel">Basket Contents:</td>
					<td class="fielddata">
						<table width="100%" style="border-collapse;">
							<tr class="greybar">
								<td width="10%" align="right">Quantity</td>
								<td width="15%" align="center">Image</td>
								<td width="50%" align="left">Description</td>
								<td width="25%" align="right">Price</td>
							</tr>
                            <asp:Repeater ID="dataRepBasket" runat="server"> 
			                <ItemTemplate>
			                <tr>
			                    <td align="center"><%#DataBinder.Eval(Container, "DataItem.Quantity")%></td>
			                    <td align="center"><img src="images/dvd<%# right("00"&DataBinder.Eval(Container, "DataItem.ProductId"), 2) %>.gif" alt="DVD box"></td>
				                <td align="left"><%# DataBinder.Eval(Container, "DataItem.Description") %></td>
				                <td align="right"><%#FormatNumber(DataBinder.Eval(Container, "DataItem.Price"), 2, TriState.True)%> <%# includes.strCurrency %></td>
			                </tr>
			                </ItemTemplate>
			                </asp:Repeater>
						</table>
					</td>
				</tr>
			</table>
		</asp:panel>
		</asp:panel>
		<form id="Form1" name="completionform" action="orderSuccessful.aspx" method="POST" runat="server">
		<div class="greyHzShadeBar">&nbsp;</div>
		<table border="0" width="100%" class="formFooter">
			<tr>
				<td colspan="2">Click Proceed to go back to the Home Page to start another transaction, or click Admin to go to the Order Administration example pages where you can view your orders and perform REPEAT payments, REFUNDs, VOIDs and other 
				transaction processing functions. </td>
			</tr>
			<tr>
			    <td width="50%" align="left"><asp:ImageButton ID="admin" ImageUrl="images/admin.gif"  runat="server" /></td>
				<td width="50%" align="right"><asp:ImageButton ID="proceed" ImageUrl="images/proceed.gif"  runat="server" /></td>
			</tr>
		</table>
		</form>
</div>
</div>
</body>
</html>