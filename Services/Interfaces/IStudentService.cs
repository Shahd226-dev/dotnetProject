public interface IStudentService
{
    Task<List<StudentResponseDto>> GetAllAsync();
    Task<StudentResponseDto> CreateAsync(CreateStudentDto dto, int userId);
    Task<StudentResponseDto?> UpdateAsync(int id, UpdateStudentDto dto);
    Task<StudentResponseDto?> GetByIdAsync(int id);
}