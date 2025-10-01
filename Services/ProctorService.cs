using DensityReportingToolBackend.Data;
using DensityReportingToolBackend.Models;
using DensityReportingToolBackend.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace DensityReportingToolBackend.Services
{
    /// <summary>
    /// Service for managing Proctor operations including CRUD operations, 
    /// density calculations, and lab test coordination.
    /// </summary>
    public class ProctorService(AppDbContext dbContext)
    {
        private readonly AppDbContext dbContext = dbContext;

        #region Core Domain Operations

        /// <summary>
        /// Retrieves all proctors with their associated lab tests, jobs, and proctor types.
        /// Results are ordered by test date (descending) and proctor ID.
        /// </summary>
        /// <returns>Collection of proctors with full navigation properties loaded</returns>
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

        /// <summary>
        /// Retrieves a specific proctor by ID with all related navigation properties.
        /// </summary>
        /// <param name="proctorId">The unique identifier of the proctor</param>
        /// <returns>The proctor if found, null otherwise</returns>
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

        /// <summary>
        /// Searches for proctors by job number with fuzzy matching.
        /// Uses a hybrid approach: first finds matching job IDs, then queries proctors using indexed foreign keys.
        /// </summary>
        /// <param name="jobNumber">The job number to search for (partial matches supported)</param>
        /// <param name="limit">Maximum number of results to return (default: 10)</param>
        /// <returns>Collection of matching proctors ordered by test date</returns>
        public async Task<IEnumerable<Proctor>> SearchProctorsByJobNumber(string jobNumber, int limit = 100)
        {
            // Step 1: Find matching job IDs (optimized query on indexed JobNumber field)
            var jobIds = await dbContext.Jobs
                .Where(j => j.JobNumber.Contains(jobNumber))
                .Select(j => j.Id)
                .ToListAsync();
            
            // Early return if no jobs found
            if (!jobIds.Any())
                return Enumerable.Empty<Proctor>();
            
            // Step 2: Query proctors using job IDs (faster integer comparison on indexed FK)
            return await dbContext.Proctors
                .Include(p => p.LabTest)
                    .ThenInclude(lt => lt.Job)
                .Include(p => p.ProctorType)
                .Where(p => jobIds.Contains(p.LabTest.JobId)) // Integer comparison on indexed foreign key
                .OrderByDescending(p => p.DateTested)
                .ThenBy(p => p.ProctorID)
                .Take(limit)
                .ToListAsync();
        }

        /// <summary>
        /// Creates a new proctor with associated lab test if needed.
        /// Validates job existence and proctor type before creation.
        /// </summary>
        /// <param name="dto">The proctor creation data</param>
        /// <returns>The created proctor with all navigation properties</returns>
        /// <exception cref="ArgumentException">Thrown when job number or proctor type is invalid</exception>
        public async Task<Proctor> CreateProctor(ProctorCreateDto dto)
        {
            // Validate job exists
            var job = await dbContext.Jobs.FirstOrDefaultAsync(j => j.JobNumber == dto.JobNumber);
            if (job == null)
                throw new ArgumentException($"Job with number '{dto.JobNumber}' does not exist.");

            // Find or create lab test
            var labTest = await FindOrCreateLabTest(job.Id, dto);

            // Validate proctor type exists
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

        /// <summary>
        /// Updates an existing proctor with the provided data.
        /// Updates both proctor and associated lab test properties as needed.
        /// </summary>
        /// <param name="proctorId">The ID of the proctor to update</param>
        /// <param name="dto">The update data</param>
        /// <returns>The updated proctor</returns>
        /// <exception cref="KeyNotFoundException">Thrown when proctor is not found</exception>
        public async Task<Proctor> UpdateProctor(int proctorId, ProctorUpdateDto dto)
        {
            var proctor = await dbContext.Proctors
                .Include(p => p.LabTest)
                .FirstOrDefaultAsync(p => p.Id == proctorId);

            if (proctor == null)
                throw new KeyNotFoundException($"Proctor with ID {proctorId} not found.");

            // Update proctor properties if provided
            UpdateProctorProperties(proctor, dto);

            // Update lab test properties if provided
            await UpdateLabTestProperties(proctor.LabTest, dto);

            await dbContext.SaveChangesAsync();
            return proctor;
        }

        #endregion

        #region Legacy API Methods (Backward Compatibility)

        /// <summary>
        /// Creates a proctor using the legacy API request format.
        /// Maintains backward compatibility with existing API consumers.
        /// </summary>
        /// <param name="request">The legacy creation request</param>
        /// <returns>Legacy response format</returns>
        public async Task<ProctorCreateResponse> CreateProctorAsync(CreateProctorRequest request)
        {
            var dto = ConvertToProctorCreateDto(request);
            var proctor = await CreateProctor(dto);

            return new ProctorCreateResponse
            {
                Id = proctor.Id.ToString(),
                Message = "Proctor created successfully",
                Proctor = ConvertToProctorDataResponse(proctor, request)
            };
        }

        /// <summary>
        /// Updates a proctor using the legacy API request format.
        /// Maintains backward compatibility with existing API consumers.
        /// </summary>
        /// <param name="id">The proctor ID to update</param>
        /// <param name="request">The legacy update request</param>
        /// <returns>Legacy response format</returns>
        public async Task<ProctorUpdateResponse?> UpdateProctorAsync(int id, UpdateProctorRequest request)
        {
            var dto = ConvertToProctorUpdateDto(id, request);
            var proctor = await UpdateProctor(id, dto);

            return new ProctorUpdateResponse
            {
                Id = proctor.Id.ToString(),
                Message = "Proctor updated successfully",
                Proctor = ConvertToProctorDataResponse(proctor)
            };
        }

        /// <summary>
        /// Retrieves a proctor using the legacy API response format.
        /// Maintains backward compatibility with existing API consumers.
        /// </summary>
        /// <param name="id">The proctor ID</param>
        /// <returns>Legacy response format or null if not found</returns>
        public async Task<ProctorDataResponse?> GetProctorAsync(int id)
        {
            var proctor = await GetProctorById(id);
            return proctor != null ? ConvertToProctorDataResponse(proctor) : null;
        }

        /// <summary>
        /// Retrieves all proctors for a specific job using the legacy API response format.
        /// Maintains backward compatibility with existing API consumers.
        /// </summary>
        /// <param name="jobNumber">The job number to filter by</param>
        /// <returns>Collection of proctors in legacy response format</returns>
        public async Task<IEnumerable<ProctorDataResponse>> GetProctorsForJobAsync(string jobNumber)
        {
            var proctors = await SearchProctorsByJobNumber(jobNumber);
            return proctors.Select(ConvertToProctorDataResponse);
        }

        /// <summary>
        /// Retrieves proctors for lab admin interface with pagination.
        /// Supports filtering by job number and provides paginated results.
        /// </summary>
        /// <param name="page">Page number (1-based)</param>
        /// <param name="limit">Number of items per page</param>
        /// <param name="jobNumber">Optional job number filter</param>
        /// <returns>Paginated list of proctors in legacy response format</returns>
        public async Task<ProctorListResponse> GetProctorsForLabAdminAsync(int page, int limit, string? jobNumber = null)
        {
            IEnumerable<Proctor> proctors = !string.IsNullOrWhiteSpace(jobNumber)
                ? await SearchProctorsByJobNumber(jobNumber, limit * page) // Get more to support pagination
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

        /// <summary>
        /// Generates density requirements for field technicians.
        /// Provides pre-calculated target densities and compaction guidance.
        /// </summary>
        /// <param name="id">The proctor ID</param>
        /// <returns>Density requirements with field tech guidance or null if not found</returns>
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
                CompactionRequirement = "95% of maximum dry density",
                
                // Pre-calculated target densities for field tech convenience
                TargetDensity95 = maxDensity * 0.95,
                TargetDensity90 = maxDensity * 0.90,
                TargetDensity98 = maxDensity * 0.98,
                
                // Additional context for field techs
                MaterialType = proctor.LabTest.MaterialType ?? "",
                TestMethod = $"Modified Proctor ({proctor.ProctorType.Type})",
                ProctorType = proctor.ProctorType.Type,
                SpecificGravity = proctor.SpecificGravity,
                OversizePercentage = proctor.OversizePercentage ?? 0,
                
                // Moisture guidance
                MoistureGuidance = $"Target moisture content: {proctor.OptimumMoistureContent ?? 0}% ± 2%"
            };
        }

        #endregion

        #region Private Helper Methods

        /// <summary>
        /// Finds an existing lab test or creates a new one for the specified job and material type.
        /// </summary>
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

        /// <summary>
        /// Updates proctor entity properties from the update DTO.
        /// </summary>
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

        /// <summary>
        /// Updates lab test entity properties from the update DTO.
        /// </summary>
        private async Task UpdateLabTestProperties(LabTest labTest, ProctorUpdateDto dto)
        {
            if (!string.IsNullOrWhiteSpace(dto.MaterialType) || 
                !string.IsNullOrWhiteSpace(dto.LabLocation) || 
                dto.DateSampled.HasValue)
            {
                if (!string.IsNullOrWhiteSpace(dto.MaterialType))
                    labTest.MaterialType = dto.MaterialType;

                if (!string.IsNullOrWhiteSpace(dto.LabLocation))
                    labTest.ImportLocation = dto.LabLocation;

                if (dto.DateSampled.HasValue)
                    labTest.ReceiveDate = dto.DateSampled;
            }
        }

        /// <summary>
        /// Converts a legacy CreateProctorRequest to a ProctorCreateDto.
        /// </summary>
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
                DateSampled = DateTime.TryParse(request.DateSampled, out var sampledDate) ? DateTime.SpecifyKind(sampledDate, DateTimeKind.Utc) : null,
                DateTested = DateTime.TryParse(request.DateTested, out var testedDate) ? DateTime.SpecifyKind(testedDate, DateTimeKind.Utc) : null
            };
        }

        /// <summary>
        /// Converts a legacy UpdateProctorRequest to a ProctorUpdateDto.
        /// </summary>
        private static ProctorUpdateDto ConvertToProctorUpdateDto(int id, UpdateProctorRequest request)
        {
            return new ProctorUpdateDto
            {
                Id = id,
                ProctorId = null, // ProctorId is not updatable via UpdateProctorRequest
                MaterialType = request.MaterialType,
                LabLocation = request.LabLocation,
                ProctorType = request.ProctorType,
                MaxDensity = ParseDoubleOrNull(request.MaxDryDensity),
                CorrectedDensity = ParseDoubleOrNull(request.CorrectedDensity),
                OptimumMoisture = request.OptimumMoisture,
                SpecificGravity = ParseDoubleOrNull(request.SpecificGravity),
                OversizePercentage = request.OversizePercentage,
                DateSampled = DateTime.TryParse(request.DateSampled, out var sampledDate) ? DateTime.SpecifyKind(sampledDate, DateTimeKind.Utc) : null,
                DateTested = DateTime.TryParse(request.DateTested, out var testedDate) ? DateTime.SpecifyKind(testedDate, DateTimeKind.Utc) : null
            };
        }

        /// <summary>
        /// Converts a Proctor entity to a ProctorDataResponse for legacy API compatibility.
        /// </summary>
        private static ProctorDataResponse ConvertToProctorDataResponse(Proctor proctor)
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

        /// <summary>
        /// Converts a Proctor entity to a ProctorDataResponse using request data for legacy API compatibility.
        /// </summary>
        private static ProctorDataResponse ConvertToProctorDataResponse(Proctor proctor, CreateProctorRequest request)
        {
            return new ProctorDataResponse
            {
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

        /// <summary>
        /// Safely parses a string to a double, returning null if parsing fails.
        /// </summary>
        private static double? ParseDoubleOrNull(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
                
            return double.TryParse(value, out var result) ? result : null;
        }

        #endregion
    }
}