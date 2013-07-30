<%@ Page Language="VB" AutoEventWireup="false" CodeFile="release.aspx.vb" Inherits="release" %>

<html>
<head runat="server">
	<title>Server ASP.Net Kit Release Transaction Page</title>
	<link rel="STYLESHEET" type="text/css" href="images/serverKitStyle.css">

</head>

<body>
<div id="pageContainer">
<!-- #include file="header.html" -->
<!-- #include file="resourceBar.html" -->
<div id="content">
    <div id="contentHeader">Release Transaction Page</div>
    <asp:panel id="pnlPreRelease" runat="server" Visible="False">		
        <p> This page formats an Release POST to send to the Server, to cancel the DEFERRED transaction you selected in the Order Admin area.  The POST is displayed below.  If you wish to go ahead with the Release, check the ReleaseAmount and click Proceed, otherwise click Back to go Back to the admin area.<br><br>
		    The code for this page can be found in the Release.aspx file.
        </p>
    </asp:panel>
    <asp:panel id="pnlStatus" runat="server" Visible="False">
	    <p>The tables below show the results of the Release POST.  Click Back to return to the Order Admin area.</p>
        <div class="<%response.write(getStyle())%>">
            Server returned a Status of <asp:Label ID="lblStatus" runat="server" Text="Label"></asp:Label><br />
            <span class="warning"><asp:Label ID="lblResults" runat="server" Text="Label"></asp:Label></span>
        </div>
	</asp:panel>					  						
	<div class="greyHzShadeBar">&nbsp;</div>					
	<asp:panel id="pnlResponse" runat="server" Visible="False">
		<table class="formTable">
			<tr>
				<td ><div class="subheader">POST Sent to Server</div></td>
			</tr>
			<tr>
			  <td style="word-wrap:break-word; word-break: break-all;" class="code"><asp:Label ID="lblPost" runat="server" Text="Label"></asp:Label></td>
			</tr>
			<tr>
			  <td ><div class="subheader">Raw Response from Server</div></td>
			</tr>
			<tr>
			  <td style="word-wrap:break-word; word-break: break-all;" class="code"><asp:Label ID="lblResponse" runat="server" Text="Label"></asp:Label></td>
			</tr>
		</table>
		<form name="adminform" action="release.aspx" method="POST">
		<table class="formTable">
			<tr>
				<td width="100%" align="left"><input type="image" name="back" src="images/back.gif" value="Go back to the Order Administration screen"></td>
			</tr>
		</table>
		</form>				
		</asp:panel>
		<asp:panel id="pnlReleaseDetails" runat="server" Visible="False">
		<form name="adminform" action="release.aspx" method="POST" runat=server>
		<table class="formTable">
			<tr>
				<td colspan="2"><div class="subheader">RELEASE the following DEFERRED transaction</div></td>
			</tr>
			<tr>
				<td colspan="2"><p>You've chosen to RELEASE the DEFERRED transaction shown
				below. You must specify a ReleaseAmount. This can be anything up to and including the full value of the
				original DEFERRED transaction (the default value in the box below). You can only RELEASE once, not
				multiple times, so DEFERRED cannot be used for part shipments (use AUTHENTICATE and AUTHORISE
				if you need that functionality).</p></td>
			</tr>
			<tr>
				<td class="fieldlabel">VendorTxCode:</td>
				<td class="fielddata"><asp:Label ID="lblvendorTxCode" runat="server" Text="Label"></asp:Label></td>
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
				<td class="fieldlabel">TxAuthNo:</td>
				<td class="fielddata"><asp:Label ID="lblTxAuthNo" runat="server" Text="Label"></asp:Label></td>
			</tr>
			<tr>
				<td class="fieldlabel">Release Amount:</td>
				<td class="fielddata"><asp:TextBox ID="ReleaseAmount" runat="server"></asp:TextBox></td>
			</tr>
		</table>
		<div class="greyHzShadeBar">&nbsp;</div>
		<table border="0" width="100%" class="formFooter">
			<tr>
				<td width="50%" align="left"><asp:ImageButton ID="back" ImageUrl="images/back.gif"  runat="server" /></td>
				<td width="50%" align="right"><asp:ImageButton ID="proceed" ImageUrl="images/proceed.gif"  runat="server" /></td>
			</tr>
		</table>
		</form>		
	</asp:panel>
</div>
</div>
</body>
</html>