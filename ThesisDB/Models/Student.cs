using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ThesisDB.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Bitte geben Sie den Vornamen an.")]
        [Display(Name = "Vorname")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Bitte geben Sie den Nachnamen an.")]
        [Display(Name = "Nachname")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Bitte geben Sie die E-Mail-Adresse an.")]
        [EmailAddress(ErrorMessage = "Ungültige E-Mail-Adresse.")]
        [Display(Name = "E-Mail")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Bitte geben Sie die Matrikelnummer an.")]
        [Display(Name = "Matrikelnummer")]
        public string MatriculationNumber { get; set; }

        // Navigation Property: Ein Student kann mehrere Abschlussarbeiten haben (1:n)
        public ICollection<Thesis> Theses { get; set; } = new List<Thesis>();
    }
}
