using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ThesisDB.Models
{
    public static class DataSeeder
    {
        public static async Task SeedDataAsync(ThesisDbContext context, UserManager<ThesisDbUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Sicherstellen, dass die Datenbank existiert und die Migrationen angewendet wurden
            context.Database.Migrate();

            // 0. Rollen und Benutzer seeden
            string[] roleNames = { "Administrator", "Operator" };
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            var adminUser = await userManager.FindByNameAsync("admin");
            if (adminUser == null)
            {
                var user = new ThesisDbUser
                {
                    UserName = "admin",
                    Email = "admin@thesisdb.local",
                    FirstName = "Admin",
                    LastName = "User"
                };

                var result = await userManager.CreateAsync(user, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Administrator");
                }
            }

            // 1. Studiengänge (Programmes) seeden, falls noch keine existieren
            if (!context.Programmes.Any())
            {
                var programmes = new List<Programme>
                {
                    new Programme { Name = "B.Sc. Wirtschaftsinformatik" },
                    new Programme { Name = "B.Sc. Digital Business and Data Science" },
                    new Programme { Name = "M.Sc. Information Systems" },
                    new Programme { Name = "M.Sc. Management" }
                };

                context.Programmes.AddRange(programmes);
                context.SaveChanges();
            }

            // 2. Studenten seeden
            if (!context.Students.Any())
            {
                var students = new List<Student>
                {
                    new Student { FirstName = "Max", LastName = "Mustermann", Email = "max.mustermann@student.uni.de", MatriculationNumber = "123456" },
                    new Student { FirstName = "Erika", LastName = "Musterfrau", Email = "erika.musterfrau@student.uni.de", MatriculationNumber = "654321" },
                    new Student { FirstName = "Lukas", LastName = "Schmidt", Email = "lukas.schmidt@student.uni.de", MatriculationNumber = "112233" },
                    new Student { FirstName = "Anna", LastName = "Müller", Email = "anna.mueller@student.uni.de", MatriculationNumber = "332211" },
                    new Student { FirstName = "Felix", LastName = "Weber", Email = "felix.weber@student.uni.de", MatriculationNumber = "445566" }
                };
                context.Students.AddRange(students);
                context.SaveChanges();
            }

            // 3. Supervisoren seeden
            if (!context.Supervisors.Any())
            {
                var supervisors = new List<Supervisor>
                {
                    new Supervisor { FirstName = "Hans", LastName = "Meier", Email = "hans.meier@uni.de", Active = true },
                    new Supervisor { FirstName = "Julia", LastName = "Wagner", Email = "julia.wagner@uni.de", Active = true },
                    new Supervisor { FirstName = "Klaus", LastName = "Becker", Email = "klaus.becker@uni.de", Active = false }
                };
                context.Supervisors.AddRange(supervisors);
                context.SaveChanges();
            }

            // 4. Theses seeden, falls noch keine existieren
            if (!context.Theses.Any())
            {
                var programmes = context.Programmes.ToList();
                var students = context.Students.ToList();
                var supervisors = context.Supervisors.ToList();
                var random = new Random();

                var theses = new List<Thesis>
                {
                    new Thesis
                    {
                        Title = "Analyse von Blockchain-Technologien im Supply-Chain-Management",
                        Description = "Untersuchung der Einsatzmöglichkeiten und Potenziale von Blockchain zur Optimierung von Lieferketten.",
                        Status = Thesis.ThesisStatus.ausgeschrieben,
                        Type = Thesis.ThesisType.Master,
                        LastModified = DateTime.UtcNow,
                        ProgrammeId = programmes[random.Next(programmes.Count)].Id,
                        StudentId = students[random.Next(students.Count)].Id,
                        SupervisorId = supervisors[random.Next(supervisors.Count)].Id
                    },
                    new Thesis
                    {
                        Title = "Entwicklung eines KI-basierten Chatbots für den Kundenservice",
                        Description = "Konzeption und prototypische Implementierung eines Chatbots zur Automatisierung von Kundenanfragen.",
                        Status = Thesis.ThesisStatus.angemeldet,
                        Type = Thesis.ThesisType.Bachelor,
                        StartDate = new DateTime(2023, 10, 15),
                        LastModified = DateTime.UtcNow,
                        ProgrammeId = programmes[random.Next(programmes.Count)].Id,
                        StudentId = students[random.Next(students.Count)].Id,
                        SupervisorId = supervisors[random.Next(supervisors.Count)].Id
                    },
                    new Thesis
                    {
                        Title = "Cloud-Migration-Strategien für KMU",
                        Description = "Vergleich und Bewertung von Strategien für die Migration von IT-Infrastrukturen kleiner und mittlerer Unternehmen in die Cloud.",
                        Status = Thesis.ThesisStatus.abgegeben,
                        Type = Thesis.ThesisType.Bachelor,
                        StartDate = new DateTime(2023, 04, 01),
                        EndDate = new DateTime(2023, 09, 30),
                        LastModified = DateTime.UtcNow,
                        ProgrammeId = programmes[random.Next(programmes.Count)].Id,
                        StudentId = students[random.Next(students.Count)].Id,
                        SupervisorId = supervisors[random.Next(supervisors.Count)].Id
                    },
                    new Thesis
                    {
                        Title = "Big Data Analytics im E-Commerce zur Personalisierung von Kundenempfehlungen",
                        Description = "Analyse von Nutzerdaten zur Verbesserung von Produktempfehlungen in Online-Shops.",
                        Status = Thesis.ThesisStatus.bewertet,
                        Type = Thesis.ThesisType.Master,
                        StartDate = new DateTime(2022, 11, 02),
                        EndDate = new DateTime(2023, 05, 01),
                        LastModified = DateTime.UtcNow,
                        ProgrammeId = programmes[random.Next(programmes.Count)].Id,
                        StudentId = students[random.Next(students.Count)].Id,
                        SupervisorId = supervisors[random.Next(supervisors.Count)].Id
                    },
                    new Thesis
                    {
                        Title = "IT-Sicherheitskonzepte für mobile Anwendungen im Finanzsektor",
                        Description = "Erarbeitung eines umfassenden Sicherheitskonzepts für eine Banking-App.",
                        Status = Thesis.ThesisStatus.ausgeschrieben,
                        Type = Thesis.ThesisType.Master,
                        LastModified = DateTime.UtcNow,
                        ProgrammeId = programmes[random.Next(programmes.Count)].Id,
                        StudentId = students[random.Next(students.Count)].Id,
                        SupervisorId = supervisors[random.Next(supervisors.Count)].Id
                    },
                    new Thesis
                    {
                        Title = "Prozessoptimierung durch Robotic Process Automation (RPA) in der Verwaltung",
                        Description = "Identifikation und Automatisierung von repetitiven Verwaltungsprozessen mittels RPA.",
                        Status = Thesis.ThesisStatus.angemeldet,
                        Type = Thesis.ThesisType.Bachelor,
                        StartDate = new DateTime(2023, 11, 01),
                        LastModified = DateTime.UtcNow,
                        ProgrammeId = programmes[random.Next(programmes.Count)].Id,
                        StudentId = students[random.Next(students.Count)].Id,
                        SupervisorId = supervisors[random.Next(supervisors.Count)].Id
                    },
                    new Thesis
                    {
                        Title = "Agiles Projektmanagement in der Softwareentwicklung: Eine kritische Analyse",
                        Description = "Vergleich von Scrum und Kanban in der Praxis und Ableitung von Handlungsempfehlungen.",
                        Status = Thesis.ThesisStatus.nicht_abgeschlossen,
                        Type = Thesis.ThesisType.Bachelor,
                        StartDate = new DateTime(2022, 08, 01),
                        EndDate = new DateTime(2023, 02, 15),
                        LastModified = DateTime.UtcNow,
                        ProgrammeId = programmes[random.Next(programmes.Count)].Id,
                        StudentId = students[random.Next(students.Count)].Id,
                        SupervisorId = supervisors[random.Next(supervisors.Count)].Id
                    },
                    new Thesis
                    {
                        Title = "Digital-Twin-Konzepte für die Industrie 4.0",
                        Description = "Untersuchung, wie digitale Zwillinge zur Überwachung und Steuerung von Produktionsanlagen eingesetzt werden können.",
                        Status = Thesis.ThesisStatus.ausgeschrieben,
                        Type = Thesis.ThesisType.Master,
                        LastModified = DateTime.UtcNow,
                        ProgrammeId = programmes[random.Next(programmes.Count)].Id,
                        StudentId = students[random.Next(students.Count)].Id,
                        SupervisorId = supervisors[random.Next(supervisors.Count)].Id
                    },
                    new Thesis
                    {
                        Title = "Customer-Relationship-Management mit Salesforce",
                        Description = "Einführung und Anpassung von Salesforce zur Verbesserung der Kundenbeziehungen in einem mittelständischen Unternehmen.",
                        Status = Thesis.ThesisStatus.abgegeben,
                        Type = Thesis.ThesisType.Bachelor,
                        StartDate = new DateTime(2023, 05, 10),
                        EndDate = new DateTime(2023, 11, 09),
                        LastModified = DateTime.UtcNow,
                        ProgrammeId = programmes[random.Next(programmes.Count)].Id,
                        StudentId = students[random.Next(students.Count)].Id,
                        SupervisorId = supervisors[random.Next(supervisors.Count)].Id
                    },
                    new Thesis
                    {
                        Title = "Einsatz von maschinellem Lernen zur Vorhersage von Aktienkursen",
                        Description = "Entwicklung eines Prognosemodells auf Basis historischer Kursdaten und neuronaler Netze.",
                        Status = Thesis.ThesisStatus.angemeldet,
                        Type = Thesis.ThesisType.Master,
                        StartDate = new DateTime(2023, 09, 01),
                        LastModified = DateTime.UtcNow,
                        ProgrammeId = programmes[random.Next(programmes.Count)].Id,
                        StudentId = students[random.Next(students.Count)].Id,
                        SupervisorId = supervisors[random.Next(supervisors.Count)].Id
                    }
                };

                // Add 30 additional theses from economics without a student assigned
                string[] ecoTitles = new string[] {
                    "Auswirkungen der Inflation auf das Konsumverhalten", "Markteintrittsstrategien für Schwellenländer",
                    "Rolle von ESG-Kriterien bei Investitionsentscheidungen", "Einfluss der Digitalisierung auf das Personalmanagement",
                    "Wirtschaftliche Folgen des demografischen Wandels", "Bedeutung von Kryptowährungen im internationalen Zahlungsverkehr",
                    "Erfolgsfaktoren von Start-ups im FinTech-Sektor", "Analyse von Preisbildungsstrategien im E-Commerce",
                    "Auswirkungen von Remote Work auf die Produktivität", "Bewertung von Nachhaltigkeitsberichterstattung in DAX-Unternehmen",
                    "Rolle der künstlichen Intelligenz in der Finanzanalyse", "Einfluss von Social Media auf das Markenimage",
                    "Wirtschaftliche Effekte von Freihandelsabkommen", "Strategien zur Mitarbeiterbindung in Zeiten des Fachkräftemangels",
                    "Bedeutung von Corporate Social Responsibility für den Unternehmenserfolg", "Analyse von Crowdfunding als alternative Finanzierungsform",
                    "Auswirkungen der Geldpolitik auf den Immobilienmarkt", "Erfolgsfaktoren im internationalen Projektmanagement",
                    "Rolle des Influencer-Marketings in der Modebranche", "Einfluss der Sharing Economy auf traditionelle Geschäftsmodelle",
                    "Wirtschaftliche Folgen von Pandemien auf globale Lieferketten", "Strategien zur Krisenbewältigung in der Tourismusbranche",
                    "Bedeutung von Diversity Management für Innovation", "Analyse von Kundenbindungsprogrammen im Einzelhandel",
                    "Auswirkungen der CO2-Bepreisung auf die Industrie", "Erfolgsfaktoren von Familienunternehmen beim Generationswechsel",
                    "Rolle von Big Data im strategischen Marketing", "Einfluss der Automatisierung auf den Arbeitsmarkt",
                    "Wirtschaftliche Effekte von staatlichen Subventionen auf erneuerbare Energien", "Strategien zur Optimierung des Working Capital Managements"
                };

                foreach(var title in ecoTitles)
                {
                    theses.Add(new Thesis
                    {
                        Title = title,
                        Description = "Eine detaillierte wirtschaftswissenschaftliche Untersuchung.",
                        Status = Thesis.ThesisStatus.ausgeschrieben,
                        Type = Thesis.ThesisType.Bachelor,
                        LastModified = DateTime.UtcNow,
                        ProgrammeId = programmes[random.Next(programmes.Count)].Id,
                        StudentId = null, // Kein Student zugeordnet
                        SupervisorId = supervisors[random.Next(supervisors.Count)].Id
                    });
                }

                context.Theses.AddRange(theses);
                context.SaveChanges();
            }

            // 5. Reviews seeden (nur für bewertete Thesen)
            if (!context.Reviews.Any())
            {
                var bewerteteThesen = context.Theses.Where(t => t.Status == Thesis.ThesisStatus.bewertet).ToList();
                var reviews = new List<Review>();

                foreach (var thesis in bewerteteThesen)
                {
                    reviews.Add(new Review
                    {
                        Summary = "Sehr detaillierte und aufschlussreiche Arbeit.",
                        Strengths = "Klare Struktur, sehr gute Methodik.",
                        Weaknesses = "Einige Tippfehler im Literaturverzeichnis.",
                        Evaluation = "Insgesamt eine hervorragende Leistung, die den Anforderungen voll entspricht.",
                        ContentVal = 5,
                        LayoutVal = 4,
                        StructureVal = 5,
                        StyleVal = 4,
                        LiteratureVal = 5,
                        DifficultyVal = 4,
                        NoveltyVal = 5,
                        RichnessVal = 5,
                        Grade = 1.3,
                        ThesisId = thesis.Id
                    });
                }

                if (reviews.Any())
                {
                    context.Reviews.AddRange(reviews);
                    context.SaveChanges();
                }
            }
        }
        
        // Kompatibilitätsmethode für den Fall, dass noch irgendwo die alte Version aufgerufen wird
        public static void SeedData(ThesisDbContext context)
        {
            // Dummy implementation
            // Wir verwenden jetzt nur noch SeedDataAsync
        }
    }
}