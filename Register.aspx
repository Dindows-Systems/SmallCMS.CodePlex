<%@ Page Title="" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="SmallCMS.Register" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CPH1" runat="server">

<asp:LoginView runat="server">
	<AnonymousTemplate>
		<asp:CreateUserWizard runat="server" />
	</AnonymousTemplate>
	<LoggedInTemplate>
		You are logged in, so you cannot register.
	</LoggedInTemplate>
</asp:LoginView>

</asp:Content>
