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
        public DbSet<JobProjectManager> JobProjectManagers => Set<JobProjectManager>();
        //Distribution List
        public DbSet<DistributionList> DistributionLists => Set<DistributionList>();
        public DbSet<DistributionMember> DistributionMembers => Set<DistributionMember>();

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
            modelBuilder.Entity<PersonalInfo>().ToTable("PersonalInfos");
            modelBuilder.Entity<GeoPacificEmployee>().ToTable("GeoPacificEmployees");

            // ---------- People / Contractors ----------
            modelBuilder.Entity<Contractor>()
                .HasOne(c => c.Details)
                .WithMany()
                .HasForeignKey(c => c.DetailsId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------- Jobs ----------
            modelBuilder.Entity<JobContractor>()
                .HasKey(jc => new { jc.JobId, jc.ContractorId });

            modelBuilder.Entity<JobProjectManager>()
                .HasOne(jpm => jpm.Job)
                .WithMany(j => j.ProjectManagers)
                .HasForeignKey(jpm => jpm.JobId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<JobProjectManager>()
                .HasOne(jpm => jpm.Employee)
                .WithMany()
                .HasForeignKey(jpm => jpm.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

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

            // ---------- DensityTest ↔ ShotPlacement (optional 1:1) ----------
            modelBuilder.Entity<DensityTest>()
                .HasOne(d => d.ShotPlacement)
                .WithOne(s => s.DensityTest)
                .HasForeignKey<ShotPlacement>(s => s.DensityTestId)
                .IsRequired(false);

            modelBuilder.Entity<ShotPlacement>()
                .HasIndex(s => s.DensityTestId)
                .IsUnique()
                .HasFilter("\"DensityTestId\" IS NOT NULL");

            // Helpful indexes
            modelBuilder.Entity<DensityTest>().HasIndex(d => d.ReportId);
            modelBuilder.Entity<DensityTest>().HasIndex(d => d.ProctorId);
            modelBuilder.Entity<DistributionList>().HasIndex(dl => dl.JobId);
            modelBuilder.Entity<Report>().HasIndex(r => r.DistributionListId);

            // PostgreSQL check constraints (property names match your model)
            modelBuilder.Entity<DensityTest>().ToTable(t =>
            {
                t.HasCheckConstraint("ck_densitytest_moisture_0_100",
                    "\"MoistureValue\" IS NULL OR (\"MoistureValue\" >= 0 AND \"MoistureValue\" <= 100)");

                t.HasCheckConstraint("ck_densitytest_oversize_0_100",
                    "\"CorrectedOversizePercentage\" IS NULL OR (\"CorrectedOversizePercentage\" >= 0 AND \"CorrectedOversizePercentage\" <= 100)");

                t.HasCheckConstraint("ck_densitytest_compactionspec_0_110",
                    "\"CompactionSpecification\" IS NULL OR (\"CompactionSpecification\" >= 0 AND \"CompactionSpecification\" <= 110)");
            });

            // Default UTC timestamp for CreatedDate
            modelBuilder.Entity<DensityTest>()
                .Property(d => d.CreatedDate)
                .HasDefaultValueSql("timezone('utc', now())");

            // ---------- Lab ----------
            modelBuilder.Entity<Sieve>()
                .HasOne(s => s.LabTest)
                .WithMany(lt => lt.Sieves)
                .HasForeignKey(s => s.LabTestId);

            modelBuilder.Entity<SieveResult>()
                .HasOne(sr => sr.Sieve)
                .WithMany(s => s.Results)
                .HasForeignKey(sr => sr.SieveId);

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

            modelBuilder.Entity<ProctorAdditionalJob>()
                .HasOne(pj => pj.Proctor)
                .WithMany(p => p.AdditionalJobs)
                .HasForeignKey(pj => pj.ProctorId);

            modelBuilder.Entity<ProctorAdditionalJob>()
                .HasOne(pj => pj.Job)
                .WithMany(j => j.ProctorAdditionalJobs)
                .HasForeignKey(pj => pj.JobId);

            // ---------- Distribution Lists ----------
            modelBuilder.Entity<DistributionMember>()
                .HasOne(dm => dm.DistributionList)
                .WithMany(dl => dl.DistributionMembers)
                .HasForeignKey(dm => dm.DistributionListId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DistributionMember>()
                .HasOne(dm => dm.PersonalInfo)
                .WithMany()
                .HasForeignKey(dm => dm.PersonalInfoId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------- Jobs and Distribution Lists ----------
            modelBuilder.Entity<DistributionList>()
                .HasOne(dl => dl.Job)
                .WithMany(j => j.DistributionLists)
                .HasForeignKey(dl => dl.JobId)
                .OnDelete(DeleteBehavior.Cascade);

            // ---------- Reports and Distribution Lists ----------
            modelBuilder.Entity<Report>()
                .HasOne(r => r.DistributionList)
                .WithMany()
                .HasForeignKey(r => r.DistributionListId)
                .OnDelete(DeleteBehavior.SetNull);

            // Ensure the distribution list belongs to the same job as the report
            modelBuilder.Entity<Report>()
                .HasCheckConstraint("ck_report_distributionlist_same_job",
                    "\"DistributionListId\" IS NULL OR EXISTS (SELECT 1 FROM \"DistributionLists\" dl WHERE dl.\"Id\" = \"DistributionListId\" AND dl.\"JobId\" = \"JobId\")");
        }
    }
}
