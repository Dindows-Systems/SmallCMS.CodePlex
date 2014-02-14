<%@ Page Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeBehind="ArticleTemplate.aspx.cs" Inherits="Website.ArticleTemplate" %>

<asp:Content ID="C" ContentPlaceHolderID="CPH1" runat="server">

	<CMS:HtmlEditor runat="server" ContentKey="maintext" Style="width:auto;height:auto" />

</asp:Content>
