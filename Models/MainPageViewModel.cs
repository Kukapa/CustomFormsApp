using System.Collections.Generic;
using CustomFormsApp.Models;

namespace CustomFormsApp.Models
{
    public class MainPageViewModel
    {
        public List<TemplateModel> LatestTemplates { get; set; }
        public List<TemplateModel> PopularTemplates { get; set; }
        public List<TagCloudViewModel> TagCloud { get; set; }
    }
}
