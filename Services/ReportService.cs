using DensityReportingToolBackend.Data;
using DensityReportingToolBackend.Models;
using DensityReportingToolBackend.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace DensityReportingToolBackend.Services
{
    public interface IReportService
    {
        Task<ReportCreateResponse> CreateReportAsync(CreateReportRequest request);
        Task<ReportDetailResponse?> GetReportAsync(int reportId);
        Task<IEnumerable<ReportListByJobResponse>> GetAllReportsAsync();
        Task<IEnumerable<ReportListByJobResponse>> GetReportsByJobAsync(string jobNumber);
        Task<IEnumerable<ReportListByJobResponse>> SearchReportsByJobNumberAsync(string jobNumber, int limit = 10);
        Task<IEnumerable<ReportProctorDataResponse>> GetProctorsForJobAsync(int jobId);
        Task<DensityTestCreateResponse> CreateDensityTestAsync(int reportId, CreateDensityTestRequest request);
        Task<MemoInfo?> UpdateMemoAsync(int reportId, UpdateMemoRequest request);
    }

    /// <summary>
    /// Service for managing Report operations including CRUD operations,
    /// density test creation, and report coordination.
    /// </summary>
    public class ReportService(AppDbContext dbContext) : IReportService
    {
        private readonly AppDbContext _dbContext = dbContext;

        public async Task<ReportCreateResponse> CreateReportAsync(CreateReportRequest request)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            
            try
            {
                // 1. Validate and find job
                var job = await FindJobAsync(request.JobId);
                
                // 2. Validate and find employee
                var employee = await FindEmployeeAsync(request.EmployeeId);
                
                // 3. Validate and find reviewer (if provided)
                PersonalInfo? reviewer = null;
                if (request.ReviewerId.HasValue)
                {
                    reviewer = await FindReviewerAsync(request.ReviewerId.Value);
                }
                
                // 4. Get next report number for this job
                var nextReportNumber = await GetNextReportNumberAsync(request.JobId);
                
                // 5. Create new report
                var newReport = await CreateReportEntityAsync(request, nextReportNumber);
                
                // 6. Create memo if provided
                if (request.Memo != null)
                {
                    await CreateReportMemoAsync(newReport.Id, request.Memo);
                }
                
                await transaction.CommitAsync();

                return MapToCreateResponse(newReport, job, employee, reviewer);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<ReportListByJobResponse>> GetReportsByJobAsync(string jobNumber)
        {
            var reports = await _dbContext.Reports
                .Include(r => r.Job)
                .Include(r => r.DensityTests)
                .Include(r => r.Photos)
                .Include(r => r.Memos)
                .Where(r => r.Job.JobNumber == jobNumber)
                .OrderByDescending(r => r.ReportNumber) // Newest reports first
                .ToListAsync();

            var result = new List<ReportListByJobResponse>();

            foreach (var report in reports)
            {
                // Get employee info
                var employee = await _dbContext.PersonalInfos
                    .FirstOrDefaultAsync(p => p.Id == report.EmployeeId && p.Company == "GeoPacific");

                // Get reviewer info (if assigned)
                PersonalInfo? reviewer = null;
                if (report.ReviewerId > 0)
                {
                    reviewer = await _dbContext.PersonalInfos
                        .FirstOrDefaultAsync(p => p.Id == report.ReviewerId && p.Company == "GeoPacific");
                }

                result.Add(MapToListByJobResponse(report, employee, reviewer));
            }

            return result;
        }

        public async Task<ReportDetailResponse?> GetReportAsync(int reportId)
        {
            var report = await _dbContext.Reports
                .Include(r => r.Job)
                .Include(r => r.Employee)
                .Include(r => r.Reviewer)
                .Include(r => r.DensityTests)
                    .ThenInclude(dt => dt.Proctor)
                .Include(r => r.Photos)
                .Include(r => r.Memos)
                .FirstOrDefaultAsync(r => r.Id == reportId);

            if (report == null)
            {
                return null;
            }

            return MapToDetailResponse(report);
        }

        #region Private Helper Methods

        private async Task<Job> FindJobAsync(int jobId)
        {
            var job = await _dbContext.Jobs.FindAsync(jobId);
            
            if (job == null)
            {
                throw new ArgumentException($"Job with ID {jobId} not found");
            }
            
            return job;
        }

        private async Task<PersonalInfo> FindEmployeeAsync(int employeeId)
        {
            var employee = await _dbContext.PersonalInfos
                .FirstOrDefaultAsync(e => e.Id == employeeId && e.Company == "GeoPacific");
                
            if (employee == null)
            {
                throw new ArgumentException($"GeoPacific employee with ID {employeeId} not found");
            }
            
            return employee;
        }

        private async Task<PersonalInfo> FindReviewerAsync(int reviewerId)
        {
            var reviewer = await _dbContext.PersonalInfos
                .FirstOrDefaultAsync(e => e.Id == reviewerId && e.Company == "GeoPacific");
                
            if (reviewer == null)
            {
                throw new ArgumentException($"GeoPacific reviewer with ID {reviewerId} not found");
            }
            
            return reviewer;
        }

        private async Task<int> GetNextReportNumberAsync(int jobId)
        {
            var maxReportNumber = await _dbContext.Reports
                .Where(r => r.JobId == jobId)
                .MaxAsync(r => (int?)r.ReportNumber) ?? 0;
                
            var nextNumber = maxReportNumber + 1;
            
            return nextNumber;
        }

        private async Task<Report> CreateReportEntityAsync(CreateReportRequest request, int reportNumber)
        {
            var newReport = new Report
            {
                JobId = request.JobId,
                EmployeeId = request.EmployeeId,
                ReviewerId = request.ReviewerId, // Now properly nullable
                ReportNumber = reportNumber,
                StartDate = request.StartDate ?? DateTime.UtcNow,
                SubmitDate = request.SubmitDate,
                DistributeDate = request.DistributeDate,
                DistributionListId = request.DistributionListId
            };
            
            _dbContext.Reports.Add(newReport);
            await _dbContext.SaveChangesAsync();
            
            return newReport;
        }

        private async Task CreateReportMemoAsync(int reportId, ReportMemoRequest memoRequest)
        {
            var memo = new ReportMemo
            {
                ReportId = reportId,
                Purpose = memoRequest.Purpose,
                CommentsAndObservations = memoRequest.CommentsAndObservations,
                Conclusion = memoRequest.Conclusion,
                CreatedDate = DateTime.UtcNow
            };

            _dbContext.ReportMemos.Add(memo);
            await _dbContext.SaveChangesAsync();
        }

        private static ReportCreateResponse MapToCreateResponse(
            Report report, 
            Job job, 
            PersonalInfo employee, 
            PersonalInfo? reviewer)
        {
            return new ReportCreateResponse
            {
                Id = report.Id.ToString(),
                Message = "Report created successfully",
                Report = new ReportDataResponse
                {
                    Id = report.Id,
                    JobId = report.JobId,
                    ReportNumber = report.ReportNumber,
                    StartDate = report.StartDate,
                    SubmitDate = report.SubmitDate,
                    DistributeDate = report.DistributeDate,
                    Employee = new EmployeeInfo
                    {
                        Id = employee.Id,
                        FirstName = employee.FirstName,
                        LastName = employee.LastName,
                        Email = employee.Email,
                        PhoneNumber = employee.PhoneNumber
                    },
                    Reviewer = reviewer != null ? new EmployeeInfo
                    {
                        Id = reviewer.Id,
                        FirstName = reviewer.FirstName,
                        LastName = reviewer.LastName,
                        Email = reviewer.Email,
                        PhoneNumber = reviewer.PhoneNumber
                    } : new EmployeeInfo(), // Empty reviewer info if none assigned
                    Job = new JobInfo
                    {
                        Id = job.Id,
                        JobNumber = job.JobNumber,
                        ClientName = job.ClientName,
                        ProjectName = job.ProjectName
                    },
                    DistributionListId = report.DistributionListId
                }
            };
        }

        private static ReportListByJobResponse MapToListByJobResponse(
            Report report, 
            PersonalInfo? employee, 
            PersonalInfo? reviewer)
        {
            return new ReportListByJobResponse
            {
                Id = report.Id,
                JobId = report.JobId,
                Job = new JobInfo
                {
                    Id = report.Job.Id,
                    JobNumber = report.Job.JobNumber,
                    ClientName = report.Job.ClientName,
                    ProjectName = report.Job.ProjectName
                },
                ReportNumber = report.ReportNumber,
                StartDate = report.StartDate,
                SubmitDate = report.SubmitDate,
                DistributeDate = report.DistributeDate,
                Employee = employee != null ? new EmployeeInfo
                {
                    Id = employee.Id,
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    Email = employee.Email,
                    PhoneNumber = employee.PhoneNumber
                } : new EmployeeInfo(),
                Reviewer = reviewer != null ? new EmployeeInfo
                {
                    Id = reviewer.Id,
                    FirstName = reviewer.FirstName,
                    LastName = reviewer.LastName,
                    Email = reviewer.Email,
                    PhoneNumber = reviewer.PhoneNumber
                } : new EmployeeInfo(),
                DensityTestsCount = report.DensityTests.Count,
                PhotosCount = report.Photos.Count,
                MemosCount = report.Memos.Count,
                DistributionListId = report.DistributionListId
            };
        }

        private static ReportDetailResponse MapToDetailResponse(Report report)
        {
            return new ReportDetailResponse
            {
                Id = report.Id,
                JobId = report.JobId,
                Job = new JobInfo
                {
                    Id = report.Job.Id,
                    JobNumber = report.Job.JobNumber,
                    ClientName = report.Job.ClientName,
                    ProjectName = report.Job.ProjectName
                },
                ReportNumber = report.ReportNumber,
                StartDate = report.StartDate,
                SubmitDate = report.SubmitDate,
                DistributeDate = report.DistributeDate,
                Employee = new EmployeeInfo
                {
                    Id = report.Employee.Id,
                    FirstName = report.Employee.FirstName,
                    LastName = report.Employee.LastName,
                    Email = report.Employee.Email,
                    PhoneNumber = report.Employee.PhoneNumber
                },
                Reviewer = report.Reviewer != null ? new EmployeeInfo
                {
                    Id = report.Reviewer.Id,
                    FirstName = report.Reviewer.FirstName,
                    LastName = report.Reviewer.LastName,
                    Email = report.Reviewer.Email,
                    PhoneNumber = report.Reviewer.PhoneNumber
                } : null,
                DensityTests = report.DensityTests.OrderBy(dt => dt.TestNumber == 0 ? dt.Id : dt.TestNumber).Select(dt => new DensityTestInfo
                {
                    Id = dt.Id,
                    TestNumber = dt.TestNumber,
                    TestArea = dt.TestArea,
                    Location = dt.Location,
                    ElevationReference = dt.ElevationReference.ToString(),
                    ElevationValue = dt.ElevationValue ?? 0,
                    ElevationUnit = dt.ElevationUnit.ToString(),
                    CompactionSpecification = dt.CompactionSpecification ?? 0,
                    CompactionSpecificationUnit = dt.CompactionSpecificationUnit.ToString(),
                    DensityValue = dt.DensityValue ?? 0,
                    MoistureValue = dt.MoistureValue ?? 0,
                    CreatedDate = dt.CreatedDate ?? DateTime.UtcNow,
                    CompactionPercentage = dt.DensityValue.HasValue && dt.Proctor?.CorrectedDensity > 0
                        ? Math.Round(dt.DensityValue.Value / dt.Proctor.CorrectedDensity!.Value * 100, 1)
                        : 0,
                    Passed = dt.DensityValue.HasValue && dt.Proctor?.CorrectedDensity > 0 && dt.CompactionSpecification.HasValue
                        && dt.DensityValue.Value / dt.Proctor.CorrectedDensity!.Value * 100 >= dt.CompactionSpecification.Value
                }),
                Photos = report.Photos.Select(p => new PhotoInfo
                {
                    Id = p.Id,
                    Code = p.Code,
                    Url = p.Url,
                    Description = p.Description,
                    Latitude = p.Latitude,
                    Longitude = p.Longitude,
                    GpsAccuracyMeters = p.GpsAccuracyMeters
                }),
                Memos = report.Memos.Select(m => new MemoInfo
                {
                    Id = m.Id,
                    Purpose = m.Purpose,
                    CommentsAndObservations = m.CommentsAndObservations,
                    Conclusion = m.Conclusion,
                    CreatedDate = m.CreatedDate ?? DateTime.UtcNow,
                    UpdatedDate = m.UpdatedDate
                }),
                DistributionListId = report.DistributionListId
            };
        }

        #endregion

        public async Task<IEnumerable<ReportListByJobResponse>> GetAllReportsAsync()
        {
            var reports = await _dbContext.Reports
                .Include(r => r.Job)
                .Include(r => r.DensityTests)
                .Include(r => r.Photos)
                .Include(r => r.Memos)
                .OrderByDescending(r => r.StartDate) // Newest reports first
                .ToListAsync();

            var result = new List<ReportListByJobResponse>();

            foreach (var report in reports)
            {
                // Get employee info
                var employee = await _dbContext.PersonalInfos
                    .FirstOrDefaultAsync(p => p.Id == report.EmployeeId && p.Company == "GeoPacific");

                // Get reviewer info (if assigned)
                PersonalInfo? reviewer = null;
                if (report.ReviewerId > 0)
                {
                    reviewer = await _dbContext.PersonalInfos
                        .FirstOrDefaultAsync(p => p.Id == report.ReviewerId && p.Company == "GeoPacific");
                }

                result.Add(MapToListByJobResponse(report, employee, reviewer));
            }

            return result;
        }

        public async Task<IEnumerable<ReportProctorDataResponse>> GetProctorsForJobAsync(int jobId)
        {
            var proctors = await _dbContext.Proctors
                .Include(p => p.ProctorType)
                .Include(p => p.LabTest)
                .Where(p => p.LabTest.JobId == jobId)
                .Select(p => new ReportProctorDataResponse
                {
                    Id = p.Id,
                    ProctorID = p.ProctorID ?? string.Empty,
                    MaxDensity = p.MaxDensity,
                    CorrectedDensity = p.CorrectedDensity,
                    OptimumMoistureContent = p.OptimumMoistureContent,
                    SpecificGravity = p.SpecificGravity,
                    ProctorType = p.ProctorType.Type,
                    MaterialType = p.LabTest.MaterialType
                })
                .ToListAsync();

            return proctors;
        }

        public async Task<DensityTestCreateResponse> CreateDensityTestAsync(int reportId, CreateDensityTestRequest request)
        {
            // Verify report exists
            var report = await _dbContext.Reports.FindAsync(reportId);
            if (report == null)
            {
                throw new ArgumentException($"Report with ID {reportId} not found");
            }

            // Compute next test number for this job (across all its reports)
            var nextTestNumber = await _dbContext.DensityTests
                .Where(dt => dt.Report.JobId == report.JobId)
                .CountAsync() + 1;

            // Create density test
            var densityTest = new DensityTest
            {
                ReportId = reportId,
                ProctorId = request.ProctorId,
                TestNumber = nextTestNumber,
                TestArea = request.TestArea,
                Location = request.Location,
                ElevationReference = Enum.Parse<ElevationReference>(request.ElevationReference),
                ElevationValue = request.ElevationValue,
                ElevationUnit = Enum.Parse<ElevationUnit>(request.ElevationUnit),
                CorrectedOversizePercentage = (float)request.CorrectedOversizePercentage,
                ProbeDepth = (int)request.ProbeDepth,
                ProbeDepthUnit = Enum.Parse<ProbeDepthUnit>(request.ProbeDepthUnit),
                CompactionSpecification = request.CompactionSpecification,
                CompactionSpecificationUnit = Enum.Parse<CompactionSpecificationUnit>(request.CompactionSpecificationUnit),
                DensityValue = request.DensityValue,
                MoistureValue = request.MoistureValue,
                CreatedDate = DateTime.UtcNow
            };

            _dbContext.DensityTests.Add(densityTest);
            await _dbContext.SaveChangesAsync();

            return new DensityTestCreateResponse
            {
                Id = densityTest.Id,
                Message = "Density test created successfully"
            };
        }

        public async Task<MemoInfo?> UpdateMemoAsync(int reportId, UpdateMemoRequest request)
        {
            var report = await _dbContext.Reports.FindAsync(reportId);
            if (report == null) return null;

            var memo = await _dbContext.ReportMemos
                .Where(m => m.ReportId == reportId)
                .OrderByDescending(m => m.CreatedDate)
                .FirstOrDefaultAsync();

            if (memo == null)
            {
                memo = new ReportMemo
                {
                    ReportId = reportId,
                    CreatedDate = DateTime.UtcNow
                };
                _dbContext.ReportMemos.Add(memo);
            }

            memo.Purpose = request.Purpose;
            memo.CommentsAndObservations = request.CommentsAndObservations;
            memo.Conclusion = request.Conclusion;
            memo.UpdatedDate = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            return new MemoInfo
            {
                Id = memo.Id,
                Purpose = memo.Purpose,
                CommentsAndObservations = memo.CommentsAndObservations,
                Conclusion = memo.Conclusion,
                CreatedDate = memo.CreatedDate ?? DateTime.UtcNow,
                UpdatedDate = memo.UpdatedDate
            };
        }

        public async Task<IEnumerable<ReportListByJobResponse>> SearchReportsByJobNumberAsync(string jobNumber, int limit = 10)
        {
            var reports = await _dbContext.Reports
                .Include(r => r.Job)
                .Include(r => r.DensityTests)
                .Include(r => r.Photos)
                .Include(r => r.Memos)
                .Where(r => r.Job.JobNumber.Contains(jobNumber))
                .OrderByDescending(r => r.StartDate)
                .Take(limit)
                .ToListAsync();

            var result = new List<ReportListByJobResponse>();

            foreach (var report in reports)
            {
                // Get employee info
                var employee = await _dbContext.PersonalInfos
                    .FirstOrDefaultAsync(p => p.Id == report.EmployeeId && p.Company == "GeoPacific");

                // Get reviewer info (if assigned)
                PersonalInfo? reviewer = null;
                if (report.ReviewerId > 0)
                {
                    reviewer = await _dbContext.PersonalInfos
                        .FirstOrDefaultAsync(p => p.Id == report.ReviewerId && p.Company == "GeoPacific");
                }

                result.Add(MapToListByJobResponse(report, employee, reviewer));
            }

            return result;
        }
    }
}
