using DensityReportingToolBackend.Data;
using DensityReportingToolBackend.Models;
using DensityReportingToolBackend.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace DensityReportingToolBackend.Services
{
    public class ProctorService : IProctorService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<ProctorService> _logger;

        public ProctorService(AppDbContext dbContext, ILogger<ProctorService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<ProctorCreateResponse> CreateProctorAsync(CreateProctorRequest request)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            
            try
            {
                _logger.LogInformation("Creating proctor for job: {JobNumber}", request.JobNumber);

                // 1. Find existing Job (throw error if not found)
                var job = await FindJobAsync(request.JobNumber);
                
                // 2. Create LabTest
                var labTest = await CreateLabTestAsync(job.Id, request);
                
                // 3. Find ProctorType
                var proctorType = await GetProctorTypeAsync(request.ProctorType);
                
                // 4. Create Proctor
                var proctor = await CreateProctorEntityAsync(labTest.Id, proctorType.Id, request);
                
                await transaction.CommitAsync();
                
                _logger.LogInformation("Successfully created proctor with ID: {ProctorId} for job: {JobNumber}", 
                    proctor.Id, request.JobNumber);

                return MapToCreateResponse(proctor, job, labTest, proctorType, request);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Failed to create proctor for job: {JobNumber}", request.JobNumber);
                throw;
            }
        }

        #region Private Helper Methods

        private async Task<Job> FindJobAsync(string jobNumber)
        {
            var job = await _dbContext.Jobs.FirstOrDefaultAsync(j => j.JobNumber == jobNumber);
            
            if (job == null)
            {
                _logger.LogWarning("Job with number {JobNumber} does not exist", jobNumber);
                throw new ArgumentException($"Job with number '{jobNumber}' does not exist. Please create the job first before adding proctors.");
            }
            
            _logger.LogInformation("Found existing job with ID: {JobId} and number: {JobNumber}", 
                job.Id, job.JobNumber);
            
            return job;
        }

        private async Task<LabTest> CreateLabTestAsync(int jobId, CreateProctorRequest request)
        {
            var labTest = new LabTest
            {
                JobId = jobId,
                MaterialType = request.MaterialType,
                ImportLocation = request.LabLocation,
                ReceiveDate = DateTime.TryParse(request.DateSampled, out var sampledDate) ? DateTime.SpecifyKind(sampledDate, DateTimeKind.Utc) : null
            };
            
            _dbContext.LabTests.Add(labTest);
            await _dbContext.SaveChangesAsync();
            
            _logger.LogInformation("Created LabTest with ID: {LabTestId} for job: {JobId}", 
                labTest.Id, jobId);
            
            return labTest;
        }

        private async Task<ProctorType> GetProctorTypeAsync(string proctorTypeName)
        {
            var proctorType = await _dbContext.ProctorTypes
                .FirstOrDefaultAsync(pt => pt.Type == proctorTypeName);
                
            if (proctorType == null)
            {
                throw new ArgumentException($"Proctor type '{proctorTypeName}' not found. Available types: SPDD, MPDD");
            }
            
            _logger.LogInformation("Using ProctorType: {ProctorType} (ID: {ProctorTypeId})", 
                proctorType.Type, proctorType.Id);
            
            return proctorType;
        }

        private async Task<Proctor> CreateProctorEntityAsync(int labTestId, int proctorTypeId, CreateProctorRequest request)
        {
            var proctor = new Proctor
            {
                ProctorID = request.ProctorId,
                LabTestId = labTestId,
                ProctorTypeId = proctorTypeId,
                MaxDensity = ParseDoubleOrNull(request.MaxDryDensity),
                CorrectedDensity = ParseDoubleOrNull(request.CorrectedDensity),
                OptimumMoistureContent = request.OptimumMoisture,
                SpecificGravity = ParseDoubleOrNull(request.SpecificGravity),
                OversizePercentage = request.OversizePercentage,
                DateTested = DateTime.TryParse(request.DateTested, out var testedDate) ? DateTime.SpecifyKind(testedDate, DateTimeKind.Utc) : null
            };
            
            _dbContext.Proctors.Add(proctor);
            await _dbContext.SaveChangesAsync();
            
            _logger.LogInformation("Created Proctor with ID: {ProctorId} and ProctorID: {ProctorTestId}", 
                proctor.Id, proctor.ProctorID);
            
            return proctor;
        }

        private static double? ParseDoubleOrNull(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
                
            return double.TryParse(value, out var result) ? result : null;
        }

        private static ProctorCreateResponse MapToCreateResponse(
            Proctor proctor, 
            Job job, 
            LabTest labTest, 
            ProctorType proctorType, 
            CreateProctorRequest request)
        {
            return new ProctorCreateResponse
            {
                Id = proctor.Id.ToString(),
                Message = "Proctor created successfully",
                Proctor = new ProctorDataResponse
                {
                    JobNumber = job.JobNumber,
                    ProctorTestNumber = request.ProctorTestNumber,
                    MaterialType = labTest.MaterialType ?? "",
                    DateSampled = request.DateSampled,
                    ProctorType = proctorType.Type,
                    MaxDryDensity = request.MaxDryDensity,
                    CorrectedDensity = request.CorrectedDensity,
                    LabLocation = labTest.ImportLocation ?? "",
                    ProctorId = proctor.ProctorID ?? "",
                    DateTested = request.DateTested,
                    OversizePercentage = request.OversizePercentage,
                    OptimumMoisture = request.OptimumMoisture,
                    SpecificGravity = request.SpecificGravity ?? ""
                }
            };
        }

        #endregion

        #region TODO: Implement Additional Methods

        public async Task<ProctorUpdateResponse?> UpdateProctorAsync(int id, UpdateProctorRequest request)
        {
            // TODO: Implement update functionality
            throw new NotImplementedException("UpdateProctorAsync not yet implemented");
        }

        public async Task<ProctorDataResponse?> GetProctorAsync(int id)
        {
            try
            {
                _logger.LogInformation("Getting proctor with ID: {ProctorId}", id);

                var proctor = await _dbContext.Proctors
                    .Include(p => p.LabTest)
                    .ThenInclude(lt => lt.Job)
                    .Include(p => p.ProctorType)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (proctor == null)
                {
                    _logger.LogWarning("Proctor with ID {ProctorId} not found", id);
                    return null;
                }

                _logger.LogInformation("Found proctor {ProctorId} for job {JobNumber}", id, proctor.LabTest.Job.JobNumber);
                
                return MapToDataResponse(proctor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting proctor with ID: {ProctorId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<ProctorDataResponse>> GetProctorsForJobAsync(string jobNumber)
        {
            try
            {
                _logger.LogInformation("Getting proctors for job: {JobNumber}", jobNumber);

                var proctors = await _dbContext.Proctors
                    .Include(p => p.LabTest)
                    .ThenInclude(lt => lt.Job)
                    .Include(p => p.ProctorType)
                    .Where(p => p.LabTest.Job.JobNumber == jobNumber)
                    .ToListAsync();

                _logger.LogInformation("Found {Count} proctors for job {JobNumber}", proctors.Count, jobNumber);

                return proctors.Select(MapToDataResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting proctors for job: {JobNumber}", jobNumber);
                throw;
            }
        }

        public async Task<ProctorListResponse> GetProctorsForLabAdminAsync(int page, int limit, string? jobNumber = null)
        {
            // TODO: Implement get proctors for lab admin functionality
            throw new NotImplementedException("GetProctorsForLabAdminAsync not yet implemented");
        }

        public async Task<DensityRequirementsResponse?> GetDensityRequirementsAsync(int id)
        {
            // TODO: Implement get density requirements functionality
            throw new NotImplementedException("GetDensityRequirementsAsync not yet implemented");
        }

        private static ProctorDataResponse MapToDataResponse(Proctor proctor)
        {
            return new ProctorDataResponse
            {
                JobNumber = proctor.LabTest.Job.JobNumber,
                ProctorTestNumber = proctor.ProctorTestNumber ?? "",
                MaterialType = proctor.LabTest.MaterialType ?? "",
                DateSampled = proctor.LabTest.ReceiveDate?.ToString("yyyy-MM-dd") ?? "",
                ProctorType = proctor.ProctorType.Type,
                MaxDryDensity = proctor.MaxDensity?.ToString() ?? "",
                CorrectedDensity = proctor.CorrectedDensity?.ToString() ?? "",
                LabLocation = proctor.LabTest.ImportLocation ?? "",
                ProctorId = proctor.ProctorID ?? "",
                DateTested = proctor.DateTested?.ToString("yyyy-MM-dd") ?? "",
                OversizePercentage = proctor.OversizePercentage ?? 0,
                OptimumMoisture = proctor.OptimumMoistureContent ?? 0,
                SpecificGravity = proctor.SpecificGravity?.ToString() ?? ""
            };
        }

        #endregion
    }
}