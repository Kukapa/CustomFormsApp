﻿using System.Collections.Generic;
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

        [Required(ErrorMessage = "Tags are required")]
        public string Tags { get; set; }

        public bool IsPublic { get; set; }

        public List<QuestionModel> Questions { get; set; } = new List<QuestionModel>();

        public string OwnerUserId { get; set; }

        public List<FilledFormModel> FilledForms { get; set; } = new List<FilledFormModel>();
    }
}
