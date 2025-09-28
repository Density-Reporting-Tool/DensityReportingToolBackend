using DensityReportingToolBackend.Data;
using DensityReportingToolBackend.Models;
using DensityReportingToolBackend.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace DensityReportingToolBackend.Services
{
    public class ProctorService(AppDbContext dbContext)
    {
        private readonly AppDbContext dbContext = dbContext;

        public async Task<IEnumerable<Proctor>> ListProctors()
        {
            return await dbContext.Proctors
                        .Include(p => p.LabTest)
                            .ThenInclude(lt => lt.Job)
                        .Include(p => p.ProctorType)
                        .Include(p => p.AdditionalJobs)
                            .ThenInclude(paj => paj.Job)
                        .OrderByDescending(p => p.DateTested)
                        .ThenBy(p => p.ProctorID)
                        .ToListAsync();
        }

        public async Task<Proctor?> GetProctorById(int proctorId)
        {
            return await dbContext.Proctors
                        .Include(p => p.LabTest)
                            .ThenInclude(lt => lt.Job)
                        .Include(p => p.ProctorType)
                        .Include(p => p.AdditionalJobs)
                            .ThenInclude(paj => paj.Job)
                        .Include(p => p.DensityTests)
                        .FirstOrDefaultAsync(p => p.Id == proctorId);
        }

        public async Task<IEnumerable<Proctor>> SearchProctorsByJobNumber(string jobNumber, int limit = 10)
        {
            return await dbContext.Proctors
                .Include(p => p.LabTest)
                    .ThenInclude(lt => lt.Job)
                .Include(p => p.ProctorType)
                .Where(p => p.LabTest.Job.JobNumber.Contains(jobNumber))
                .OrderByDescending(p => p.DateTested)
                .ThenBy(p => p.ProctorID)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<Proctor> CreateProctor(ProctorCreateDto dto)
        {
            // Find the job first
            var job = await dbContext.Jobs.FirstOrDefaultAsync(j => j.JobNumber == dto.JobNumber);
            if (job == null)
                throw new ArgumentException($"Job with number '{dto.JobNumber}' does not exist.");

            // Find or create lab test
            var labTest = await dbContext.LabTests
                .FirstOrDefaultAsync(lt => lt.JobId == job.Id && lt.MaterialType == dto.MaterialType);
            
            if (labTest == null)
            {
                labTest = new LabTest
                {
                    JobId = job.Id,
                    MaterialType = dto.MaterialType,
                    ImportLocation = dto.LabLocation,
                    ReceiveDate = dto.DateSampled
                };
                dbContext.LabTests.Add(labTest);
                await dbContext.SaveChangesAsync();
            }

            // Find proctor type
            var proctorType = await dbContext.ProctorTypes
                .FirstOrDefaultAsync(pt => pt.Type == dto.ProctorType);
            if (proctorType == null)
                throw new ArgumentException($"Proctor type '{dto.ProctorType}' not found.");

            // Create proctor
            var proctor = new Proctor
            {
                ProctorID = dto.ProctorId,
                LabTestId = labTest.Id,
                ProctorTypeId = proctorType.Id,
                MaxDensity = dto.MaxDensity,
                CorrectedDensity = dto.CorrectedDensity,
                OptimumMoistureContent = dto.OptimumMoisture,
                SpecificGravity = dto.SpecificGravity,
                OversizePercentage = dto.OversizePercentage,
                DateTested = dto.DateTested
            };

            dbContext.Proctors.Add(proctor);
            await dbContext.SaveChangesAsync();

            return proctor;
        }

        public async Task<Proctor> UpdateProctor(int proctorId, ProctorUpdateDto dto)
        {
            var proctor = await dbContext.Proctors
                .Include(p => p.LabTest)
                .FirstOrDefaultAsync(p => p.Id == proctorId);

            if (proctor == null)
                throw new KeyNotFoundException($"Proctor with ID {proctorId} not found.");

            // Update proctor properties if provided
            if (!string.IsNullOrWhiteSpace(dto.ProctorId))
                proctor.ProctorID = dto.ProctorId;

            if (dto.MaxDensity.HasValue)
                proctor.MaxDensity = dto.MaxDensity;

            if (dto.CorrectedDensity.HasValue)
                proctor.CorrectedDensity = dto.CorrectedDensity;

            if (dto.OptimumMoisture.HasValue)
                proctor.OptimumMoistureContent = dto.OptimumMoisture;

            if (dto.SpecificGravity.HasValue)
                proctor.SpecificGravity = dto.SpecificGravity;

            if (dto.OversizePercentage.HasValue)
                proctor.OversizePercentage = dto.OversizePercentage;

            if (dto.DateTested.HasValue)
                proctor.DateTested = dto.DateTested;

            // Update lab test if material type or location changed
            if (!string.IsNullOrWhiteSpace(dto.MaterialType) || !string.IsNullOrWhiteSpace(dto.LabLocation))
            {
                if (!string.IsNullOrWhiteSpace(dto.MaterialType))
                    proctor.LabTest.MaterialType = dto.MaterialType;

                if (!string.IsNullOrWhiteSpace(dto.LabLocation))
                    proctor.LabTest.ImportLocation = dto.LabLocation;

                if (dto.DateSampled.HasValue)
                    proctor.LabTest.ReceiveDate = dto.DateSampled;
            }

            await dbContext.SaveChangesAsync();
            return proctor;
        }

        public async Task<ProctorCreateResponse> CreateProctorAsync(CreateProctorRequest request)
        {
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            
            try
            {
                // 1. Find existing Job (throw error if not found)
                var job = await FindJobAsync(request.JobNumber);
                
                // 2. Create LabTest
                var labTest = await CreateLabTestAsync(job.Id, request);
                
                // 3. Find ProctorType
                var proctorType = await GetProctorTypeAsync(request.ProctorType);
                
                // 4. Create Proctor
                var proctor = await CreateProctorEntityAsync(labTest.Id, proctorType.Id, request);
                
                await transaction.CommitAsync();

                return MapToCreateResponse(proctor, job, labTest, proctorType, request);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        #region Private Helper Methods

        private async Task<Job> FindJobAsync(string jobNumber)
        {
            var job = await dbContext.Jobs.FirstOrDefaultAsync(j => j.JobNumber == jobNumber);
            
            if (job == null)
            {
                throw new ArgumentException($"Job with number '{jobNumber}' does not exist. Please create the job first before adding proctors.");
            }
            
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
            
            dbContext.LabTests.Add(labTest);
            await dbContext.SaveChangesAsync();
            
            return labTest;
        }

        private async Task<ProctorType> GetProctorTypeAsync(string proctorTypeName)
        {
            var proctorType = await dbContext.ProctorTypes
                .FirstOrDefaultAsync(pt => pt.Type == proctorTypeName);
                
            if (proctorType == null)
            {
                throw new ArgumentException($"Proctor type '{proctorTypeName}' not found. Available types: SPDD, MPDD");
            }
            
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
            
            dbContext.Proctors.Add(proctor);
            await dbContext.SaveChangesAsync();
            
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
            var proctor = await dbContext.Proctors
                .Include(p => p.LabTest)
                .ThenInclude(lt => lt.Job)
                .Include(p => p.ProctorType)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (proctor == null)
                return null;
                
            return MapToDataResponse(proctor);
        }

        public async Task<IEnumerable<ProctorDataResponse>> GetProctorsForJobAsync(string jobNumber)
        {
            var proctors = await dbContext.Proctors
                .Include(p => p.LabTest)
                .ThenInclude(lt => lt.Job)
                .Include(p => p.ProctorType)
                .Where(p => p.LabTest.Job.JobNumber == jobNumber)
                .ToListAsync();

            return proctors.Select(MapToDataResponse);
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