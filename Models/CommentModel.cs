using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CustomFormsApp.Models
{
    public class CommentModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TemplateId { get; set; }

        [ForeignKey("TemplateId")]
        public TemplateModel Template { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "Comment length can't exceed 500 characters.")]
        public string Content { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}