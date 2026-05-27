using DensityReportingToolBackend.Data;
using DensityReportingToolBackend.Models;
using DensityReportingToolBackend.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace DensityReportingToolBackend.Services
{
    public interface IProctorService
    {
        // Standard CRUD - all return DTOs (matches Jobs pattern)
        Task<IEnumerable<ProctorDataResponse>> GetAllProctorsAsync();
        Task<ProctorDataResponse?> GetProctorAsync(int proctorId);
        Task<IEnumerable<ProctorDataResponse>> SearchProctorsAsync(string jobNumber, int limit = 100);
        Task<ProctorDataResponse> CreateProctorAsync(ProctorCreateDto dto);
        Task<ProctorDataResponse> UpdateProctorAsync(int proctorId, ProctorUpdateDto dto);

        // Lab Admin - richer request/response models
        Task<ProctorCreateResponse> CreateProctorAsync(CreateProctorRequest request);
        Task<ProctorUpdateResponse?> UpdateProctorAsync(int id, UpdateProctorRequest request);
        Task<ProctorListResponse> GetProctorsForLabAdminAsync(int page, int limit, string? jobNumber = null);

        // Field Tech
        Task<DensityRequirementsResponse?> GetDensityRequirementsAsync(int id);

        // Shared (by job)
        Task<IEnumerable<ProctorDataResponse>> GetProctorsForJobAsync(string jobNumber);
        Task<IEnumerable<ProctorDataResponse>> GetProctorsForJobByIdAsync(int jobId);
    }

    public class ProctorService(AppDbContext dbContext) : IProctorService
    {
        #region Public Interface Implementation

        public async Task<IEnumerable<ProctorDataResponse>> GetAllProctorsAsync()
        {
            var proctors = await ListProctors();
            return proctors.Select(ConvertToProctorDataResponse);
        }

        public async Task<ProctorDataResponse?> GetProctorAsync(int proctorId)
        {
            var proctor = await GetProctorById(proctorId);
            return proctor is null ? null : ConvertToProctorDataResponse(proctor);
        }

        public async Task<IEnumerable<ProctorDataResponse>> SearchProctorsAsync(string jobNumber, int limit = 100)
        {
            var proctors = await SearchProctorsByJobNumber(jobNumber, limit);
            return proctors.Select(ConvertToProctorDataResponse);
        }

        public async Task<ProctorDataResponse> CreateProctorAsync(ProctorCreateDto dto)
        {
            var proctor = await CreateProctor(dto);
            var withNav = await GetProctorById(proctor.Id)
                ?? throw new InvalidOperationException($"Proctor {proctor.Id} not found after creation.");
            return ConvertToProctorDataResponse(withNav);
        }

        public async Task<ProctorDataResponse> UpdateProctorAsync(int proctorId, ProctorUpdateDto dto)
        {
            var proctor = await UpdateProctor(proctorId, dto);
            var withNav = await GetProctorById(proctor.Id)
                ?? throw new InvalidOperationException($"Proctor {proctor.Id} not found after update.");
            return ConvertToProctorDataResponse(withNav);
        }

        public async Task<ProctorCreateResponse> CreateProctorAsync(CreateProctorRequest request)
        {
            var dto = ConvertToProctorCreateDto(request);
            var proctor = await CreateProctor(dto);
            var withNav = await GetProctorById(proctor.Id)
                ?? throw new InvalidOperationException($"Proctor {proctor.Id} not found after creation.");

            return new ProctorCreateResponse
            {
                Id = proctor.Id.ToString(),
                Message = "Proctor created successfully",
                Proctor = ConvertToProctorDataResponse(withNav, request)
            };
        }

        public async Task<ProctorUpdateResponse?> UpdateProctorAsync(int id, UpdateProctorRequest request)
        {
            var dto = ConvertToProctorUpdateDto(id, request);
            var proctor = await UpdateProctor(id, dto);
            var withNav = await GetProctorById(proctor.Id)
                ?? throw new InvalidOperationException($"Proctor {proctor.Id} not found after update.");

            return new ProctorUpdateResponse
            {
                Id = proctor.Id.ToString(),
                Message = "Proctor updated successfully",
                Proctor = ConvertToProctorDataResponse(withNav)
            };
        }

        public async Task<ProctorListResponse> GetProctorsForLabAdminAsync(int page, int limit, string? jobNumber = null)
        {
            IEnumerable<Proctor> proctors = !string.IsNullOrWhiteSpace(jobNumber)
                ? await SearchProctorsByJobNumber(jobNumber, limit * page)
                : await ListProctors();

            var total = proctors.Count();
            var paginatedProctors = proctors
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToList();

            return new ProctorListResponse
            {
                Proctors = paginatedProctors.Select(ConvertToProctorDataResponse),
                Total = total,
                Page = page,
                Limit = limit
            };
        }

        public async Task<DensityRequirementsResponse?> GetDensityRequirementsAsync(int id)
        {
            var proctor = await GetProctorById(id);
            if (proctor == null)
                return null;

            var maxDensity = proctor.MaxDensity ?? 0;
            var correctedDensity = proctor.CorrectedDensity ?? 0;

            return new DensityRequirementsResponse
            {
                ProctorId = proctor.Id,
                ProctorTestNumber = proctor.ProctorTestNumber ?? "",
                MaxDryDensity = maxDensity,
                CorrectedDensity = correctedDensity,
                OptimumMoisture = proctor.OptimumMoistureContent ?? 0,
                CompactionRequirement = "95% of corrected maximum dry density",
                TargetDensity95 = correctedDensity * 0.95,
                TargetDensity90 = correctedDensity * 0.90,
                TargetDensity98 = correctedDensity * 0.98,
                MaterialType = proctor.LabTest.MaterialType ?? "",
                TestMethod = $"{proctor.ProctorType.Type} Proctor",
                ProctorType = proctor.ProctorType.Type,
                SpecificGravity = proctor.SpecificGravity,
                OversizePercentage = proctor.OversizePercentage ?? 0,
                MoistureGuidance = $"Target moisture content: {proctor.OptimumMoistureContent ?? 0}% ± 2%"
            };
        }

        public async Task<IEnumerable<ProctorDataResponse>> GetProctorsForJobAsync(string jobNumber)
        {
            var proctors = await SearchProctorsByJobNumber(jobNumber);
            return proctors.Select(ConvertToProctorDataResponse);
        }

        public async Task<IEnumerable<ProctorDataResponse>> GetProctorsForJobByIdAsync(int jobId)
        {
            var proctors = await dbContext.Proctors
                .Include(p => p.LabTest)
                    .ThenInclude(lt => lt.Job)
                .Include(p => p.ProctorType)
                .Where(p => p.LabTest.JobId == jobId)
                .OrderByDescending(p => p.DateTested)
                .ThenBy(p => p.ProctorID)
                .ToListAsync();

            return proctors.Select(ConvertToProctorDataResponse);
        }

        #endregion

        #region Private Entity Operations

        private async Task<IEnumerable<Proctor>> ListProctors()
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

        private async Task<Proctor?> GetProctorById(int proctorId)
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

        private async Task<IEnumerable<Proctor>> SearchProctorsByJobNumber(string jobNumber, int limit = 100)
        {
            var jobIds = await dbContext.Jobs
                .Where(j => j.JobNumber.Contains(jobNumber))
                .Select(j => j.Id)
                .ToListAsync();

            if (!jobIds.Any())
                return Enumerable.Empty<Proctor>();

            return await dbContext.Proctors
                .Include(p => p.LabTest)
                    .ThenInclude(lt => lt.Job)
                .Include(p => p.ProctorType)
                .Where(p => jobIds.Contains(p.LabTest.JobId))
                .OrderByDescending(p => p.DateTested)
                .ThenBy(p => p.ProctorID)
                .Take(limit)
                .ToListAsync();
        }

        private async Task<Proctor> CreateProctor(ProctorCreateDto dto)
        {
            var job = await dbContext.Jobs.FirstOrDefaultAsync(j => j.JobNumber == dto.JobNumber);
            if (job == null)
                throw new ArgumentException($"Job with number '{dto.JobNumber}' does not exist.");

            var labTest = await FindOrCreateLabTest(job.Id, dto);

            var proctorType = await dbContext.ProctorTypes
                .FirstOrDefaultAsync(pt => pt.Type == dto.ProctorType);
            if (proctorType == null)
                throw new ArgumentException($"Proctor type '{dto.ProctorType}' not found. Must be SPDD or MPDD.");

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

        private async Task<Proctor> UpdateProctor(int proctorId, ProctorUpdateDto dto)
        {
            var proctor = await dbContext.Proctors
                .Include(p => p.LabTest)
                .FirstOrDefaultAsync(p => p.Id == proctorId)
                ?? throw new KeyNotFoundException($"Proctor with ID {proctorId} not found.");

            UpdateProctorProperties(proctor, dto);
            await UpdateLabTestProperties(proctor.LabTest, dto);
            await dbContext.SaveChangesAsync();

            return proctor;
        }

        #endregion

        #region Private Helpers

        private async Task<LabTest> FindOrCreateLabTest(int jobId, ProctorCreateDto dto)
        {
            var labTest = await dbContext.LabTests
                .FirstOrDefaultAsync(lt => lt.JobId == jobId && lt.MaterialType == dto.MaterialType);

            if (labTest == null)
            {
                labTest = new LabTest
                {
                    JobId = jobId,
                    MaterialType = dto.MaterialType,
                    ImportLocation = dto.LabLocation,
                    ReceiveDate = dto.DateSampled
                };
                dbContext.LabTests.Add(labTest);
                await dbContext.SaveChangesAsync();
            }

            return labTest;
        }

        private static void UpdateProctorProperties(Proctor proctor, ProctorUpdateDto dto)
        {
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
        }

        private static async Task UpdateLabTestProperties(LabTest labTest, ProctorUpdateDto dto)
        {
            if (!string.IsNullOrWhiteSpace(dto.MaterialType))
                labTest.MaterialType = dto.MaterialType;
            if (!string.IsNullOrWhiteSpace(dto.LabLocation))
                labTest.ImportLocation = dto.LabLocation;
            if (dto.DateSampled.HasValue)
                labTest.ReceiveDate = dto.DateSampled;

            await Task.CompletedTask;
        }

        private static ProctorDataResponse ConvertToProctorDataResponse(Proctor proctor)
        {
            return new ProctorDataResponse
            {
                Id = proctor.Id,
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

        private static ProctorDataResponse ConvertToProctorDataResponse(Proctor proctor, CreateProctorRequest request)
        {
            return new ProctorDataResponse
            {
                Id = proctor.Id,
                JobNumber = proctor.LabTest.Job.JobNumber,
                ProctorTestNumber = request.ProctorTestNumber,
                MaterialType = proctor.LabTest.MaterialType ?? "",
                DateSampled = request.DateSampled,
                ProctorType = proctor.ProctorType.Type,
                MaxDryDensity = request.MaxDryDensity,
                CorrectedDensity = request.CorrectedDensity,
                LabLocation = proctor.LabTest.ImportLocation ?? "",
                ProctorId = proctor.ProctorID ?? "",
                DateTested = request.DateTested,
                OversizePercentage = request.OversizePercentage,
                OptimumMoisture = request.OptimumMoisture,
                SpecificGravity = request.SpecificGravity ?? ""
            };
        }

        private static ProctorCreateDto ConvertToProctorCreateDto(CreateProctorRequest request)
        {
            return new ProctorCreateDto
            {
                JobNumber = request.JobNumber,
                ProctorId = request.ProctorId,
                MaterialType = request.MaterialType,
                LabLocation = request.LabLocation,
                ProctorType = request.ProctorType,
                MaxDensity = ParseDoubleOrNull(request.MaxDryDensity),
                CorrectedDensity = ParseDoubleOrNull(request.CorrectedDensity),
                OptimumMoisture = request.OptimumMoisture,
                SpecificGravity = ParseDoubleOrNull(request.SpecificGravity),
                OversizePercentage = request.OversizePercentage,
                DateSampled = DateTime.TryParse(request.DateSampled, out var sampledDate)
                    ? DateTime.SpecifyKind(sampledDate, DateTimeKind.Utc) : null,
                DateTested = DateTime.TryParse(request.DateTested, out var testedDate)
                    ? DateTime.SpecifyKind(testedDate, DateTimeKind.Utc) : null
            };
        }

        private static ProctorUpdateDto ConvertToProctorUpdateDto(int id, UpdateProctorRequest request)
        {
            return new ProctorUpdateDto
            {
                Id = id,
                ProctorId = null,
                MaterialType = request.MaterialType,
                LabLocation = request.LabLocation,
                ProctorType = request.ProctorType,
                MaxDensity = ParseDoubleOrNull(request.MaxDryDensity),
                CorrectedDensity = ParseDoubleOrNull(request.CorrectedDensity),
                OptimumMoisture = request.OptimumMoisture,
                SpecificGravity = ParseDoubleOrNull(request.SpecificGravity),
                OversizePercentage = request.OversizePercentage,
                DateSampled = DateTime.TryParse(request.DateSampled, out var sampledDate)
                    ? DateTime.SpecifyKind(sampledDate, DateTimeKind.Utc) : null,
                DateTested = DateTime.TryParse(request.DateTested, out var testedDate)
                    ? DateTime.SpecifyKind(testedDate, DateTimeKind.Utc) : null
            };
        }

        private static double? ParseDoubleOrNull(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
            return double.TryParse(value, out var result) ? result : null;
        }

        #endregion
    }
}
