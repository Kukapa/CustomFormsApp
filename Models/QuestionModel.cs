using System.ComponentModel.DataAnnotations;

namespace CustomFormsApp.Models
{
    public class QuestionModel
    {
        public int Id { get; set; }
        public int TemplateId { get; set; }

        [Required(ErrorMessage = "Question title is required.")]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Please select the type of the question.")]
        public string QuestionType { get; set; }

        public bool IsVisibleInResults { get; set; } = true;

        public TemplateModel Template { get; set; }
    }
}
