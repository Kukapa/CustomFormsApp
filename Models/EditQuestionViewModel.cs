namespace CustomFormsApp.Models
{
    public class EditQuestionViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public QuestionType Type { get; set; }
        public bool ShowInTable { get; set; }
        public int TemplateId { get; set; }
    }
}
