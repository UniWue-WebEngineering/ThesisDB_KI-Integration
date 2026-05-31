using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThesisDB.Models
{
    public class Thesis
    {
        public enum ThesisStatus
        {
            [Display(Name = "ausgeschrieben")]
            ausgeschrieben,
            [Display(Name = "angemeldet")]
            angemeldet,
            [Display(Name = "abgegeben")]
            abgegeben,
            [Display(Name = "bewertet")]
            bewertet,
            [Display(Name = "nicht abgeschlossen")]
            nicht_abgeschlossen
        }

        public enum ThesisType
        {
            [Display(Name = "Bachelor")]
            Bachelor,
            [Display(Name = "Master")]
            Master
        }

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Bitte geben Sie den Titel der Arbeit an.")]
        [Display(Name = "Titel")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Bitte geben Sie eine Beschreibung für die Arbeit an.")]
        [Display(Name = "Beschreibung")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Bitte wählen Sie den Status der Arbeit aus.")]
        [Display(Name = "Status")]
        public ThesisStatus Status { get; set; }

        [Required(ErrorMessage = "Bitte wählen Sie den Typ der Arbeit aus.")]
        [Display(Name = "Typ")]
        public ThesisType Type { get; set; }

        [Display(Name = "Startdatum")]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [Display(Name = "Enddatum")]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [Required]
        [Display(Name = "Letzte Änderung")]
        public DateTime LastModified { get; set; }

        // Foreign Key für Programme (Pflichtfeld)
        [Required(ErrorMessage = "Bitte wählen Sie einen Studiengang aus.")]
        [Display(Name = "Studiengang")]
        public int ProgrammeId { get; set; }
        [ForeignKey("ProgrammeId")]
        public Programme Programme { get; set; }

        // Foreign Key für Student (Optional)
        [Display(Name = "Student")]
        public int? StudentId { get; set; }
        [ForeignKey("StudentId")]
        public Student Student { get; set; }

        // Foreign Key für Supervisor (Pflichtfeld)
        [Required(ErrorMessage = "Bitte wählen Sie einen Betreuer aus.")]
        [Display(Name = "Betreuer")]
        public int SupervisorId { get; set; }
        [ForeignKey("SupervisorId")]
        public Supervisor Supervisor { get; set; }

        // Navigation Property: Ein Review pro Thesis (kann null sein, da nicht jede Thesis ein Review hat)
        public Review Review { get; set; }

        [Display(Name = "PDF Dateiname")] public string PdfFileName { get; set; } = "";
    }
}