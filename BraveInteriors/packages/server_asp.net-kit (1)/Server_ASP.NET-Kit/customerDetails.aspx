<%@ Page Language="VB" AutoEventWireup="false" EnableViewStateMac="false"  CodeFile="customerDetails.aspx.vb" Inherits="customerDetails" %>

<html>
<head>
	<title>Server ASP.Net Kit Customer Details Page</title>
	<link rel="STYLESHEET" type="text/css" href="images/serverKitStyle.css">
    <script type="text/javascript" language="javascript" src="scripts/countrycodes.js"></script>
    <script type="text/javascript" language="javascript" src="scripts/customerDetails.js"></script>

</head>

<body>
<div id="pageContainer">
<!-- #include file="header.html" -->
<!-- #include file="resourceBar.html" -->
<div id="content">
	<div id="contentHeader">Gathering Customer Details </div>
		<p>This page is a simple form to allow your customers to provide their name, address and contact details. The form makes some fields compulsory and the code of this page ensures the customer has completed these correctly. The code for this page can be found in customerDetails.aspx</p>
        <div class="greyHzShadeBar">&nbsp;</div>
		<asp:panel id="pnlError" runat="server" Visible="False">					
		  <div class="errorheader"><asp:Label Font-Bold=false ID='lblError' runat=server></asp:Label></div>
	    </asp:panel>
		<form id="customerform" action="customerDetails.aspx" method="POST" runat="server">
		<table class="formTable">
			
			<tr>
			  <td colspan="2"><div class="subheader">Please enter your Billing details below</div> </td>
			</tr>
			<tr>
				<td class="fieldLabel"><span class="warning">*</span>First Name(s):</td>
			    <td class="fieldData"><asp:TextBox ID="txtBillingFirstnames" maxlength="20" style="width: 200px;" runat="server" Text=""></asp:TextBox></td>
            </tr>	
			<tr>
				<td class="fieldLabel"><span class="warning">*</span>Surname:</td>
			    <td class="fieldData"><asp:TextBox ID="txtBillingSurname" maxlength="20" style="width: 200px;" runat="server" Text=""></asp:TextBox></td>
            </tr>
			<tr>
				<td class="fieldLabel"><span class="warning">*</span>Address Line 1:</td>
			    <td class="fieldData"><asp:TextBox ID="txtBillingAddress1" maxlength="100" style="width: 400px;" runat="server" Text=""></asp:TextBox></td>
            </tr>	
			<tr>
				<td class="fieldLabel">Address Line 2:</td>
			    <td class="fieldData"><asp:TextBox ID="txtBillingAddress2" maxlength="100" style="width: 400px;" runat="server" Text=""></asp:TextBox></td>
            </tr>
			<tr>
				<td class="fieldLabel"><span class="warning">*</span>City:</td>
			    <td class="fieldData"><asp:TextBox ID="txtBillingCity" maxlength="40" style="width: 200px;" runat="server" Text=""></asp:TextBox></td>
            </tr>
			<tr>
				<td class="fieldLabel"><span class="warning">*</span>Post/Zip Code:</td>
			    <td class="fieldData"><asp:TextBox ID="txtBillingPostCode" maxlength="10" style="width: 80px;" runat="server" Text=""></asp:TextBox></td>
            </tr>
			<tr>
				<td class="fieldLabel"><span class="warning">*</span>Country:</td>
			    <td class="fieldData">
			        <select name="txtBillingCountry" style="width: 200px;" >
			            <script type="text/javascript" language="javascript">
			                document.write( getCountryOptionsListHtml( '<% If Not Page.IsPostBack Then Response.Write(Session("strBillingCountry")) Else Response.Write(Request.Form("txtBillingCountry")) %>' ) );
			            </script>
			        </select> 
			    </td>
            </tr>
			<tr>
				<td class="fieldLabel">State Code (U.S. only):</td>
			    <td class="fieldData"><asp:TextBox ID="txtBillingState" maxlength="2" style="width: 40px;" runat="server" Text=""></asp:TextBox> (<span class="warning">*</span>State Code for U.S. customers only)</td>
            </tr>	
			<tr>
				<td class="fieldLabel">Phone:</td>
			    <td class="fieldData"><asp:TextBox ID="txtBillingPhone" maxlength="20" style="width: 150px;" runat="server" Text=""></asp:TextBox></td>
            </tr>
			<tr>
				<td class="fieldLabel">e-Mail Address:</td>
			    <td class="fieldData"><asp:TextBox ID="txtCustomerEMail" maxlength="255" runat="server" Text=""></asp:TextBox></td>
			</tr>
			<tr>
			  <td colspan="2"><div class="subheader">Please enter your Delivery details below</div> </td>
			</tr>
			<tr>
				<td class="fieldLabel">Same as Billing Details?:</td>
			    <td class="fieldData"><input name="IsDeliverySame" type="checkbox" value="YES" <%If (Not Page.IsPostBack AND Session("bIsDeliverySame") = true) OR (Page.IsPostBack AND Request.Form("IsDeliverySame") = "YES") Then Response.Write("checked") %> onClick="IsDeliverySame_clicked();"></td>
			</tr>
			<tr>
				<td class="fieldLabel"><span class="warning">*</span>First Name(s):</td>
			    <td class="fieldData"><asp:TextBox ID="txtDeliveryFirstnames" maxlength="20" style="width: 200px;" runat="server" Text=""></asp:TextBox></td>
            </tr>	
			<tr>
				<td class="fieldLabel"><span class="warning">*</span>Surname:</td>
			    <td class="fieldData"><asp:TextBox ID="txtDeliverySurname" maxlength="20" style="width: 200px;" runat="server" Text=""></asp:TextBox></td>
            </tr>
			<tr>
				<td class="fieldLabel"><span class="warning">*</span>Address Line 1:</td>
			    <td class="fieldData"><asp:TextBox ID="txtDeliveryAddress1" maxlength="100" style="width: 400px;" runat="server" Text=""></asp:TextBox></td>
            </tr>	
			<tr>
				<td class="fieldLabel">Address Line 2:</td>
			    <td class="fieldData"><asp:TextBox ID="txtDeliveryAddress2" maxlength="100" style="width: 400px;" runat="server" Text=""></asp:TextBox></td>
            </tr>	
			<tr>
				<td class="fieldLabel"><span class="warning">*</span>City:</td>
			    <td class="fieldData"><asp:TextBox ID="txtDeliveryCity" maxlength="40" style="width: 200px;" runat="server" Text=""></asp:TextBox></td>
            </tr>
			<tr>
				<td class="fieldLabel"><span class="warning">*</span>Post/Zip Code:</td>
			    <td class="fieldData"><asp:TextBox ID="txtDeliveryPostCode" maxlength="10" style="width: 80px;" runat="server" Text=""></asp:TextBox></td>
            </tr>	
			<tr>
				<td class="fieldLabel"><span class="warning">*</span>Country:</td>
			    <td class="fieldData">
			        <select name="txtDeliveryCountry" style="width: 200px;" >
			            <script type="text/javascript" language="javascript">
			                document.write( getCountryOptionsListHtml( '<% If Not Page.IsPostBack Then Response.Write(Session("strDeliveryCountry")) Else Response.Write(Request.Form("txtDeliveryCountry")) %>' ) );
			            </script>
			        </select>
			    </td>
            </tr>
			<tr>
				<td class="fieldLabel">State Code (U.S. only):</td>
			    <td class="fieldData"><asp:TextBox ID="txtDeliveryState" maxlength="2" style="width: 40px;" runat="server" Text=""></asp:TextBox> (<span class="warning">*</span>State Code for U.S. customers only)</td>
            </tr>
			<tr>
				<td class="fieldLabel">Phone:</td>
			    <td class="fieldData"><asp:TextBox ID="txtDeliveryPhone" maxlength="20" style="width: 150px;" runat="server" Text=""></asp:TextBox></td>
            </tr>
		</table>
        <script type="text/javascript" language="javascript">
            IsDeliverySame_clicked();
        </script>	
		</table>
		<div class="greyHzShadeBar">&nbsp;</div>
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