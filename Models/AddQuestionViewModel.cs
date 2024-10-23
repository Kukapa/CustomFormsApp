using System.ComponentModel.DataAnnotations;

namespace CustomFormsApp.Models
{
    public class AddQuestionViewModel
    {
        public int TemplateId { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public QuestionType Type { get; set; }

        public bool ShowInTable { get; set; }
    }
}
