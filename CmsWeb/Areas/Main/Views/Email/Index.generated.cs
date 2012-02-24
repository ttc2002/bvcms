﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.239
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CmsWeb.Areas.Main.Views.Email
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Web;
    using System.Web.Helpers;
    using System.Web.Mvc;
    using System.Web.Mvc.Ajax;
    using System.Web.Mvc.Html;
    using System.Web.Routing;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.WebPages;
    
    #line 2 "..\..\Areas\Main\Views\Email\Index.cshtml"
    using CmsWeb;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "1.3.2.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Areas/Main/Views/Email/Index.cshtml")]
    public class Index : System.Web.Mvc.WebViewPage<CmsWeb.Areas.Main.Models.MassEmailer>
    {
        public Index()
        {
        }
        public override void Execute()
        {



            
            #line 3 "..\..\Areas\Main\Views\Email\Index.cshtml"
  
    Layout = "/Views/Shared/SiteLayout.cshtml";
    ViewBag.Title = "Email";


            
            #line default
            #line hidden

DefineSection("head", () => {

WriteLiteral("\r\n<style type=\"text/css\">\r\nfieldset label {\r\n    display: block;\r\n}\r\n</style>\r\n");


});

WriteLiteral("\r\n<div>\r\n<span class=\"style1\">Please Note</span>: \r\nYou can include a file, image" +
", mp3 or whatever you like in your email. \r\n<a href=\"http://www.bvcms.com/Doc/Em" +
"ailFileUpload\">Read this article for instructions</a>.\r\n");


            
            #line 19 "..\..\Areas\Main\Views\Email\Index.cshtml"
 using (Html.BeginForm("QueueEmails", "Email"))
{ 

            
            #line default
            #line hidden
WriteLiteral("   <div>\r\n   <fieldset>\r\n   ");


            
            #line 23 "..\..\Areas\Main\Views\Email\Index.cshtml"
Write(Html.Hidden("QBId"));

            
            #line default
            #line hidden
WriteLiteral("\r\n   ");


            
            #line 24 "..\..\Areas\Main\Views\Email\Index.cshtml"
Write(Html.Hidden("Host"));

            
            #line default
            #line hidden
WriteLiteral("\r\n   ");


            
            #line 25 "..\..\Areas\Main\Views\Email\Index.cshtml"
Write(Html.Hidden("CmsHost"));

            
            #line default
            #line hidden
WriteLiteral("\r\n   ");


            
            #line 26 "..\..\Areas\Main\Views\Email\Index.cshtml"
Write(Html.Hidden("Count", Model.Count));

            
            #line default
            #line hidden
WriteLiteral("\r\n   ");


            
            #line 27 "..\..\Areas\Main\Views\Email\Index.cshtml"
Write(Html.Hidden("wantParents", Model.wantParents));

            
            #line default
            #line hidden
WriteLiteral("\r\n   <p>Number of Emails: ");


            
            #line 28 "..\..\Areas\Main\Views\Email\Index.cshtml"
                   Write(Model.Count);

            
            #line default
            #line hidden
WriteLiteral(" ");


            
            #line 28 "..\..\Areas\Main\Views\Email\Index.cshtml"
                                Write(ViewData["parentsof"]);

            
            #line default
            #line hidden
WriteLiteral("</p>\r\n   <p>\r\n        <input type=\"button\" id=\"Send\" value=\"Send\" class=\"bt\" styl" +
"e=\"width:62px;height: 42px\" />\r\n");


            
            #line 31 "..\..\Areas\Main\Views\Email\Index.cshtml"
         if ((User.IsInRole("ScheduleEmails") || User.IsInRole("Edit")) && System.Web.Configuration.WebConfigurationManager.AppSettings["UseEmailScheduler"] == "true")
        { 

            
            #line default
            #line hidden
WriteLiteral("        ");

WriteLiteral("Scheduled Date and Time (mm/dd/yy h:mm AM|PM)");


            
            #line 33 "..\..\Areas\Main\Views\Email\Index.cshtml"
                                                  Write(Html.TextBox("Schedule", Model.Schedule, new { style = "width:120px" }));

            
            #line default
            #line hidden
WriteLiteral(" (Optional, note: this is Central Time)</p>\r\n");


            
            #line 34 "..\..\Areas\Main\Views\Email\Index.cshtml"
        } 

            
            #line default
            #line hidden
WriteLiteral("   <p>From: ");


            
            #line 35 "..\..\Areas\Main\Views\Email\Index.cshtml"
       Write(Html.DropDownList("FromAddress", Model.EmailFroms()));

            
            #line default
            #line hidden
WriteLiteral("\r\n    <input type=\"button\" id=\"TestSend\" class=\"bt\" value=\"Test (Send To Yourself" +
")\" />\r\n    &nbsp;&nbsp;Should this email be publicly viewable? \r\n    ");


            
            #line 38 "..\..\Areas\Main\Views\Email\Index.cshtml"
Write(Html.CheckBox("PublicViewable"));

            
            #line default
            #line hidden
WriteLiteral("\r\n    </p>\r\n    <p><label>Subject:</label>\r\n    ");


            
            #line 41 "..\..\Areas\Main\Views\Email\Index.cshtml"
Write(Html.TextBox("Subject", Model.Subject, new { style = "width:90%" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n    </p>\r\n    <p><label>Body:</label>\r\n    ");


            
            #line 44 "..\..\Areas\Main\Views\Email\Index.cshtml"
Write(Html.TextArea("Body", Model.Body, new { @class = "editor", rows="16", cols="20" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n    </p>\r\n    </fieldset>\r\n    </div>\r\n");


            
            #line 48 "..\..\Areas\Main\Views\Email\Index.cshtml"
} 

            
            #line default
            #line hidden
WriteLiteral(@"    <h4>VoteTag</h4>
    <p>You can now use votelinks and votetags by highlighting any of your text or an image, 
    and then clicking on the hyperlink icon in the toolbar (looks like the world with a chain link). 
    Then put the word votelink in the URL field and fill out the Special Links tab.</p>
</div>
");


DefineSection("scripts", () => {

WriteLiteral("\r\n<script src=\"/ckeditor/ckeditor.js\" type=\"text/javascript\"></script>\r\n<script s" +
"rc=\"/ckeditor/adapters/jquery.js\" type=\"text/javascript\"></script>\r\n");


            
            #line 58 "..\..\Areas\Main\Views\Email\Index.cshtml"
Write(Html.Script("/Scripts/Email.js"));

            
            #line default
            #line hidden
WriteLiteral("\r\n");


            
            #line 59 "..\..\Areas\Main\Views\Email\Index.cshtml"
Write(Html.Script("/Scripts/Edit.js"));

            
            #line default
            #line hidden
WriteLiteral("\r\n");


});

WriteLiteral("\r\n<div id=\"progress\">\r\n<h2>Working...</h2>\r\n</div>");


        }
    }
}
#pragma warning restore 1591