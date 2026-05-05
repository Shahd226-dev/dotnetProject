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

    public async Task<List<StudentDto>> GetAllAsync()
    {
        var students = await _studentRepository.GetAllAsync();

        return students
            .Select(s => new StudentDto
            {
                Id = s.Id,
                FullName = s.FullName,
                UserId = s.UserId
            })
            .ToList();
    }

    public async Task<StudentDto> CreateAsync(StudentDto dto)
    {
        var user = await _userRepository.GetByIdAsync(dto.UserId);

        if (user == null)
            throw new InvalidOperationException("User not found.");

        if (!string.Equals(user.Role, RoleConstants.Student, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Student must be linked to a user with role 'Student'.");

        var linkedStudentExists = await _studentRepository.UserLinkedToStudentAsync(dto.UserId);

        if (linkedStudentExists)
            throw new ConflictException("This user is already linked to another student.");

        var linkedInstructorExists = await _instructorRepository.UserLinkedToInstructorAsync(dto.UserId);

        if (linkedInstructorExists)
            throw new ConflictException("This user is already linked to an instructor.");

        var student = new Student
        {
            FullName = dto.FullName.Trim(),
            UserId = dto.UserId
        };

        await _studentRepository.AddAsync(student);
        await _studentRepository.SaveChangesAsync();

        return new StudentDto
        {
            Id = student.Id,
            FullName = student.FullName,
            UserId = student.UserId
        };
    }

    public async Task<StudentDto?> GetByIdAsync(int id)
    {
        var student = await _studentRepository.GetByIdAsync(id);

        if (student == null)
            return null;

        return new StudentDto
        {
            Id = student.Id,
            FullName = student.FullName,
            UserId = student.UserId
        };
    }

    public async Task<StudentDto?> UpdateAsync(int id, StudentDto dto)
    {
        var student = await _studentRepository.GetByIdForUpdateAsync(id);
        if (student == null) return null;

        var user = await _userRepository.GetByIdAsync(dto.UserId);

        if (user == null)
            throw new InvalidOperationException("User not found.");

        if (!string.Equals(user.Role, RoleConstants.Student, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Student must be linked to a user with role 'Student'.");

        var linkedStudentExists = await _studentRepository.UserLinkedToStudentAsync(dto.UserId, id);

        if (linkedStudentExists)
            throw new ConflictException("This user is already linked to another student.");

        var linkedInstructorExists = await _instructorRepository.UserLinkedToInstructorAsync(dto.UserId);

        if (linkedInstructorExists)
            throw new ConflictException("This user is already linked to an instructor.");

        student.FullName = dto.FullName.Trim();
        student.UserId = dto.UserId;
        await _studentRepository.SaveChangesAsync();

        return new StudentDto
        {
            Id = student.Id,
            FullName = student.FullName,
            UserId = student.UserId
        };
    }
}
