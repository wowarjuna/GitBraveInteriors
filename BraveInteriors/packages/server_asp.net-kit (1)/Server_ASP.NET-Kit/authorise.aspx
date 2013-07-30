<%@ Page Language="VB" AutoEventWireup="false" CodeFile="authorise.aspx.vb" Inherits="authorise" %>

<html>
<head runat="server">
	<title>Server ASP.Net Kit Authorise Transaction Page</title>
	<link rel="STYLESHEET" type="text/css" href="images/serverKitStyle.css">

</head>

<body>
<div id="pageContainer">
<!-- #include file="header.html" -->
<!-- #include file="resourceBar.html" -->
<div id="content">
	<div id="contentHeader">AUTHORISE Transaction Page</div>
    <asp:panel id="pnlPreAuthorise" runat="server" Visible="False">	
        <p>This page formats an AUTHORISE POST to send to the Server, to Authorise against the AUTHENTICATED transaction you selected in the Order Admin area.  The details are displayed below.  If you wish to go ahead with the Authorise, check the Authorise Amount and click Proceed, otherwise click Back to go Back to the admin area.<br />
            <br />
            The code for this page can be found in the Authorise.aspx file.
        </p>
    </asp:panel>
    <asp:panel id="pnlStatus" runat="server" Visible="False">
        <p>
        The tables below show the results of the Authorise POST.  Click Back to return to the Order Admin area.
        </p>
	</asp:panel>
    <div class="greyHzShadeBar">&nbsp;</div>
    <asp:panel id="pnlStatus2" runat="server" Visible="False">
	    <div class="<%response.write(getStyle())%>">
            Server returned a Status of <asp:Label ID="lblStatus" runat="server" Text="Label"></asp:Label><br />
            <span class="warning"><asp:Label ID="lblResults" runat="server" Text="Label"></asp:Label></span>
        </div>
	</asp:panel>
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
    <div class="greyHzShadeBar">&nbsp;</div>
    <form name="adminform" action="Authorise.aspx" method="POST">
    <table border="0" width="100%" class="formFooter">
	    <tr>
		    <td width="100%" align="left"><input type="image" name="back" src="images/back.gif" value="Go back to the Order Administration screen"></td>
	    </tr>
    </table>
    </form>				
    </asp:panel>

    <asp:panel id="pnlAuthoriseDetails" runat="server" Visible="False">
    <form name="adminform" action="Authorise.aspx" method="POST" runat=server>
    <table class="formTable">
	    <tr>
		    <td colspan="2"><div class="subheader">AUTHORISE the following AUTHENTICATED transaction</div></td>
	    </tr>
	    <tr>
		    <td colspan="2"><p>You've chosen to Authorise the transaction shown
		    below. You must specify the Authorise details in the boxes below. You can Authorise for any amount up
		    to and including 115% the amount of the original transaction (minus any other Authorises you've done against
		    this transaction).</p></td>
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
		    <td class="fieldlabel">Authorise VendorTxCode:</td>
		    <td class="fielddata"><asp:TextBox ID="AuthoriseVendorTxCode" runat="server" Width=250></asp:TextBox></td>
	    </tr>
	    <tr>
		    <td class="fieldlabel">Authorise Description:</td>
		    <td class="fielddata"><asp:TextBox ID="AuthoriseDescription" runat="server" Width=200></asp:TextBox></td>
	    </tr>
	    <tr>
		    <td class="fieldlabel">Authorise Amount:</td>
		    <td class="fielddata"><asp:TextBox ID="AuthoriseAmount" runat="server"></asp:TextBox></td>
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