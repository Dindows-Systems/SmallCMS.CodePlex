<%@ Page Title="" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CMS.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CPH1" runat="server">

<div style="float:left">
	<a href="Users.aspx">Users</a>
</div>

<div style="float:right">
	<asp:LoginView runat="server">
		<AnonymousTemplate>
			<div style="float:left">
				<asp:Login runat="server" />
			</div>
		</AnonymousTemplate>
		<LoggedInTemplate>
			<asp:LoginName runat="server" />
			<asp:LoginStatus runat="server" />
		</LoggedInTemplate>
	</asp:LoginView>
</div>

</asp:Content>
