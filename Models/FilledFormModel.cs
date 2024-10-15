using System;
using System.Collections.Generic;

namespace CustomFormsApp.Models
{
    public class FilledFormModel
    {
        public int Id { get; set; }
        public int TemplateId { get; set; }
        public TemplateModel Template { get; set; }
        public DateTime DateFilled { get; set; }

        public string UserId { get; set; }
        public ICollection<AnswerModel> Answers { get; set; }
    }
}
