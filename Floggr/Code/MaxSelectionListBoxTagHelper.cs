//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.AspNetCore.Mvc.ViewFeatures;
//using Microsoft.AspNetCore.Razor.TagHelpers;

//namespace Floggr.Code
//{
//    [HtmlTargetElement("max-selection-listbox")]
//    public class MaxSelectionListBoxTagHelper : TagHelper
//    {
//        [HtmlAttributeName("asp-for")]
//        public ModelExpression For { get; set; }

//        [HtmlAttributeName("items")]
//        public List<SelectListItem> Items { get; set; }

//        [HtmlAttributeName("max-selection")]
//        public int MaxSelection { get; set; } = 5;

//        public override void Process(TagHelperContext context, TagHelperOutput output)
//        {
//            if (context == null || output == null)
//            {
//                throw new ArgumentNullException();
//            }

//            var listBox = new TagBuilder("select");
//            listBox.MergeAttributes(output.Attributes.ToDictionary(attr => attr.Name, attr => attr.Value));
//            listBox.MergeAttribute("id", For.Name);

//            foreach (var item in Items)
//            {
//                listBox.InnerHtml.AppendHtml($"<option value='{item.Value}'>{item.Text}</option>");
//            }

//            output.TagName = "div";
//            output.TagMode = TagMode.StartTagAndEndTag;
//            output.Content.AppendHtml(listBox);

//            var script = $@"<script>
//            document.addEventListener('DOMContentLoaded', function() {{
//                var selectList = document.getElementById('{For.Name}');
//                selectList.addEventListener('change', function() {{
//                    var selectedCount = Array.from(selectList.selectedOptions).length;
//                    if (selectedCount > {MaxSelection}) {{
//                        alert('You can select up to {MaxSelection} items.');
//                        Array.from(selectList.options).forEach(function(option) {{
//                            option.selected = false;
//                        }});
//                        Array.from(selectList.options).slice(0, {MaxSelection}).forEach(function(option) {{
//                            option.selected = true;
//                        }});
//                    }}
//                }});
//            }});
//        </script>";

//            output.PostContent.AppendHtml(script);
//        }
//    }
//}
