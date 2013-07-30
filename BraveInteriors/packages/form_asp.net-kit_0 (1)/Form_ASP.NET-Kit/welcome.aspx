<%@ Page Language="VB" ValidateRequest="false" AutoEventWireup="false" CodeFile="welcome.aspx.vb" Inherits="welcome" %>

<html>
<head runat="server">
    <title>Sage Pay Form ASP.Net Welcome Page</title>
    <link rel="STYLESHEET" type="text/css" href="images/formKitStyle.css">
</head>

<body>
<div id="pageContainer">
<!-- #include file="header.html" -->
<!-- #include file="resourceBar.html" -->
<div id="content">
	<div id="contentHeader">Welcome to the Sage Pay Form ASP.Net Kit </div>
	<p>If you are viewing this page in your browser at <asp:Label Font-Bold=true ID='lblLocalUrl' runat=server></asp:Label> then you have correctly set up your virtual directory.</p>
    <asp:panel id="pnlUnconfigured" runat="server" Visible="False">
		<p>You still need to configure the kit for your site, however, by modifying the includes.asp file to set your database connections, Vendor Name and full URL to your site. If you've not already got it open, click the readme icon to the left to  find out how to customise the kit for your server.</p>
    </asp:panel>
    <div class="greyHzShadeBar">&nbsp;</div>
    <asp:panel id="pnlConfigured" runat="server" Visible="False">
			<table class="formTable">
				<tr>
				    <td colspan="2"><div class="subheader">Your current kit set-up </div></td></tr>
				<tr>
				    <td class="fieldLabel">Vendor Name:</td>
				    <td><asp:Label Font-Bold=false ID='lblVendorName' runat=server></asp:Label></td></tr>
				<tr>
				    <td class="fieldLabel">Default Currency:</td>
				    <td><asp:Label Font-Bold=false ID='lblCurrency' runat=server></asp:Label></td></tr>
				<tr><td class="fieldLabel">Full Internal URL to this kit:</td>
				    <td><asp:Label Font-Bold=false ID='lblFullInternalURL' runat=server></asp:Label></td></tr>
				<tr>
					<td colspan="2">
					</td>
				</tr>
			</table>
			<p>
              <asp:literal id="pnlLive" runat="server" Visible="False">
			  <span class="warning">Your kit is pointing at the Live Sage Pay environment.  You should only do this once your have completed testing on both the Simulator AND Test servers, have sent your GoLive request to the technical support team and had confirmation that your account has been set up. <br><br><strong>Transactions sent to the Live service WILL charge your customers' cards.</strong></span>
			  </asp:literal>
			  <asp:literal id="pnlTest" runat="server" Visible="False">
			  Your kit is pointing at the Sage Pay TEST environment.  This is an exact replica of the Live systems except that no banks are attached, so no authorisation requests are sent, nothing is settled and you can use our test card numbers when making payments. You should only use the test environment after you have completed testing using the Simulator AND the Sage Pay support team have mailed you to let you know your account has been created.<br><br><span class="warning"><strong>If you are already set up on Live and are testing additional functionality, DO NOT leave your kit set to Test or you will not receive any money for your transactions!</strong></span>
              </asp:literal>
              <asp:literal id="pnlSimulator" runat="server" Visible="False">
			  Your kit is currently pointing at the Simulator. This is an Expert System provided by Sage Pay to enable you to build and configure your site correctly, to debug the messages you send to Form and practise handling responses from it.  No customers are charged, no money is moved around.  The Simulator is for development and testing ONLY.
              </asp:literal>
			</p>
            <div class="greyHzShadeBar">&nbsp;</div>
			<div class="formFooter">
			    <form action="welcome.aspx" method="post" runat="server">
			    <asp:ImageButton ID="proceed" ImageUrl="images/proceed.gif"  runat="server" ImageAlign="Right" />
			    </form>
			</div>								
    </asp:panel>
</div>
</div>
</body>
</html>
