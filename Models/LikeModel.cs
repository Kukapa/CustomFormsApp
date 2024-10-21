using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CustomFormsApp.Models
{
    public class LikeModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TemplateId { get; set; }

        [ForeignKey("TemplateId")]
        public TemplateModel Template { get; set; }

        [Required]
        public string UserName { get; set; }
    }
}