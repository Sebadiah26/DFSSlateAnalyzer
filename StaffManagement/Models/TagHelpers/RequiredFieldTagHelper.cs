using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using StaffManagement.Models.CustomAttributes;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// Override to the asp-for label tag helper to display "*" in red immediately following the label text for a model property.
/// </summary>
namespace StaffManagement.Common
{
    [HtmlTargetElement("label", Attributes = "asp-for")]


    public class RequiredFieldTagHelper : LabelTagHelper
    {
        /// <summary>
        /// Constructor firing Label tag helper base class
        /// </summary>
        /// <param name="htmlGenerator"></param>
        public RequiredFieldTagHelper(IHtmlGenerator htmlGenerator)
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

            var metadata = For.Metadata as DefaultModelMetadata;



            string requiredActionMethod = this.ViewContext.HttpContext.Request.RouteValues["action"].ToString();
            string currentActionMethod = "";

            // Look for "Required" attribute in the metadata of the given property named in the asp-for tag of the tag helper.
            bool hasRequiredAttribute = (metadata?.Attributes.PropertyAttributes?.Any(i => i.GetType() == typeof(RequiredAttribute)) ?? false)

                ;
            bool hasRequiredIfActionMethodAttribute = (metadata?.Attributes.PropertyAttributes?.Any(i => i.GetType() == typeof(RequiredIfActionMethodAttribute)) ?? false);


            if (hasRequiredIfActionMethodAttribute)
            {
                currentActionMethod = ((RequiredIfActionMethodAttribute)metadata.Attributes.PropertyAttributes.SingleOrDefault(i => i.GetType() == typeof(RequiredIfActionMethodAttribute))).RequiredActionMethod;

            }



            // if true, append span tag to the "PostContent" portion of the tag helper's output.
            if (hasRequiredAttribute || (hasRequiredIfActionMethodAttribute && (requiredActionMethod == currentActionMethod)))
            {

                output.PostContent.AppendHtml("<span class='fw-bold text-danger'> *</span>");
            }




        }
    }
}
