using FutureOfWork.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FutureOfWork.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Job> Jobs { get; set; }
    public DbSet<Candidate> Candidates { get; set; }
    public DbSet<Skill> Skills { get; set; }
    public DbSet<JobSkill> JobSkills { get; set; }
    public DbSet<CandidateSkill> CandidateSkills { get; set; }
    public DbSet<Application> Applications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Job configuration
        modelBuilder.Entity<Job>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).IsRequired();
            entity.Property(e => e.Company).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Location).HasMaxLength(200);
            entity.Property(e => e.EmploymentType).HasMaxLength(50);
            entity.Property(e => e.SalaryMin).HasPrecision(18, 2);
            entity.Property(e => e.SalaryMax).HasPrecision(18, 2);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.HasIndex(e => e.Title);
            entity.HasIndex(e => e.Company);
        });

        // Candidate configuration
        modelBuilder.Entity<Candidate>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).IsRequired();
        });

        // Skill configuration
        modelBuilder.Entity<Skill>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Name).IsUnique();
            entity.Property(e => e.Category).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.DemandScore).HasDefaultValue(0);
        });

        // JobSkill configuration (many-to-many)
        modelBuilder.Entity<JobSkill>(entity =>
        {
            entity.HasKey(e => new { e.JobId, e.SkillId });
            entity.HasOne(e => e.Job)
                .WithMany(j => j.JobSkills)
                .HasForeignKey(e => e.JobId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Skill)
                .WithMany(s => s.JobSkills)
                .HasForeignKey(e => e.SkillId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.Property(e => e.Level).HasMaxLength(50);
        });

        // CandidateSkill configuration (many-to-many)
        modelBuilder.Entity<CandidateSkill>(entity =>
        {
            entity.HasKey(e => new { e.CandidateId, e.SkillId });
            entity.HasOne(e => e.Candidate)
                .WithMany(c => c.CandidateSkills)
                .HasForeignKey(e => e.CandidateId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Skill)
                .WithMany(s => s.CandidateSkills)
                .HasForeignKey(e => e.SkillId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.Property(e => e.Level).HasMaxLength(50);
        });

        // Application configuration
        modelBuilder.Entity<Application>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Job)
                .WithMany(j => j.Applications)
                .HasForeignKey(e => e.JobId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Candidate)
                .WithMany(c => c.Applications)
                .HasForeignKey(e => e.CandidateId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.AppliedAt).IsRequired();
            entity.HasIndex(e => new { e.JobId, e.CandidateId }).IsUnique();
        });
    }
}

