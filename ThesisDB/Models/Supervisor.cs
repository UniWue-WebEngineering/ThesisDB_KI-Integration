using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ThesisDB.Models
{
    public class Supervisor
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

        [Required(ErrorMessage = "Bitte geben Sie den Aktivitätsstatus an.")]
        [Display(Name = "aktiv")]
        public bool Active { get; set; } = true;

        // Navigation Property: Ein Betreuer kann mehrere Abschlussarbeiten betreuen (1:n)
        public ICollection<Thesis> Theses { get; set; } = new List<Thesis>();
    }
}
