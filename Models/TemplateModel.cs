using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CustomFormsApp.Models
{
    public class TemplateModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }

        public string Tags { get; set; } // For tag-based search functionality later

        public bool IsPublic { get; set; } // Determines if the template is public or private

        public List<QuestionModel> Questions { get; set; } = new List<QuestionModel>(); // Questions associated with the template
    }
}
