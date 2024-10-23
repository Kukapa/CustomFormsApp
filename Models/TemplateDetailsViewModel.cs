namespace CustomFormsApp.Models
{
    public class TemplateDetailsViewModel
    {
        public TemplateModel Template { get; set; }
        public bool CanManageTemplate { get; set; } 
        public bool IsAdmin { get; set; }
        public List<FilledFormModel> FormResults { get; set; }
        public Dictionary<int, double> Averages { get; set; }
        public bool HasUserLiked { get; set; }
        public List<TagModel> Tags { get; set; }
    }
}
