using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThesisDB.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Zusammenfassung")]
        public string Summary { get; set; }

        [Display(Name = "Stärken")]
        public string Strengths { get; set; }

        [Display(Name = "Schwächen")]
        public string Weaknesses { get; set; }

        [Display(Name = "Bewertung")]
        public string Evaluation { get; set; }

        [Required(ErrorMessage = "Bitte bewerten Sie den Inhalt.")]
        [Range(1, 5, ErrorMessage = "Der Wert muss zwischen 1 und 5 liegen.")]
        [Display(Name = "Inhalt")]
        public int ContentVal { get; set; }

        [Required(ErrorMessage = "Bitte bewerten Sie die Gestaltung.")]
        [Range(1, 5, ErrorMessage = "Der Wert muss zwischen 1 und 5 liegen.")]
        [Display(Name = "Gestaltung")]
        public int LayoutVal { get; set; }

        [Required(ErrorMessage = "Bitte bewerten Sie den Aufbau.")]
        [Range(1, 5, ErrorMessage = "Der Wert muss zwischen 1 und 5 liegen.")]
        [Display(Name = "Aufbau")]
        public int StructureVal { get; set; }

        [Required(ErrorMessage = "Bitte bewerten Sie die Sprache.")]
        [Range(1, 5, ErrorMessage = "Der Wert muss zwischen 1 und 5 liegen.")]
        [Display(Name = "Sprache")]
        public int StyleVal { get; set; }

        [Required(ErrorMessage = "Bitte bewerten Sie die Literaturanalyse.")]
        [Range(1, 5, ErrorMessage = "Der Wert muss zwischen 1 und 5 liegen.")]
        [Display(Name = "Literature")]
        public int LiteratureVal { get; set; }

        [Required(ErrorMessage = "Bitte bewerten Sie die Schwierigkeit.")]
        [Range(1, 5, ErrorMessage = "Der Wert muss zwischen 1 und 5 liegen.")]
        [Display(Name = "Schwierigkeit")]
        public int DifficultyVal { get; set; }

        [Required(ErrorMessage = "Bitte bewerten Sie die Neuartigkeit.")]
        [Range(1, 5, ErrorMessage = "Der Wert muss zwischen 1 und 5 liegen.")]
        [Display(Name = "Neuartigkeit")]
        public int NoveltyVal { get; set; }

        [Required(ErrorMessage = "Bitte bewerten Sie die Themenerfassung.")]
        [Range(1, 5, ErrorMessage = "Der Wert muss zwischen 1 und 5 liegen.")]
        [Display(Name = "Themenerfassung")]
        public int RichnessVal { get; set; }

        [Required]
        public int ContentWt { get; set; } = 30;

        [Required]
        public int LayoutWt { get; set; } = 15;

        [Required]
        public int StructureWt { get; set; } = 10;

        [Required]
        public int StyleWt { get; set; } = 10;

        [Required]
        public int LiteratureWt { get; set; } = 10;

        [Required]
        public int DifficultyWt { get; set; } = 5;

        [Required]
        public int NoveltyWt { get; set; } = 10;

        [Required]
        public int RichnessWt { get; set; } = 10;

        [Required(ErrorMessage = "Bitte geben Sie eine Endnote ein.")]
        [Display(Name = "Endnote")]
        [DisplayFormat(DataFormatString = "{0:F1}", ApplyFormatInEditMode = true)]
        public double Grade { get; set; }

        // Foreign Key für Thesis (Pflichtfeld 1:1)
        [Required]
        public int ThesisId { get; set; }

        [ForeignKey("ThesisId")]
        public Thesis Thesis { get; set; }
    }
}
