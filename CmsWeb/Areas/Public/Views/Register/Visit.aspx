﻿<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site2.Master" Inherits="System.Web.Mvc.ViewPage<CMSWeb.Models.RegisterModel>" %>

<asp:Content ID="registerHead" ContentPlaceHolderID="TitleContent" runat="server">
    <title>Register</title>
</asp:Content>

<asp:Content ID="registerContent" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        $(function() {
            $("#zip").blur(function() {
                $.post('/Register/CityState/' + $(this).val(), null, function(ret) {
                    if (ret) {
                        $('#state').val(ret.state);
                        $('#city').val(ret.city);
                    }
                }, 'json');
            });
            $('input:text:first').focus();
            $('#dob').blur(function() {
                var bd = $(this).val();
                var re0 = /^(0[1-9]|1[012])(0[1-9]|[12][0-9]|3[01])((19|20)?[0-9]{2})$/i;
                var re = /^(0?[1-9]|1[012])[\/-](0?[1-9]|[12][0-9]|3[01])[\/-]((19|20)?[0-9]{2})$/i;
                var m = re0.exec(bd);
                if (m == null)
                    m = re.exec(bd);
                if (m == null)
                    return;

                var y = parseInt(m[3]);
                if (y < 1000)
                    if (y < 50) y = y + 2000; else y = y + 1900;
                var bday = new Date(y, m[1] - 1, m[2]);
                var tday = new Date();
                if (bday > tday)
                    bday = new Date(y - 100, m[1] - 1, m[2]);

                var by = bday.getFullYear();
                var bm = bday.getMonth();
                var bd = bday.getDate();
                var age = 0;
                while (bday <= tday) {
                    bday = new Date(by + age, bm, bd);
                    age++;
                }
                age -= 2;
                $('#age').text(age);
            });
            $('#addfam').click(function() {
                var thisday = $('#thisday').val();
                var loc = "/Register/Add/<%=Model.campusid %>";
                if (thisday)
                    loc += "?thisday=" + thisday;
                window.location = loc;
            });
        });
    </script>
    <h2>Church Visit Registration</h2>
    <%=Html.Hidden("thisday", Model.thisday) %>
    <p>
        Use the form below to register a visit in our Church Database.<br />
        <a id="addfam" href='#'>Add to an existing family</a>
    </p>

    <% using (Html.BeginForm()) { %>
        <div>
            <fieldset>
                <table style="empty-cells:show">
                <col style="width: 13em; text-align:right" />
                <col />
                <col />
                <tr>
                    <td><label for="first">First Name</label></td>
                    <td><%= Html.TextBox("first") %></td>
                    <td><%= Html.ValidationMessage("first") %></td>
                </tr>
                <tr>
                    <td><label for="last">Last Name</label></td>
                    <td><%= Html.TextBox("last") %></td>
                    <td><%= Html.ValidationMessage("last") %></td>
                </tr>
                <tr>
                    <td><label for="dob">Date of Birth</label></td>
                    <td><%= Html.TextBox("dob", Model.dob, new { title = "m/d/y, mmddyy, mmddyyyy" })%> <span id="age"></span></td>
                    <td>(m/d/y) <%= Html.ValidationMessage("dob") %></td>
                </tr>
                <tr>
                    <td><label for="gender">Gender</label></td>
                    <td>
                    <%= Html.RadioButton("gender", 1, new { accesskey="m" })%> <u>M</u>ale 
                    <%= Html.RadioButton("gender", 2, new { accesskey = "f" })%> <u>F</u>emale </td>
                    <td><%= Html.ValidationMessage("gender") %></td>
                </tr>
                <tr>
                    <td><label for="married">Married</label></td>
                    <td>
                    <%= Html.RadioButton("married", 10, new { accesskey = "s" })%> <u>S</u>ingle
                    <%= Html.RadioButton("married", 20, new { accesskey = "a" })%> M<u>a</u>rried</td>
                    <td><%= Html.ValidationMessage("married") %></td>
                </tr>
                <tr>
                    <td><label for="address">Address Line 1</label></td>
                    <td><%= Html.TextBox("address1") %></td>
                    <td><%= Html.ValidationMessage("address1") %></td>
                </tr>
                <tr>
                    <td><label for="address2">Address Line 2</label></td>
                    <td><%= Html.TextBox("address2") %></td>
                    <td><%= Html.ValidationMessage("address2") %></td>
                </tr>
                <tr>
                    <td><label for="zip">Zip</label></td>
                    <td><%= Html.TextBox("zip") %></td>
                    <td><%= Html.ValidationMessage("zip") %></td>
                </tr>
                <tr>
                    <td><label for="city">City</label></td>
                    <td><%= Html.TextBox("city") %></td>
                    <td><%= Html.ValidationMessage("city") %></td>
                </tr>
                <tr>
                    <td><label for="state">State</label></td>
                    <td><%= Html.DropDownList("state", CMSWeb.Models.RegisterModel.StateList())%></td>
                    <td><%= Html.ValidationMessage("state") %></td>
                </tr>
                <tr>
                    <td><label for="phone">Home Phone</label></td>
                    <td><%= Html.TextBox("phone")%></td>
                    <td><%= Html.ValidationMessage("phone")%></td>
                </tr>
                <tr>
                    <td><label for="cellphone">Cell Phone</label></td>
                    <td><%= Html.TextBox("cellphone") %></td>
                    <td><%= Html.ValidationMessage("cellphone") %></td>
                </tr>
                <tr>
                    <td><label for="email">Email</label></td>
                    <td><%= Html.TextBox("email") %></td>
                    <td><%= Html.ValidationMessage("email") %></td>
                </tr>
                <tr>
                    <td><label for="org">Visiting Where</label></td>
                    <td><%= Html.DropDownList("org", Model.OrgList())%></td>
                    <td><%= Html.ValidationMessage("org") %></td>
                </tr>
                <tr>
                    <td>&nbsp;</td><td>
                    <input type="submit" name="submit" value="Register" />
            <% if (Model.existingfamily ?? false)
               { %>
                    <%=Html.Hidden("existingfamily", true) %>
                    <input type="submit" name="submit" value="Add to Existing Family" />
            <% } %>
                    </td>
                    <td><%= Html.ValidationMessage("existing") %></td>
                </tr>
                </table>
            </fieldset>
    <p><a href="/Register/Visit/<%=Session["campus"] %>">New Registration</a></p>
        </div>
    <% } %>
</asp:Content>