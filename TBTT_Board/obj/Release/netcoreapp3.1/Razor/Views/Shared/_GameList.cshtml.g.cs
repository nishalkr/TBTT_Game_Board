#pragma checksum "C:\NC_Projects\TBTT_Application\TBTT_Board\Views\Shared\_GameList.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "d02cf77d13f912e9bb3191dd53ff522bb1d5edc7"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Shared__GameList), @"mvc.1.0.view", @"/Views/Shared/_GameList.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "C:\NC_Projects\TBTT_Application\TBTT_Board\Views\_ViewImports.cshtml"
using TBTT_Board;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\NC_Projects\TBTT_Application\TBTT_Board\Views\_ViewImports.cshtml"
using TBTT_Board.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"d02cf77d13f912e9bb3191dd53ff522bb1d5edc7", @"/Views/Shared/_GameList.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"b8f0208410e2483024a761539657fa8c871eff70", @"/Views/_ViewImports.cshtml")]
    public class Views_Shared__GameList : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<IEnumerable<TBTT_Board.Models.GameViewModel>>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("    <ul id=\"gameSelector\">\r\n");
#nullable restore
#line 3 "C:\NC_Projects\TBTT_Application\TBTT_Board\Views\Shared\_GameList.cshtml"
           int intCounter = 0; string className = "fa fa-user-circle-o fa-color-white"; bool isDoubles = true; int intCourtTypeCount = 4;

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n");
#nullable restore
#line 5 "C:\NC_Projects\TBTT_Application\TBTT_Board\Views\Shared\_GameList.cshtml"
         foreach (var item in Model)
        {
            

#line default
#line hidden
#nullable disable
#nullable restore
#line 7 "C:\NC_Projects\TBTT_Application\TBTT_Board\Views\Shared\_GameList.cshtml"
             if (!item.IsDoubles)
            {
                intCourtTypeCount = 2;
            }

#line default
#line hidden
#nullable disable
#nullable restore
#line 10 "C:\NC_Projects\TBTT_Application\TBTT_Board\Views\Shared\_GameList.cshtml"
             

            intCounter = intCounter + 1;
            

#line default
#line hidden
#nullable disable
#nullable restore
#line 13 "C:\NC_Projects\TBTT_Application\TBTT_Board\Views\Shared\_GameList.cshtml"
             if (intCounter > intCourtTypeCount)
            {

#line default
#line hidden
#nullable disable
            WriteLiteral("                <li class=\"GameScoreSelector\" data-jstree=\'{\"icon\": \"fa fa-user-circle-o fa-color-blue\",\"state\" : {\"opened\" : true, \"disable_node\" : true }}\'>\r\n");
#nullable restore
#line 16 "C:\NC_Projects\TBTT_Application\TBTT_Board\Views\Shared\_GameList.cshtml"
                     if ((item.MembershipType == "M") || (item.MembershipType == "G"))
                    {

#line default
#line hidden
#nullable disable
            WriteLiteral("                        <a class=\"additionalMember\" data-nodeid=\"");
#nullable restore
#line 18 "C:\NC_Projects\TBTT_Application\TBTT_Board\Views\Shared\_GameList.cshtml"
                                                            Write(item.MembershipID);

#line default
#line hidden
#nullable disable
            WriteLiteral("\" data-nodeCourt=\"");
#nullable restore
#line 18 "C:\NC_Projects\TBTT_Application\TBTT_Board\Views\Shared\_GameList.cshtml"
                                                                                                Write(item.CourtName);

#line default
#line hidden
#nullable disable
            WriteLiteral("\" data-minGameStartTime=\"");
#nullable restore
#line 18 "C:\NC_Projects\TBTT_Application\TBTT_Board\Views\Shared\_GameList.cshtml"
                                                                                                                                        Write(item.MinGameStartDate);

#line default
#line hidden
#nullable disable
            WriteLiteral("\" data-gameElapsedTime=\"");
#nullable restore
#line 18 "C:\NC_Projects\TBTT_Application\TBTT_Board\Views\Shared\_GameList.cshtml"
                                                                                                                                                                                      Write(item.GameStopWatchstartMin);

#line default
#line hidden
#nullable disable
            WriteLiteral("\" data-nodemembershiptype=\"");
#nullable restore
#line 18 "C:\NC_Projects\TBTT_Application\TBTT_Board\Views\Shared\_GameList.cshtml"
                                                                                                                                                                                                                                            Write(item.MembershipType);

#line default
#line hidden
#nullable disable
            WriteLiteral("\" data-nodecount=\"");
#nullable restore
#line 18 "C:\NC_Projects\TBTT_Application\TBTT_Board\Views\Shared\_GameList.cshtml"
                                                                                                                                                                                                                                                                                  Write(item.NodeCounter);

#line default
#line hidden
#nullable disable
            WriteLiteral("\" data-nodeOrderID=\"");
#nullable restore
#line 18 "C:\NC_Projects\TBTT_Application\TBTT_Board\Views\Shared\_GameList.cshtml"
                                                                                                                                                                                                                                                                                                                       Write(item.OrderID);

#line default
#line hidden
#nullable disable
            WriteLiteral("\" data-nodemembername=\"");
#nullable restore
#line 18 "C:\NC_Projects\TBTT_Application\TBTT_Board\Views\Shared\_GameList.cshtml"
                                                                                                                                                                                                                                                                                                                                                           Write(item.MemberName);

#line default
#line hidden
#nullable disable
            WriteLiteral("\" href=\"#\">(R)");
#nullable restore
#line 18 "C:\NC_Projects\TBTT_Application\TBTT_Board\Views\Shared\_GameList.cshtml"
                                                                                                                                                                                                                                                                                                                                                                                         Write(item.MemberName);

#line default
#line hidden
#nullable disable
            WriteLiteral("</a>\r\n");
#nullable restore
#line 19 "C:\NC_Projects\TBTT_Application\TBTT_Board\Views\Shared\_GameList.cshtml"
                    }
                    else
                    {

#line default
#line hidden
#nullable disable
            WriteLiteral("                <a class=\"nonMember additionalMember\" data-nodeid=\"");
#nullable restore
#line 22 "C:\NC_Projects\TBTT_Application\TBTT_Board\Views\Shared\_GameList.cshtml"
                                                              Write(item.MembershipID);

#line default
#line hidden
#nullable disable
            WriteLiteral("\" data-nodeCourt=\"");
#nullable restore
#line 22 "C:\NC_Projects\TBTT_Application\TBTT_Board\Views\Shared\_GameList.cshtml"
                                                                                                  Write(item.CourtName);

#line default
#line hidden
#nullable disable
            WriteLiteral("\" data-minGameStartTime=\"");
#nullable restore
#line 22 "C:\NC_Projects\TBTT_Application\TBTT_Board\Views\Shared\_GameList.cshtml"
                                                                                                                                          Write(item.MinGameStartDate);

#line default
#line hidden
#nullable disable
            WriteLiteral("\" data-gameElapsedTime=\"");
#nullable restore
#line 22 "C:\NC_Projects\TBTT_Application\TBTT_Board\Views\Shared\_GameList.cshtml"
                                                                                                                                                                                        Write(item.GameStopWatchstartMin);

#line default
#line hidden
#nullable disable
            WriteLiteral("\" data-nodemembershiptype=\"");
#nullable restore
#line 22 "C:\NC_Projects\TBTT_Application\TBTT_Board\Views\Shared\_GameList.cshtml"
                                                                                                                                                                                                                                              Write(item.MembershipType);

#line default
#line hidden
#nullable disable
            WriteLiteral("\" data-nodecount=\"");
#nullable restore
#line 22 "C:\NC_Projects\TBTT_Application\TBTT_Board\Views\Shared\_GameList.cshtml"
                                                                                                                                                                                                                                                                                    Write(item.NodeCounter);

#line default
#line hidden
#nullable disable
            WriteLiteral("\" data-nodeOrderID=\"");
#nullable restore
#line 22 "C:\NC_Projects\TBTT_Application\TBTT_Board\Views\Shared\_GameList.cshtml"
                                                                                                                                                                                                                                                                                                                         Write(item.OrderID);

#line default
#line hidden
#nullable disable
            WriteLiteral("\" data-nodemembername=\"");
#nullable restore
#line 22 "C:\NC_Projects\TBTT_Application\TBTT_Board\Views\Shared\_GameList.cshtml"
                                                                                                                                                                                                                                                                                                                                                             Write(item.MemberName);

#line default
#line hidden
#nullable disable
            WriteLiteral("\" href=\"#\">(R)");
#nullable restore
#line 22 "C:\NC_Projects\TBTT_Application\TBTT_Board\Views\Shared\_GameList.cshtml"
                                                                                                                                                                                                                                                                                                                                                                                           Write(item.MemberName);

#line default
#line hidden
#nullable disable
            WriteLiteral("</a>\r\n");
#nullable restore
#line 23 "C:\NC_Projects\TBTT_Application\TBTT_Board\Views\Shared\_GameList.cshtml"
                    }

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </li>\r\n");
#nullable restore
#line 26 "C:\NC_Projects\TBTT_Application\TBTT_Board\Views\Shared\_GameList.cshtml"
            }
            else
            {

#line default
#line hidden
#nullable disable
            WriteLiteral("                <li class=\"GameScoreSelector\" data-jstree=\'{\"icon\": \"fa fa-user-circle-o fa-color-white\",\"state\" : {\"opened\" : true, \"disable_node\" : true }}\'>\r\n");
#nullable restore
#line 30 "C:\NC_Projects\TBTT_Application\TBTT_Board\Views\Shared\_GameList.cshtml"
                     if ((item.MembershipType == "M") || (item.MembershipType == "G"))
                    {

#line default
#line hidden
#nullable disable
            WriteLiteral("                        <a data-nodeid=\"");
#nullable restore
#line 32 "C:\NC_Projects\TBTT_Application\TBTT_Board\Views\Shared\_GameList.cshtml"
                                   Write(item.MembershipID);

#line default
#line hidden
#nullable disable
            WriteLiteral("\" data-nodeCourt=\"");
#nullable restore
#line 32 "C:\NC_Projects\TBTT_Application\TBTT_Board\Views\Shared\_GameList.cshtml"
                                                                       Write(item.CourtName);

#line default
#line hidden
#nullable disable
            WriteLiteral("\" data-minGameStartTime=\"");
#nullable restore
#line 32 "C:\NC_Projects\TBTT_Application\TBTT_Board\Views\Shared\_GameList.cshtml"
                                                                                                               Write(item.MinGameStartDate);

#line default
#line hidden
#nullable disable
            WriteLiteral("\" data-gameElapsedTime=\"");
#nullable restore
#line 32 "C:\NC_Projects\TBTT_Application\TBTT_Board\Views\Shared\_GameList.cshtml"
                                                                                                                                                             Write(item.GameStopWatchstartMin);

#line default
#line hidden
#nullable disable
            WriteLiteral("\" data-nodemembershiptype=\"");
#nullable restore
#line 32 "C:\NC_Projects\TBTT_Application\TBTT_Board\Views\Shared\_GameList.cshtml"
                                                                                                                                                                                                                   Write(item.MembershipType);

#line default
#line hidden
#nullable disable
            WriteLiteral("\" data-nodecount=\"");
#nullable restore
#line 32 "C:\NC_Projects\TBTT_Application\TBTT_Board\Views\Shared\_GameList.cshtml"
                                                                                                                                                                                                                                                         Write(item.NodeCounter);

#line default
#line hidden
#nullable disable
            WriteLiteral("\" data-nodeOrderID=\"");
#nullable restore
#line 32 "C:\NC_Projects\TBTT_Application\TBTT_Board\Views\Shared\_GameList.cshtml"
                                                                                                                                                                                                                                                                                              Write(item.OrderID);

#line default
#line hidden
#nullable disable
            WriteLiteral("\" data-nodemembername=\"");
#nullable restore
#line 32 "C:\NC_Projects\TBTT_Application\TBTT_Board\Views\Shared\_GameList.cshtml"
                                                                                                                                                                                                                                                                                                                                  Write(item.MemberName);

#line default
#line hidden
#nullable disable
            WriteLiteral("\" href=\"#\">&nbsp;");
#nullable restore
#line 32 "C:\NC_Projects\TBTT_Application\TBTT_Board\Views\Shared\_GameList.cshtml"
                                                                                                                                                                                                                                                                                                                                                                   Write(item.MemberName);

#line default
#line hidden
#nullable disable
            WriteLiteral("</a>\r\n");
#nullable restore
#line 33 "C:\NC_Projects\TBTT_Application\TBTT_Board\Views\Shared\_GameList.cshtml"
                    }
                    else
                    {

#line default
#line hidden
#nullable disable
            WriteLiteral("                        <a class=\"nonMember\" data-nodeid=\"");
#nullable restore
#line 36 "C:\NC_Projects\TBTT_Application\TBTT_Board\Views\Shared\_GameList.cshtml"
                                                     Write(item.MembershipID);

#line default
#line hidden
#nullable disable
            WriteLiteral("\" data-nodeCourt=\"");
#nullable restore
#line 36 "C:\NC_Projects\TBTT_Application\TBTT_Board\Views\Shared\_GameList.cshtml"
                                                                                         Write(item.CourtName);

#line default
#line hidden
#nullable disable
            WriteLiteral("\" data-minGameStartTime=\"");
#nullable restore
#line 36 "C:\NC_Projects\TBTT_Application\TBTT_Board\Views\Shared\_GameList.cshtml"
                                                                                                                                 Write(item.MinGameStartDate);

#line default
#line hidden
#nullable disable
            WriteLiteral("\" data-gameElapsedTime=\"");
#nullable restore
#line 36 "C:\NC_Projects\TBTT_Application\TBTT_Board\Views\Shared\_GameList.cshtml"
                                                                                                                                                                               Write(item.GameStopWatchstartMin);

#line default
#line hidden
#nullable disable
            WriteLiteral("\" data-nodemembershiptype=\"");
#nullable restore
#line 36 "C:\NC_Projects\TBTT_Application\TBTT_Board\Views\Shared\_GameList.cshtml"
                                                                                                                                                                                                                                     Write(item.MembershipType);

#line default
#line hidden
#nullable disable
            WriteLiteral("\" data-nodecount=\"");
#nullable restore
#line 36 "C:\NC_Projects\TBTT_Application\TBTT_Board\Views\Shared\_GameList.cshtml"
                                                                                                                                                                                                                                                                           Write(item.NodeCounter);

#line default
#line hidden
#nullable disable
            WriteLiteral("\" data-nodeOrderID=\"");
#nullable restore
#line 36 "C:\NC_Projects\TBTT_Application\TBTT_Board\Views\Shared\_GameList.cshtml"
                                                                                                                                                                                                                                                                                                                Write(item.OrderID);

#line default
#line hidden
#nullable disable
            WriteLiteral("\" data-nodemembername=\"");
#nullable restore
#line 36 "C:\NC_Projects\TBTT_Application\TBTT_Board\Views\Shared\_GameList.cshtml"
                                                                                                                                                                                                                                                                                                                                                    Write(item.MemberName);

#line default
#line hidden
#nullable disable
            WriteLiteral("\" href=\"#\">&nbsp;");
#nullable restore
#line 36 "C:\NC_Projects\TBTT_Application\TBTT_Board\Views\Shared\_GameList.cshtml"
                                                                                                                                                                                                                                                                                                                                                                                     Write(item.MemberName);

#line default
#line hidden
#nullable disable
            WriteLiteral("</a>\r\n");
#nullable restore
#line 37 "C:\NC_Projects\TBTT_Application\TBTT_Board\Views\Shared\_GameList.cshtml"
                    }

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </li>\r\n");
#nullable restore
#line 40 "C:\NC_Projects\TBTT_Application\TBTT_Board\Views\Shared\_GameList.cshtml"
            }

#line default
#line hidden
#nullable disable
#nullable restore
#line 40 "C:\NC_Projects\TBTT_Application\TBTT_Board\Views\Shared\_GameList.cshtml"
             


        }

#line default
#line hidden
#nullable disable
            WriteLiteral("    </ul>");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<IEnumerable<TBTT_Board.Models.GameViewModel>> Html { get; private set; }
    }
}
#pragma warning restore 1591
