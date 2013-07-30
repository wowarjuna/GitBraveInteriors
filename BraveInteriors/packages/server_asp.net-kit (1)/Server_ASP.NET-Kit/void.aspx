<%@ Page Language="VB" AutoEventWireup="false" CodeFile="void.aspx.vb" Inherits="void" %>

<html>
<head runat="server">
	<title>Server ASP.Net Kit Void Transaction Page</title>
	<link rel="STYLESHEET" type="text/css" href="images/serverKitStyle.css">

</head>

<body>
<div id="pageContainer">
<!-- #include file="header.html" -->
<!-- #include file="resourceBar.html" -->
<div id="content">
	<div id="contentHeader">Void Transaction Page</div>
    <asp:panel id="pnlPreRelease" runat="server" Visible="False">
        <p>This page formats a VOID post to send to the Server, to cancel the transaction you selected in the Order Admin area.  The POST is displayed below.  If you wish to go ahead with the VOID, click Proceed, otherwise click Back to go back to the admin area.<br />
	    <br />
		The code for this page can be found in the void.aspx file.</p>
    </asp:panel>
    <asp:panel id="pnlStatus" runat="server" Visible="False">
	    <p>The tables below show the results of the VOID POST.  Click Back to return to the Order Admin area.</p>
        <div class="<%response.write(getStyle())%>">
            Server returned a Status of <asp:Label ID="lblStatus" runat="server" Text="Label"></asp:Label><br />
            <span class="warning"><asp:Label ID="lblResults" runat="server" Text="Label"></asp:Label></span>
        </div>
    </asp:panel>			
	<div class="greyHzShadeBar">&nbsp;</div>
	<table class="formTable">
		<tr>
			<td ><div class="subheader">POST Sent to Server</div></td>
		</tr>
		<tr>
		  <td style="word-wrap:break-word; word-break: break-all;" class="code"><asp:Label ID="lblPost" runat="server" Text="Label"></asp:Label></td>
		</tr>
		<asp:panel id="pnlResponse" runat="server" Visible="False">
		<tr>
		  <td ><div class="subheader">Raw Response from Server</div></td>
		</tr>
		<tr>
		  <td style="word-wrap:break-word; word-break: break-all;" class="code"><asp:Label ID="lblResponse" runat="server" Text="Label"></asp:Label></td>
		</tr>
		</asp:panel>
	</table>	
	<div class="greyHzShadeBar">&nbsp;</div>
	<asp:panel id="pnlReleaseDetails" runat="server" Visible="False">
		<form name="adminform" action="release.aspx" method="POST" runat=server>
		<table border="0" width="100%" class="formFooter">
			<tr>
				<td width="50%" align="left"><asp:ImageButton ID="back" ImageUrl="images/back.gif" runat="server" /></td>
				<td width="50%" align="right"><asp:ImageButton ID="proceed" ImageUrl="images/proceed.gif"  runat="server" /></td>
			</tr>
		</table>
		</form>
	</asp:panel>
</div>
</div>
</body>
</html>