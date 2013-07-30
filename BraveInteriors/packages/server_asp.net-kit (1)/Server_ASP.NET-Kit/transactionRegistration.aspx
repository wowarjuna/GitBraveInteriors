<%@ Page Language="VB" AutoEventWireup="false" CodeFile="transactionRegistration.aspx.vb" Inherits="transactionRegistration" %>

<html>
<head>
	<title>Server ASP.Net Kit Transaction Registration Page</title>
	<link rel="STYLESHEET" type="text/css" href="images/serverKitStyle.css">
</head>
<body>
<div id="pageContainer">
<!-- #include file="header.html" -->
<!-- #include file="resourceBar.html" -->
<div id="content">
	<div id="contentHeader">Transaction Registration Page</div>
    <asp:panel id="pnlNoError" runat="server" Visible="False">
    <p>This page shows the contents of the POST sent to Server (based on your selections on the previous screens) 
    and the response sent back by the system. Because you are in SIMULATOR mode, you are seeing this information 
    and having to click Proceed to continue to the payment pages. In TEST and LIVE modes, the POST and redirect happen 
    invisibly, with no information sent to the browser and no user involvement.</p>
    </asp:panel>	
    <asp:panel id="pnlError" runat="server" Visible="False">			
    <p>A problem occurred whilst attempting to register this transaction with the Sage Pay systems. The details of 
    the error are shown below. This information is provided for your own debugging purposes and especially once 
    LIVE you should avoid displaying this level of detail to your customers. Instead you should modify the 
    transactionRegistration.aspx page to automatically handle these errors and redirect your customer 
    appropriately (e.g. to an error reporting page, or alternative customer services number to offline payment) </p>
    </asp:panel>		
    <asp:Label ID="lblRegistrationMessage" runat="server" Text=""></asp:Label>
    <div class="greyHzShadeBar">&nbsp;</div>
    <div class="<%response.write(getStyle())%>">
        Server returned a Status of <asp:Label ID="lblStatus" runat="server" Text="Label"></asp:Label><br />
        <asp:Label ID="lblError" runat="server" Text="" CssClass="warning"></asp:Label> 
    </div>
	<asp:panel id="pnlResult" runat="server" Visible="False">
	    <table class="formTable">
		    <tr>
		      <td colspan="2"><div class="subheader">Post Sent to Server</div></td>
		    </tr>
		    <tr>
		      <td align="left" colspan="2" style="word-wrap:break-word;word-break: break-all;" width="600" class="code"><asp:Label ID="lblPost" runat="server" Text="Label"></asp:Label></td>
		    </tr>
		    <tr>
		      <td colspan="2"><div class="subheader">Reply from Server</div></td>
		    </tr>
		    <tr>
		      <td align="left" colspan="2" style="word-wrap:break-word;word-break: break-all;" width="600" class="code"><asp:Label ID="lblReply" runat="server" Text="Label"></asp:Label></td>
		    </tr>
		    <asp:panel id="pnlTxResponse" runat="server" Visible="False">
		    <tr>
		        <td colspan="2"><div class="subheader">Order Details stored in your Database</div></td>
		    </tr>
		    <tr>
			    <td class="fieldlabel">VendorTxCode:</td>
			    <td class="fielddata"><asp:Label ID="lblVendorTxCode" runat="server" Text="Label"></asp:Label></td>
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
			    <td class="fieldlabel">Order Total:</td>
			    <td class="fielddata"><asp:Label ID="lblTotal" runat="server" Text="Label"></asp:Label></td>
		    </tr>
		    <tr>
			    <td class="fieldlabel" valign="top">Basket Contents:</td>
			    <td>
				    <table width="100%" style="border-collapse:collapse;">
					    <tr class="greybar">
						    <td width="10%" align="right">Quantity</td>
						    <td width="30%" align="center">Image</td>
						    <td width="60%" align="left">Title</td>
					    </tr>
                        <asp:Repeater ID="dataRepBasket" runat="server"> 
		                <ItemTemplate>
		                <tr>
		                    <td class="fieldlabel"><%# DataBinder.Eval(Container, "DataItem.Quantity") %></td>
			                <td class="fieldlabel"><img src="images/dvd<%# right("00"&DataBinder.Eval(Container, "DataItem.ProductId"), 2) %>.gif" alt="DVD box"></td>
			                <td class="fieldlabel"><%#DataBinder.Eval(Container, "DataItem.Description")%></td>
		                </tr>
		                </ItemTemplate>
		                </asp:Repeater>
				    </table>
				</td>
			</tr>
	        </asp:panel>
		</table>
	    <div class="greyHzShadeBar">&nbsp;</div>
	    <form id="Form1" name="customerform" action="transactionRegistration.aspx" runat=server method="POST">
        <asp:HiddenField ID="NextURL" runat="server" />
	    <table border="0" width="100%" class="formFooter">
		    <tr>
			    <td width="50%" align="left"><asp:ImageButton ID="Back" ImageUrl="images/back.gif"  runat="server" /></td>
			    <td width="50%" align="right"><asp:ImageButton ID="Proceed" ImageUrl="images/proceed.gif" visible=false runat="server" /></td>
		    </tr>
	    </table>
	    </form>
	</asp:panel>
</div>
</div>
</body>
</html>
