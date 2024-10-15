namespace CustomFormsApp.Models
{
    public class TemplateDetailsViewModel
    {
        public TemplateModel Template { get; set; }
        public bool CanManageTemplate { get; set; } 
        public bool IsAdmin { get; set; } 
    }
}
