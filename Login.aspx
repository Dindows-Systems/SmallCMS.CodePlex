<%@ Page Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Website.Login" %>

<asp:Content ID="C" ContentPlaceHolderID="CPH1" runat="server">

	<CMS:HtmlEditor runat="server" ContentKey="maintext" EditorHeight="200" Style="width:auto;height:auto" />

	<asp:LoginView runat="server">
		<AnonymousTemplate>
			<div style="float:left">
				<asp:Login runat="server" DestinationPageUrl="/" FailureAction="RedirectToLoginPage" />
			</div>
			<div style="float:left">
				<a href="/Register.aspx">Sign Up</a>
			</div>
		</AnonymousTemplate>
		<LoggedInTemplate>
			<asp:LoginName runat="server" />
			<asp:LoginStatus runat="server" LogoutAction="Redirect" LogoutPageUrl="/" />
		</LoggedInTemplate>
	</asp:LoginView>

</asp:Content>

