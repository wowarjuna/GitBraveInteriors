<%@ Page Language="VB" AutoEventWireup="false" CodeFile="cancel.aspx.vb" Inherits="cancel" %>

<html>
<head runat="server">
	<title>Server ASP.Net Kit Cancel Transaction Page</title>
	<link rel="STYLESHEET" type="text/css" href="images/serverKitStyle.css">

</head>

<body>
<div id="pageContainer">
<!-- #include file="header.html" -->
<!-- #include file="resourceBar.html" -->
<div id="content">
	<div id="contentHeader">Cancel Transaction Page</div>
     <asp:panel id="pnlPreCancel" runat="server" Visible="False">
        <p>This page formats a cancel post to send to the Server, to cancel the transaction you selected in the Order Admin area.  The POST is displayed below.  If you wish to go ahead with the cancel, click Proceed, otherwise click Back to go back to the admin area.<br>
        The tables below show the results of the cancel.  Click Back to return to the Order Admin area.
        </p>
    </asp:panel>
    <asp:panel id="pnlStatus" runat="server" Visible="False">
        <p>The tables below show the results of the cancel POST.  Click Back to return to the Order Admin area.</p>
	</asp:panel>
    <asp:panel id="pnlStatus2" runat="server" Visible="False">
	<div class="greyHzShadeBar">&nbsp;</div>
		    <div class="<%response.write(getStyle())%>">
	        Server returned a Status of <asp:Label ID="lblStatus" runat="server" Text="Label"></asp:Label><br />
            <span class="warning"><asp:Label ID="lblResults" runat="server" Text="Label"></asp:Label></span>
        </div>
	</asp:panel>
  	<table class="formTable">
		<tr>
			<td colspan="2"><div class="subheader">POST Sent to Server</div></td>
		</tr>
		<tr>
		  <td colspan="2" style="word-wrap:break-word; word-break: break-all;" class="code"><asp:Label ID="lblPost" runat="server" Text="Label"></asp:Label></td>
		</tr>
		<asp:panel id="pnlResponse" runat="server" Visible="False">
		<tr>
		  <td colspan="2"><div class="subheader">Raw Response from  Server</div></td>
		</tr>
		<tr>
		  <td colspan="2" style="word-wrap:break-word; word-break: break-all;" class="code"><asp:Label ID="lblResponse" runat="server" Text="Label"></asp:Label></td>
		</tr>
		</asp:panel>
	</table>
	<div class="greyHzShadeBar">&nbsp;</div>
	<form name="adminform" action="cancel.aspx" method="POST" runat=server>
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