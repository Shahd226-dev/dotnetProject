public interface IInstructorService
{
    Task<List<InstructorResponseDto>> GetAllAsync();
    Task<InstructorResponseDto> CreateAsync(CreateInstructorDto dto);
    Task<InstructorResponseDto?> UpdateAsync(int id, UpdateInstructorDto dto);
    Task<InstructorResponseDto?> GetByIdAsync(int id);
}