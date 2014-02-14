<%@ Page Title="" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeBehind="ArticleTwoColumnsTemplate.aspx.cs" Inherits="Website.ArticleTwoColumnsTemplate" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CPH1" runat="server">

	<div style="float:left;width:482px;padding:0 5px 0 0">
		<CMS:HtmlEditor runat="server" ContentKey="lefttext" Style="width:auto;height:auto" />
	</div>
	<div style="float:left;width:482px;padding:0 0 0 5px">
		<CMS:HtmlEditor runat="server" ContentKey="righttext" Style="width:auto;height:auto" />
	</div>

</asp:Content>
