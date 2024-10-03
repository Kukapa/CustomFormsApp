using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace CustomFormsApp.Models
{
    public class FormModel
    {
        [Key]
        public int FormId { get; set; } // This serves as the primary key

        // User inputs with validation attributes
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Message is required")]
        public string Message { get; set; }

        public int TemplateId { get; set; } // Assuming this is needed to link forms to templates
        public string? UserId { get; set; } // Assuming this is for tracking the user
        public DateTime DateFilled { get; set; } // Date of form submission
    }
}
