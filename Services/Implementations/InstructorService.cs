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
                Name = i.Name,
                UserId = i.UserId
            })
            .ToList();
    }

    public async Task<InstructorResponseDto> CreateAsync(CreateInstructorDto dto)
    {
        var user = await _userRepository.GetByIdAsync(dto.UserId);

        if (user == null)
            throw new InvalidOperationException("User not found.");

        if (!string.Equals(user.Role, RoleConstants.Instructor, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Instructor must be linked to a user with role 'Instructor'.");

        var linkedInstructorExists = await _instructorRepository.UserLinkedToInstructorAsync(dto.UserId);

        if (linkedInstructorExists)
            throw new InvalidOperationException("This user is already linked to another instructor.");

        var linkedStudentExists = await _studentRepository.UserLinkedToStudentAsync(dto.UserId);

        if (linkedStudentExists)
            throw new InvalidOperationException("This user is already linked to a student.");

        var instructor = new Instructor
        {
            Name = dto.Name,
            UserId = dto.UserId
        };

        await _instructorRepository.AddAsync(instructor);
        await _instructorRepository.SaveChangesAsync();

        return new InstructorResponseDto
        {
            Id = instructor.Id,
            Name = instructor.Name,
            UserId = instructor.UserId
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
            Name = instructor.Name,
            UserId = instructor.UserId
        };
    }

    public async Task<InstructorResponseDto?> UpdateAsync(int id, UpdateInstructorDto dto)
    {
        var instructor = await _instructorRepository.GetByIdForUpdateAsync(id);
        if (instructor == null) return null;

        var user = await _userRepository.GetByIdAsync(dto.UserId);

        if (user == null)
            throw new InvalidOperationException("User not found.");

        if (!string.Equals(user.Role, RoleConstants.Instructor, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Instructor must be linked to a user with role 'Instructor'.");

        var linkedInstructorExists = await _instructorRepository.UserLinkedToInstructorAsync(dto.UserId, id);

        if (linkedInstructorExists)
            throw new InvalidOperationException("This user is already linked to another instructor.");

        var linkedStudentExists = await _studentRepository.UserLinkedToStudentAsync(dto.UserId);

        if (linkedStudentExists)
            throw new InvalidOperationException("This user is already linked to a student.");

        instructor.Name = dto.Name;
        instructor.UserId = dto.UserId;
        await _instructorRepository.SaveChangesAsync();

        return new InstructorResponseDto
        {
            Id = instructor.Id,
            Name = instructor.Name,
            UserId = instructor.UserId
        };
    }
}
