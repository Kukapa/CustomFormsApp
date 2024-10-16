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
        public QuestionType Type { get; set; }

        public bool ShowInTable { get; set; }

        public TemplateModel Template { get; set; }
    }

    public enum QuestionType
    {
        SingleLineString,
        MultiLineText,
        PositiveInteger,
        Checkbox
    }
}
