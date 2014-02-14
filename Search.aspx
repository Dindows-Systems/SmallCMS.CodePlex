<%@ Page Title="" Language="C#" EnableViewStateMac="false" ViewStateEncryptionMode="Never" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeBehind="Search.aspx.cs" Inherits="Website.Search" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CPH1" runat="server">
<h3>
	Search</h3>
<p>
	<asp:TextBox runat="server" ID="txtKeywords" />
	<asp:Button runat="server" ID="btnSearch" Text="Search" OnClick="btnSearch_Click" />
</p>
<p>
	<asp:Label runat="server" ID="lblTotal" Text="Found no pages." EnableViewState="false" /></p>
<asp:ListView runat="server" ID="lvwResults" EnableViewState="false">
	<LayoutTemplate>
		<blockquote>
			<asp:PlaceHolder runat="server" ID="itemPlaceholder" />
		</blockquote>
	</LayoutTemplate>
	<ItemTemplate>
		<div style="font-weight:bolder;padding:5px 5px 0;"><%#Eval("Title")%></div>
		<div style="padding:0 5px;"><%#Eval("Description")%></div>
		<div style="padding:0 5px 5px;"><asp:HyperLink runat="server" Text='<%#Eval("Url")%>' NavigateUrl='<%#Eval("Url")%>' /></div>
	</ItemTemplate>
		<EmptyDataTemplate>
			<div style="padding: 8px;">
				<p>Your search did not match any page.</p>
				<p>Suggestions:
				<ul>
				<li>Make sure all words are spelled correctly.</li>
				<li>Try different keywords.</li>
				<li>Try more general keywords.</li>
				<li>Try fewer keywords.</li>
				</ul>
				</p>
			</div>
		</EmptyDataTemplate>
</asp:ListView>
<div>
	<asp:LinkButton runat="server" ID="lnkPrev" Text=" < " EnableViewState="false" />
	<asp:PlaceHolder runat="server" ID="phPager" EnableViewState="false" />
	<asp:LinkButton runat="server" ID="lnkNext" Text=" > " EnableViewState="false" />
</div>
</asp:Content>
