public class StudentService : IStudentService
{
    private readonly IStudentRepository _studentRepository;
    private readonly IUserRepository _userRepository;
    private readonly IInstructorRepository _instructorRepository;

    public StudentService(
        IStudentRepository studentRepository,
        IUserRepository userRepository,
        IInstructorRepository instructorRepository)
    {
        _studentRepository = studentRepository;
        _userRepository = userRepository;
        _instructorRepository = instructorRepository;
    }

    public async Task<List<StudentResponseDto>> GetAllAsync()
    {
        var students = await _studentRepository.GetAllAsync();

        return students
            .Select(s => new StudentResponseDto
            {
                Id = s.Id,
                Name = s.Name,
                Email = s.Email,
                UserId = s.UserId
            })
            .ToList();
    }

    public async Task<StudentResponseDto> CreateAsync(CreateStudentDto dto)
    {
        var user = await _userRepository.GetByIdAsync(dto.UserId);

        if (user == null)
            throw new InvalidOperationException("User not found.");

        if (!string.Equals(user.Role, RoleConstants.User, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Student must be linked to a user with role 'User'.");

        var emailExists = await _studentRepository.EmailExistsAsync(dto.Email);
        if (emailExists)
            throw new InvalidOperationException("Email already exists.");

        var linkedStudentExists = await _studentRepository.UserLinkedToStudentAsync(dto.UserId);

        if (linkedStudentExists)
            throw new InvalidOperationException("This user is already linked to another student.");

        var linkedInstructorExists = await _instructorRepository.UserLinkedToInstructorAsync(dto.UserId);

        if (linkedInstructorExists)
            throw new InvalidOperationException("This user is already linked to an instructor.");

        var student = new Student
        {
            Name = dto.Name,
            Email = dto.Email,
            UserId = dto.UserId
        };

        await _studentRepository.AddAsync(student);
        await _studentRepository.SaveChangesAsync();

        return new StudentResponseDto
        {
            Id = student.Id,
            Name = student.Name,
            Email = student.Email,
            UserId = student.UserId
        };
    }

    public async Task<StudentResponseDto?> GetByIdAsync(int id)
    {
        var student = await _studentRepository.GetByIdAsync(id);

        if (student == null)
            return null;

        return new StudentResponseDto
        {
            Id = student.Id,
            Name = student.Name,
            Email = student.Email,
            UserId = student.UserId
        };
    }

    public async Task<StudentResponseDto?> UpdateAsync(int id, UpdateStudentDto dto)
    {
        var student = await _studentRepository.GetByIdForUpdateAsync(id);
        if (student == null) return null;

        var user = await _userRepository.GetByIdAsync(dto.UserId);

        if (user == null)
            throw new InvalidOperationException("User not found.");

        if (!string.Equals(user.Role, RoleConstants.User, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Student must be linked to a user with role 'User'.");

        var linkedStudentExists = await _studentRepository.UserLinkedToStudentAsync(dto.UserId, id);

        if (linkedStudentExists)
            throw new InvalidOperationException("This user is already linked to another student.");

        var linkedInstructorExists = await _instructorRepository.UserLinkedToInstructorAsync(dto.UserId);

        if (linkedInstructorExists)
            throw new InvalidOperationException("This user is already linked to an instructor.");

        student.Name = dto.Name;
        student.UserId = dto.UserId;
        await _studentRepository.SaveChangesAsync();

        return new StudentResponseDto
        {
            Id = student.Id,
            Name = student.Name,
            UserId = student.UserId
        };
    }
}
