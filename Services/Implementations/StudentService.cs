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
                FullName = s.FullName
            })
            .ToList();
    }

    public async Task<StudentResponseDto> CreateAsync(CreateStudentDto dto, int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
            throw new InvalidOperationException("User not found.");

        if (!string.Equals(user.Role, RoleConstants.Student, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Student must be linked to a user with role 'Student'.");

        var linkedStudentExists = await _studentRepository.UserLinkedToStudentAsync(userId);

        if (linkedStudentExists)
            throw new ConflictException("This user is already linked to another student.");

        var linkedInstructorExists = await _instructorRepository.UserLinkedToInstructorAsync(userId);

        if (linkedInstructorExists)
            throw new ConflictException("This user is already linked to an instructor.");

        var student = new Student
        {
            FullName = dto.FullName.Trim(),
            UserId = userId
        };

        await _studentRepository.AddAsync(student);
        await _studentRepository.SaveChangesAsync();

        return new StudentResponseDto
        {
            Id = student.Id,
            FullName = student.FullName
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
            FullName = student.FullName
        };
    }

    public async Task<StudentResponseDto?> UpdateAsync(int id, UpdateStudentDto dto)
    {
        var student = await _studentRepository.GetByIdForUpdateAsync(id);
        if (student == null) return null;

        var user = await _userRepository.GetByIdAsync(student.UserId);

        if (user == null)
            throw new InvalidOperationException("User not found.");

        if (!string.Equals(user.Role, RoleConstants.Student, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Student must be linked to a user with role 'Student'.");

        student.FullName = dto.FullName.Trim();
        await _studentRepository.SaveChangesAsync();

        return new StudentResponseDto
        {
            Id = student.Id,
            FullName = student.FullName
        };
    }
}
