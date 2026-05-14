using Microsoft.EntityFrameworkCore;

namespace ThesisDB.Models
{
    public class ThesisDbContext : DbContext
    {
        public ThesisDbContext(DbContextOptions<ThesisDbContext> options) : base(options)
        {
        }

        public DbSet<Thesis> Theses { get; set; }
        public DbSet<Programme> Programmes { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Supervisor> Supervisors { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Thesis>()
                .Property(t => t.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Thesis>()
                .Property(t => t.Type)
                .HasConversion<string>();

            // Konfiguration der 1:n-Beziehung zwischen Programme und Thesis
            modelBuilder.Entity<Thesis>()
                .HasOne(t => t.Programme)
                .WithMany(p => p.Theses)
                .HasForeignKey(t => t.ProgrammeId)
                .OnDelete(DeleteBehavior.Restrict); // Verhindert das Löschen eines Studiengangs, wenn noch Arbeiten zugeordnet sind

            // Konfiguration der optionalen 1:n-Beziehung zwischen Student und Thesis
            modelBuilder.Entity<Thesis>()
                .HasOne(t => t.Student)
                .WithMany(s => s.Theses)
                .HasForeignKey(t => t.StudentId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            // Konfiguration der 1:n-Beziehung zwischen Supervisor und Thesis
            modelBuilder.Entity<Thesis>()
                .HasOne(t => t.Supervisor)
                .WithMany(s => s.Theses)
                .HasForeignKey(t => t.SupervisorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Konfiguration der 1:1-Beziehung zwischen Review und Thesis
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Thesis)
                .WithOne(t => t.Review)
                .HasForeignKey<Review>(r => r.ThesisId)
                .OnDelete(DeleteBehavior.Cascade); // Wenn Thesis gelöscht wird, wird auch das Review gelöscht
        }
    }
}
