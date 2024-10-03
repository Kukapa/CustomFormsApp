using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace CustomFormsApp.Models
{
    public class FormModel
    {
        [Key]
        public int FormId { get; set; } 

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Message is required")]
        public string Message { get; set; }

        public int TemplateId { get; set; }
        public string? UserId { get; set; }
        public DateTime DateFilled { get; set; }
    }
}
