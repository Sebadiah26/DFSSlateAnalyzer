using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

/// <summary>
/// Override to the asp-for label tag helper to display "*" in red immediately following the label text for a model property.
/// </summary>
namespace StaffManagement.Common
{
    [HtmlTargetElement("label", Attributes = "asp-for")]


    public class DerivedFieldTagHelper : LabelTagHelper
    {
        /// <summary>
        /// Constructor firing Label tag helper base class
        /// </summary>
        /// <param name="htmlGenerator"></param>
        public DerivedFieldTagHelper(IHtmlGenerator htmlGenerator)
            : base(htmlGenerator)
        {

        }


        /// <summary>
        /// Main method of a tag helper
        /// </summary>
        /// <param name="context"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            await base.ProcessAsync(context, output);

            //var metadata = For.Metadata as DefaultModelMetadata;

            //// Look for "Derived" attribute in the metadata of the given property named in the asp-for tag of the tag helper.
            //bool hasDerivedAttribute = metadata?.Attributes.PropertyAttributes.Any(i => i.GetType() == typeof(DerivedField)) ?? false;

            //// if true, append span tag to the "PostContent" portion of the tag helper's output.
            //if (hasDerivedAttribute)
            //{

            //    output.PostContent.AppendHtml("<span class='fw-bold text-danger'> Derived</span>");
            //}




        }
    }
}
