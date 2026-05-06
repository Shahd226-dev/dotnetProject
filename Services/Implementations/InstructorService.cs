public class InstructorService : IInstructorService
{
    private readonly IInstructorRepository _instructorRepository;
    private readonly IUserRepository _userRepository;
    private readonly IStudentRepository _studentRepository;

    public InstructorService(
        IInstructorRepository instructorRepository,
        IUserRepository userRepository,
        IStudentRepository studentRepository)
    {
        _instructorRepository = instructorRepository;
        _userRepository = userRepository;
        _studentRepository = studentRepository;
    }

    public async Task<List<InstructorResponseDto>> GetAllAsync()
    {
        var instructors = await _instructorRepository.GetAllAsync();

        return instructors
            .Select(i => new InstructorResponseDto
            {
                Id = i.Id,
                FullName = i.FullName,
                Bio = i.Bio
            })
            .ToList();
    }

    public async Task<InstructorResponseDto> CreateAsync(CreateInstructorDto dto, int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
            throw new InvalidOperationException("User not found.");

        if (!string.Equals(user.Role, RoleConstants.Instructor, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Instructor must be linked to a user with role 'Instructor'.");

        var linkedInstructorExists = await _instructorRepository.UserLinkedToInstructorAsync(userId);

        if (linkedInstructorExists)
            throw new ConflictException("This user is already linked to another instructor.");

        var linkedStudentExists = await _studentRepository.UserLinkedToStudentAsync(userId);

        if (linkedStudentExists)
            throw new ConflictException("This user is already linked to a student.");

        var instructor = new Instructor
        {
            FullName = dto.FullName.Trim(),
            Bio = dto.Bio?.Trim() ?? string.Empty,
            UserId = userId
        };

        await _instructorRepository.AddAsync(instructor);
        await _instructorRepository.SaveChangesAsync();

        return new InstructorResponseDto
        {
            Id = instructor.Id,
            FullName = instructor.FullName,
            Bio = instructor.Bio
        };
    }

    public async Task<InstructorResponseDto?> GetByIdAsync(int id)
    {
        var instructor = await _instructorRepository.GetByIdAsync(id);

        if (instructor == null)
            return null;

        return new InstructorResponseDto
        {
            Id = instructor.Id,
            FullName = instructor.FullName,
            Bio = instructor.Bio
        };
    }

    public async Task<InstructorResponseDto?> UpdateAsync(int id, UpdateInstructorDto dto)
    {
        var instructor = await _instructorRepository.GetByIdForUpdateAsync(id);
        if (instructor == null) return null;

        var user = await _userRepository.GetByIdAsync(instructor.UserId);

        if (user == null)
            throw new InvalidOperationException("User not found.");

        if (!string.Equals(user.Role, RoleConstants.Instructor, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Instructor must be linked to a user with role 'Instructor'.");

        instructor.FullName = dto.FullName.Trim();
        instructor.Bio = dto.Bio?.Trim() ?? string.Empty;
        await _instructorRepository.SaveChangesAsync();

        return new InstructorResponseDto
        {
            Id = instructor.Id,
            FullName = instructor.FullName,
            Bio = instructor.Bio
        };
    }
}
