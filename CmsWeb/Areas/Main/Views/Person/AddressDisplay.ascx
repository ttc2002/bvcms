﻿<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CMSWeb.Models.PersonPage.AddressInfo>" %>
<% if (Page.User.IsInRole("Edit"))
   { %>
<a class="displayedit" href="/Person/AddressEdit/<%=Model.PeopleId %>?type=<%=Model.Name %>">Edit</a>
<% } %>
<table>
    <tr>
        <td>
            <table class="Design2">
                <tr>
                    <th>Address:</th>
                    <td><%=Model.Address1 %></td>
                    <th>Bad Address Flag:</th>
                    <td><input type="checkbox" <%=Model.BadAddress == true ? "checked='checked'" : "" %> disabled="disabled" /></td>
                </tr>
                <tr>
                    <th>&nbsp;</th>
                    <td><%=Model.Address2 %></td>
                    <th>Resident Code:</th>
                    <td><%=Model.ResCode %></td>
                </tr>
                <tr>
                    <th>City:</th>
                    <td><%=Model.City %></td>
                </tr>
                <tr>
                    <th>State:</th>
                    <td><%=Model.State %></td>
                </tr>
                <tr>
                    <th>Zip:</th>
                    <td><%=Model.Zip.FmtZip() %></td>
                </tr>
                <tr>
                    <th style="vertical-align: top">Effective Dates:</th>
                    <td colspan="3">
                        <table class="Design2">
                            <tr>
                                <th>From:</th>
                                <td><%=Model.FromDt.FormatDate() %></td>
                            </tr>
                            <tr>
                                <th>To:</th>
                                <td><%=Model.ToDt.FormatDate() %></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <th>Preferred Address</th>
                    <td>
                        <input type="checkbox" disabled="disabled" <%=Model.Preferred ? "checked='checked'" : "" %> />
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>