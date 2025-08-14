using Microsoft.EntityFrameworkCore;
using DensityReportingToolBackend.Models;

namespace DensityReportingToolBackend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // People
        public DbSet<PersonalInfo> PersonalInfos => Set<PersonalInfo>();
        public DbSet<GeoPacificEmployee> GeoPacificEmployees => Set<GeoPacificEmployee>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Contractor> Contractors => Set<Contractor>();

        // Jobs
        public DbSet<Job> Jobs => Set<Job>();
        public DbSet<JobContractor> JobContractors => Set<JobContractor>();
        public DbSet<JobNote> JobNotes => Set<JobNote>();
        public DbSet<SitePlan> SitePlans => Set<SitePlan>();

        // Reports
        public DbSet<Report> Reports => Set<Report>();
        public DbSet<ReportPhoto> ReportPhotos => Set<ReportPhoto>();
        public DbSet<ReportMemo> ReportMemos => Set<ReportMemo>();

        // Comments
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<MemoComment> MemoComments => Set<MemoComment>();
        public DbSet<DensityTestComment> DensityTestComments => Set<DensityTestComment>();

        // Density
        public DbSet<DensityTest> DensityTests => Set<DensityTest>();
        public DbSet<ShotPlacement> ShotPlacements => Set<ShotPlacement>();

        // Lab
        public DbSet<LabTest> LabTests => Set<LabTest>();
        public DbSet<Sieve> Sieves => Set<Sieve>();
        public DbSet<SieveResult> SieveResults => Set<SieveResult>();
        public DbSet<Proctor> Proctors => Set<Proctor>();
        public DbSet<ProctorType> ProctorTypes => Set<ProctorType>();
        public DbSet<ProctorAdditionalJob> ProctorAdditionalJobs => Set<ProctorAdditionalJob>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ---------- People / Contractors ----------
            // Contractor has a required 1→1 to PersonalInfo via DetailsId
            modelBuilder.Entity<Contractor>()
                .HasOne(c => c.Details)
                .WithMany()
                .HasForeignKey(c => c.DetailsId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------- Jobs ----------
            // Explicit join for Job ↔ Contractor
            modelBuilder.Entity<JobContractor>()
                .HasKey(jc => new { jc.JobId, jc.ContractorId });

            modelBuilder.Entity<JobContractor>()
                .HasOne(jc => jc.Job)
                .WithMany(j => j.JobContracts)
                .HasForeignKey(jc => jc.JobId);

            modelBuilder.Entity<JobContractor>()
                .HasOne(jc => jc.Contractor)
                .WithMany(c => c.JobContracts)
                .HasForeignKey(jc => jc.ContractorId);

            // Job ↔ JobNote (join to Comment)
            modelBuilder.Entity<JobNote>(b =>
            {
                b.HasOne(jn => jn.Job)
                    .WithMany(j => j.JobNotes)
                    .HasForeignKey(jn => jn.JobId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(jn => jn.Comment)
                    .WithMany(c => c.JobNotes)
                    .HasForeignKey(jn => jn.CommentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ---------- Reports / Comments ----------
            modelBuilder.Entity<MemoComment>(b =>
            {
                b.HasOne(mc => mc.Memo)
                    .WithMany(m => m.Comments)
                    .HasForeignKey(mc => mc.MemoId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(mc => mc.Comment)
                    .WithMany(c => c.MemoComments)
                    .HasForeignKey(mc => mc.CommentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<DensityTestComment>(b =>
            {
                b.HasOne(dc => dc.DensityTest)
                    .WithMany(dt => dt.Comments)
                    .HasForeignKey(dc => dc.DensityTestId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(dc => dc.Comment)
                    .WithMany(c => c.DensityTestComments)
                    .HasForeignKey(dc => dc.CommentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ---------- DensityTest ↔ ShotPlacement (strict 1:1) ----------
            // ShotPlacement holds the FK to DensityTest and must be unique
            modelBuilder.Entity<DensityTest>()
                .HasOne(d => d.ShotPlacement)
                .WithOne(s => s.DensityTest)
                .HasForeignKey<ShotPlacement>(s => s.DensityTestId)
                .IsRequired();

            modelBuilder.Entity<ShotPlacement>()
                .HasIndex(s => s.DensityTestId)
                .IsUnique();

            // If your DensityTest class still has a ShotPlacementId property, ignore it (redundant)
            modelBuilder.Entity<DensityTest>()
                .Ignore(d => d.ShotPlacementId);

            // ---------- Lab / Sieve / Proctor ----------
            modelBuilder.Entity<Sieve>()
                .HasOne(s => s.LabTest)
                .WithMany(lt => lt.Sieves)
                .HasForeignKey(s => s.LabTestId);

            modelBuilder.Entity<SieveResult>()
                .HasOne(sr => sr.Sieve)
                .WithMany(s => s.Results)
                .HasForeignKey(sr => sr.SieveId);

            // Proctor optional link to Sieve
            modelBuilder.Entity<Proctor>()
                .HasOne(p => p.Sieve)
                .WithMany(s => s.Proctors)
                .HasForeignKey(p => p.SieveId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Proctor>()
                .HasOne(p => p.LabTest)
                .WithMany(lt => lt.Proctors)
                .HasForeignKey(p => p.LabTestId);

            modelBuilder.Entity<Proctor>()
                .HasOne(p => p.ProctorType)
                .WithMany(pt => pt.Proctors)
                .HasForeignKey(p => p.ProctorTypeId);

            // Join: Proctor ↔ Job (extra usage)
            modelBuilder.Entity<ProctorAdditionalJob>()
                .HasOne(pj => pj.Proctor)
                .WithMany(p => p.AdditionalJobs)
                .HasForeignKey(pj => pj.ProctorId);

            modelBuilder.Entity<ProctorAdditionalJob>()
                .HasOne(pj => pj.Job)
                .WithMany(j => j.ProctorAdditionalJobs)
                .HasForeignKey(pj => pj.JobId);
        }
    }
}
