<%@ Page Language="VB" ValidateRequest="false" AutoEventWireup="false" EnableViewStateMac="false" CodeFile="customerDetails.aspx.vb" Inherits="customerDetails" %>

<html>
<head>
	<title>Form ASP.Net Kit Customer Details Page</title>
	<link rel="STYLESHEET" type="text/css" href="images/formKitStyle.css" />
	<script type="text/javascript" language="javascript" src="scripts/countrycodes.js"></script>
    <script type="text/javascript" language="javascript" src="scripts/customerDetails.js"></script>
    <script type="text/javascript" language="javascript" src="scripts/statecodes.js"></script>
</head>
<body>
<div id="pageContainer">
<!-- #include file="header.html" -->
<!-- #include file="resourceBar.html" -->
<div id="content">
	<div id="contentHeader">Gathering Customer Details </div>
    <p>This page is a simple form to allow your customers to provide their name, address and contact details. The form makes some fields compulsory and the code of this page ensures the customer has completed these correctly. The code for this page can be found in customerDetails.aspx</p>
    <div class="greyHzShadeBar">&nbsp;</div>
    <asp:panel id="pnlError" runat="server" Visible="False" CssClass="errorheader">					
		<asp:Label Font-Bold=false ID='lblError' runat=server></asp:Label>
	</asp:panel>
	<form id="customerform" action="customerDetails.aspx" method="POST" runat="server">
	<table class="formTable">
		<!-- Billing Address Details -->
		<tr>
		  <td colspan="2"><div class="subheader">Please enter your Billing details below </div></td>
		</tr>
		<tr>
			<td class="fieldLabel"><span class="warning">*</span> First name(s):</td>
		    <td class="fieldData"><asp:TextBox ID="txtBillingFirstName" runat="server" Text="" maxlength="20" style="width: 200px;"/></td>
	    </tr>
	  	<tr>
	  	    <td class="fieldLabel"><span class="warning">*</span> Surname:</td>
	  	    <td class="fieldData"><asp:textbox ID="txtBillingSurname" runat="server" Text="" maxlength="20" style="width: 200px;"/></td>
	  	</tr>
		<tr>
			<td class="fieldLabel"><span class="warning">*</span> Address Line 1:</td>
		    <td class="fieldData"><asp:TextBox ID="txtBillingAddressLine1" maxlength="100" style="width: 400px;"  runat="server" Text="" /></td>
		</tr>
		<tr>
		    <td class="fieldLabel"> Address Line 2:</td>
		    <td class="fieldData"><asp:TextBox runat="server" ID="txtBillingAddressLine2" maxlength="100" style="width: 400px;" Text="" /></td>
		</tr>
		<tr>
			<td class="fieldLabel"><span class="warning">*</span> City:</td>
		    <td class="fieldData"><asp:TextBox ID="txtBillingCity" maxlength="40" style="width: 200px;" runat="server" Text="" /></td>
		</tr>
		<tr>
			<td class="fieldLabel"><span class="warning">*</span> Post/Zip Code:</td>
		    <td class="fieldData"><asp:TextBox ID="txtBillingPostCode" maxlength="10" style="width: 100px;" runat="server" Text="" /></td>
		</tr>
		<tr>
			<td class="fieldLabel"><span class="warning">*</span> Country:</td>
		    <td class="fieldData">
		     <select name="txtBillingCountry" style="width: 200px;">
		            <script type="text/javascript" language="javascript">
		                document.write( getCountryOptionsListHtml( '<% If Not Page.IsPostBack Then Response.Write(Server.HtmlEncode(Session("strBillingCountry"))) Else Response.Write(Server.HtmlEncode(Request.Form("txtBillingCountry"))) %>' ) );
		            </script>
		        </select>
		    </td>
		</tr>
		<tr>
			<td class="fieldlabel">State Code (U.S. only):</td>
		    <td class="fielddata">
		        <select name="txtBillingState" style="width: 200px;">
		            <script type="text/javascript" language="javascript">
		                document.write( getUsStateOptionsListHtml( '<% If Not Page.IsPostBack Then Response.Write(Server.HtmlEncode(Session("strBillingState"))) Else Response.Write(Server.HtmlEncode(Request.Form("txtBillingState"))) %>' ) );
		            </script>
		        </select> 
		        &nbsp;(<span class="warning">*</span> for U.S. customers only)
		    </td>
        </tr>
		<tr>
			<td class="fieldLabel"> Phone:</td>
		    <td class="fieldData"><asp:TextBox ID="txtBillingPhone"  maxlength="20" style="width: 200px;" runat="server" Text="" /></td>
		</tr>
		<tr>
			<td class="fieldLabel"> Email:</td>
		    <td class="fieldData"><asp:TextBox ID="txtBillingEmail"  maxlength="255" style="width: 200px;" runat="server" Text="" /></td>
		</tr>
		
		<!-- Delivery Address Details -->
		<tr>
		  <td colspan="2"><div class="subheader">Please enter your Delivery details below </div></td>
		</tr>
		<tr>
			<td class="fieldLabel"> Deliver to Billing Address?:</td>
		    <td class="fieldData"><input type="CheckBox" name="IsDeliverySame" id="IsDeliverySame" value="YES" <%If (Not Page.IsPostBack AND Session("bIsDeliverySame") = true) OR (Page.IsPostBack AND Request.Form("IsDeliverySame") = "YES") Then Response.Write("checked") %> onclick="IsDeliverySame_clicked();" /></td>
		</tr>
		<tr>
		    <td class="fieldLabel"><span class="warning">*</span> First Name(s):</td>
		    <td class="fieldData"><asp:TextBox ID="txtDeliveryFirstname" Text="" maxlength="20" style="width: 200px;" runat="server" /></td>
		</tr>
		<tr>
			<td class="fieldLabel"><span class="warning">*</span> Surname:</td>
		    <td class="fieldData"><asp:TextBox ID="txtDeliverySurname" Text="" maxlength="20" style="width: 200px;"  runat="server" /></td>
		</tr>
		<tr>
			<td class="fieldLabel"><span class="warning">*</span> Address Line 1:</td>
		    <td class="fieldData"><asp:TextBox ID="txtDeliveryAddressLine1" maxlength="100" style="width: 400px;"  runat="server" /></td>
		</tr>
		<tr>
			<td class="fieldLabel"> Address Line 2:</td>
		    <td class="fieldData"><asp:TextBox ID="txtDeliveryAddressLine2" maxlength="100" style="width: 400px;" runat="server" /></td>
		</tr>	
		<tr>
			<td class="fieldLabel"><span class="warning">*</span> City:</td>
		    <td class="fieldData"><asp:TextBox ID="txtDeliveryCity" maxlength="40" style="width: 200px;" runat="server" /></td>
		</tr>
		<tr>
			<td class="fieldLabel"><span class="warning">*</span> Post/Zip Code:</td>
		    <td class="fieldData"><asp:TextBox ID="txtDeliveryPostCode" maxlength="10" style="width: 100px;" runat="server" /></td>
		</tr>	
        <tr>
			<td class="fieldLabel"><span class="warning">*</span> Country:</td>
		    <td class="fieldData">
		       <select name="txtDeliveryCountry" style="width: 200px;">
		            <script type="text/javascript" language="javascript">
		                document.write( getCountryOptionsListHtml( '<% If Not Page.IsPostBack Then Response.Write(Server.HtmlEncode(Session("strDeliveryCountry"))) Else Response.Write(Server.HtmlEncode(Request.Form("txtDeliveryCountry"))) %>' ) );
		            </script>
		        </select>
		    </td>
		</tr>
		<tr>
			<td class="fieldlabel">State Code (U.S. only):</td>
		    <td class="fielddata">
		        <select name="txtDeliveryState" style="width: 200px;">
		            <script type="text/javascript" language="javascript">
		                document.write( getUsStateOptionsListHtml( '<% If Not Page.IsPostBack Then Response.Write(Server.HtmlEncode(Session("strDeliveryState"))) Else Response.Write(Server.HtmlEncode(Request.Form("txtDeliveryState"))) %>' ) );
		            </script>
		        </select> 
		        &nbsp;(<span class="warning">*</span> for U.S. customers only)
		    </td>
        </tr>
	    <tr>
			<td class="fieldLabel"> Phone:</td>
		    <td class="fieldData"><asp:TextBox ID="txtDeliveryPhone"  maxlength="20" style="width: 200px;" runat="server" /></td>
		</tr>
	</table>
    <script type="text/javascript" language="javascript">
        IsDeliverySame_clicked(true);
    </script>	
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
