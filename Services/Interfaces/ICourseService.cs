public interface ICourseService
{
    Task<List<CourseResponseDto>> GetAllAsync();
    Task<CourseResponseDto> CreateAsync(CreateCourseDto dto);
    Task<CourseResponseDto?> UpdateAsync(int id, UpdateCourseDto dto);
    Task<CourseResponseDto?> GetByIdAsync(int id);
}