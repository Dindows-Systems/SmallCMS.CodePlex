<%@ Page Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" Inherits="Website.HashInMenu" %>

<asp:Content ID="C" ContentPlaceHolderID="CPH1" runat="server">

<p style="font-size:200px">#</p>
<p>This page displays a # in the menu, so the user cannot navigate to it.</p>
<p>This page is accesible only to the CMS administrator to set the properties et cetera. Other users get a "404 - page not found" error.</p>

</asp:Content>