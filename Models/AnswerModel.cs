using System.ComponentModel.DataAnnotations;

namespace CustomFormsApp.Models
{
    public class AnswerModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int QuestionId { get; set; }

        [Required]
        public string UserId { get; set; } 

        public string AnswerText { get; set; }

        public int? AnswerInteger { get; set; } 
        
        public bool? AnswerBoolean { get; set; }

        public DateTime SubmittedAt { get; set; }

        public QuestionModel Question { get; set; }
    }
}
