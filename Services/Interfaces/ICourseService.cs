public interface ICourseService
{
    Task<List<CourseResponseDto>> GetAllAsync();
    Task<CourseResponseDto> CreateAsync(CreateCourseDto dto, int? instructorId);
    Task<CourseResponseDto?> UpdateAsync(int id, UpdateCourseDto dto, int? instructorId);
    Task<bool> DeleteAsync(int id);
    Task<CourseResponseDto?> GetByIdAsync(int id);
}