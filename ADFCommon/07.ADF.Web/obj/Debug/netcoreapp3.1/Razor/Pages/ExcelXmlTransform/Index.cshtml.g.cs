#pragma checksum "E:\github\NetCore\ADFCommon\07.ADF.Web\Pages\ExcelXmlTransform\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "a245b656971ce49d30bcc0f5c2b49648868cb8b2"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(ADF.Web.Pages.ExcelXmlTransform.Pages_ExcelXmlTransform_Index), @"mvc.1.0.razor-page", @"/Pages/ExcelXmlTransform/Index.cshtml")]
namespace ADF.Web.Pages.ExcelXmlTransform
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"a245b656971ce49d30bcc0f5c2b49648868cb8b2", @"/Pages/ExcelXmlTransform/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"79af828c381e832c7374a9b330c0e2d684face17", @"/Pages/_ViewImports.cshtml")]
    public class Pages_ExcelXmlTransform_Index : global::Microsoft.AspNetCore.Mvc.RazorPages.Page
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
#nullable restore
#line 4 "E:\github\NetCore\ADFCommon\07.ADF.Web\Pages\ExcelXmlTransform\Index.cshtml"
  
    ViewData["Title"]="Index";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<h1>病情观察的导入和导出</h1>\r\n\r\n<div class=\"form-group\">\r\n    <input type=\"submit\" value=\"Import\" class=\"btn btn-primary\" />\r\n    <input type=\"submit\" value=\"Export\" class=\"btn btn-primary\" />\r\n</div>");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<ADF.Web.Pages.ExcelXmlTransform.IndexModel> Html { get; private set; }
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<ADF.Web.Pages.ExcelXmlTransform.IndexModel> ViewData => (global::Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<ADF.Web.Pages.ExcelXmlTransform.IndexModel>)PageContext?.ViewData;
        public ADF.Web.Pages.ExcelXmlTransform.IndexModel Model => ViewData.Model;
    }
}
#pragma warning restore 1591
