<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CMSWeb.Models.QueryModel>" %>
<% Html.RenderPartial("Conditions", Model); %>
<---------->
{
<% foreach(var e in Model.Errors)
   { %>
<%= e.Key %>: '<%= e.Value %>',
<% } %>
count: <%= Model.Errors.Count %>
}