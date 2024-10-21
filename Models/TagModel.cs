using System.ComponentModel.DataAnnotations;

namespace CustomFormsApp.Models
{
    public class TagModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public List<TemplateModel> Templates { get; set; }
    }
}
