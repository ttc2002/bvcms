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

namespace CmsWeb.Areas.OnlineReg.Views
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "1.3.2.0")]
    public partial class ManageGiving2 : RazorGenerator.Templating.RazorTemplateBase
    {
#line hidden

        #line 3 "..\..\Areas\OnlineReg\Views\ManageGiving2.cshtml"
    
    public CmsWeb.Models.ManageGivingModel Model { get; set; }

        #line default
        #line hidden

        public override void Execute()
        {


WriteLiteral("\r\n");


WriteLiteral("\r\n<table cellpadding=\"4\" style=\"font-family: Sans-Serif\">\r\n<tr>\r\n    <td align=\"r" +
"ight\">Give to Funds:</td>\r\n    <td>\r\n        <table class=\"grid\" cellpadding=\"2\"" +
">\r\n");


            
            #line 11 "..\..\Areas\OnlineReg\Views\ManageGiving2.cshtml"
   var funds = CmsWeb.Models.OnlineRegPersonModel.Funds(); 

            
            #line default
            #line hidden

            
            #line 12 "..\..\Areas\OnlineReg\Views\ManageGiving2.cshtml"
 for (var n = 0; n < funds.Length; n++)
{
    var i = funds[n];
    var amt = Model.FundItemValue(UtilityExtensions.Util.ToInt(i.Value)) ?? 0;
    Model.total += amt;
    if (amt > 0)
    {

            
            #line default
            #line hidden
WriteLiteral("            <tr>\r\n                <td>");


            
            #line 20 "..\..\Areas\OnlineReg\Views\ManageGiving2.cshtml"
               Write(i.Text);

            
            #line default
            #line hidden
WriteLiteral("</td>\r\n                <td align=\"right\">");


            
            #line 21 "..\..\Areas\OnlineReg\Views\ManageGiving2.cshtml"
                             Write(amt.ToString("n2"));

            
            #line default
            #line hidden
WriteLiteral("</td>\r\n            </tr>\r\n");


            
            #line 23 "..\..\Areas\OnlineReg\Views\ManageGiving2.cshtml"
    }
}

            
            #line default
            #line hidden
WriteLiteral("            <tr class=\"alt\">\r\n                <td>Total</td>\r\n                <td" +
" align=\"right\">$");


            
            #line 27 "..\..\Areas\OnlineReg\Views\ManageGiving2.cshtml"
                              Write(Model.total.ToString("n2"));

            
            #line default
            #line hidden
WriteLiteral("</td>\r\n            </tr>\r\n        </table>\r\n    </td>\r\n</tr>\r\n<tr>\r\n    <td align" +
"=\"right\">Frequency:</td>\r\n    <td>\r\n");


            
            #line 35 "..\..\Areas\OnlineReg\Views\ManageGiving2.cshtml"
             if (Model.SemiEvery == "S")
            {

            
            #line default
            #line hidden
WriteLiteral("                ");

WriteLiteral("Twice monthly on day ");


            
            #line 37 "..\..\Areas\OnlineReg\Views\ManageGiving2.cshtml"
                                  Write(Model.Day1);

            
            #line default
            #line hidden
WriteLiteral(" and ");


            
            #line 37 "..\..\Areas\OnlineReg\Views\ManageGiving2.cshtml"
                                                  Write(Model.Day2);

            
            #line default
            #line hidden
WriteLiteral("\r\n");


            
            #line 38 "..\..\Areas\OnlineReg\Views\ManageGiving2.cshtml"
            }
            else
            {

            
            #line default
            #line hidden
WriteLiteral("                ");

WriteLiteral("Every ");


            
            #line 41 "..\..\Areas\OnlineReg\Views\ManageGiving2.cshtml"
                   Write(Model.EveryN);

            
            #line default
            #line hidden
WriteLiteral(" ");


            
            #line 41 "..\..\Areas\OnlineReg\Views\ManageGiving2.cshtml"
                                  Write(Model.Period == "M" ? "Months" : "Weeks");

            
            #line default
            #line hidden
WriteLiteral("\r\n");


            
            #line 42 "..\..\Areas\OnlineReg\Views\ManageGiving2.cshtml"
            }

            
            #line default
            #line hidden
WriteLiteral("    </td>\r\n</tr>\r\n<tr>\r\n    <td align=\"right\">First Payment:</td>\r\n    <td>\r\n    " +
"    On or after ");


            
            #line 48 "..\..\Areas\OnlineReg\Views\ManageGiving2.cshtml"
               Write(UtilityExtensions.Util.FormatDate(Model.StartWhen));

            
            #line default
            #line hidden
WriteLiteral("\r\n    </td>\r\n</tr>\r\n<tr>\r\n    <td align=\"right\">Stop Payments:</td>\r\n    <td> Aft" +
"er ");


            
            #line 53 "..\..\Areas\OnlineReg\Views\ManageGiving2.cshtml"
          Write(UtilityExtensions.Util.FormatDate(Model.StopWhen));

            
            #line default
            #line hidden
WriteLiteral(" </td>\r\n</tr>\r\n<tr>\r\n    <td align=\"right\">Pay From:</td>\r\n    <td>\r\n");


            
            #line 58 "..\..\Areas\OnlineReg\Views\ManageGiving2.cshtml"
     if (Model.Type == "C")
    {

            
            #line default
            #line hidden
WriteLiteral("        ");

WriteLiteral("Debit/Credit Card # ");


            
            #line 60 "..\..\Areas\OnlineReg\Views\ManageGiving2.cshtml"
                         Write(UtilityExtensions.Util.MaskCC(Model.Cardnumber));

            
            #line default
            #line hidden
WriteLiteral("<br />\r\n");



WriteLiteral("        ");

WriteLiteral("Expires ");


            
            #line 61 "..\..\Areas\OnlineReg\Views\ManageGiving2.cshtml"
             Write(Model.Expires);

            
            #line default
            #line hidden
WriteLiteral("\r\n");


            
            #line 62 "..\..\Areas\OnlineReg\Views\ManageGiving2.cshtml"
    }
    else
    {

            
            #line default
            #line hidden
WriteLiteral("        ");

WriteLiteral("Bank Account # ");


            
            #line 65 "..\..\Areas\OnlineReg\Views\ManageGiving2.cshtml"
                    Write(UtilityExtensions.Util.MaskAccount(Model.Account));

            
            #line default
            #line hidden
WriteLiteral("\r\n");


            
            #line 66 "..\..\Areas\OnlineReg\Views\ManageGiving2.cshtml"
    }

            
            #line default
            #line hidden
WriteLiteral("    </td>\r\n</tr>\r\n</table>");


        }
    }
}
#pragma warning restore 1591