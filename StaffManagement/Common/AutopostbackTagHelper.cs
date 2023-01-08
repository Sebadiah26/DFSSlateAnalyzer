using Microsoft.AspNetCore.Razor.TagHelpers;

namespace StaffManagement.Common
{
    [HtmlTargetElement("select", Attributes = "autopostback")]
    [HtmlTargetElement("input", Attributes = "autopostback")]
    public class AutopostbackTagHelper : TagHelper
    {
        public bool autopostback { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (autopostback)
            //output.TagName = "div";
            // output.Attributes.Add("class", "latestBlogPosts");
            {
                output.Attributes.SetAttribute("onchange", "this.form.submit();  ");
            }

            //output.Content.SetHtmlContent("<div>Blah</div");
        }
    }
}
