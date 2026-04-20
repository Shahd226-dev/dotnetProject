using Microsoft.EntityFrameworkCore;

public class InstructorService : IInstructorService
{
    private readonly AppDbContext _context;

    public InstructorService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<InstructorResponseDto>> GetAllAsync()
    {
        return await _context.Instructors
            .AsNoTracking()
            .Select(i => new InstructorResponseDto
            {
                Id = i.Id,
                Name = i.Name,
                UserId = i.UserId
            })
            .ToListAsync();
    }

    public async Task<InstructorResponseDto> CreateAsync(CreateInstructorDto dto)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == dto.UserId);

        if (user == null)
            throw new InvalidOperationException("User not found.");

        if (!string.Equals(user.Role, RoleConstants.Instructor, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Instructor must be linked to a user with role 'Instructor'.");

        var linkedInstructorExists = await _context.Instructors
            .AsNoTracking()
            .AnyAsync(i => i.UserId == dto.UserId);

        if (linkedInstructorExists)
            throw new InvalidOperationException("This user is already linked to another instructor.");

        var linkedStudentExists = await _context.Students
            .AsNoTracking()
            .AnyAsync(s => s.UserId == dto.UserId);

        if (linkedStudentExists)
            throw new InvalidOperationException("This user is already linked to a student.");

        var instructor = new Instructor
        {
            Name = dto.Name,
            UserId = dto.UserId
        };

        _context.Instructors.Add(instructor);
        await _context.SaveChangesAsync();

        return new InstructorResponseDto
        {
            Id = instructor.Id,
            Name = instructor.Name,
            UserId = instructor.UserId
        };
    }

    public async Task<InstructorResponseDto?> GetByIdAsync(int id)
    {
        return await _context.Instructors
            .AsNoTracking()
            .Where(i => i.Id == id)
            .Select(i => new InstructorResponseDto
            {
                Id = i.Id,
                Name = i.Name,
                UserId = i.UserId
            })
            .FirstOrDefaultAsync();
    }

    public async Task<InstructorResponseDto?> UpdateAsync(int id, UpdateInstructorDto dto)
    {
        var instructor = await _context.Instructors.FindAsync(id);
        if (instructor == null) return null;

        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == dto.UserId);

        if (user == null)
            throw new InvalidOperationException("User not found.");

        if (!string.Equals(user.Role, RoleConstants.Instructor, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Instructor must be linked to a user with role 'Instructor'.");

        var linkedInstructorExists = await _context.Instructors
            .AsNoTracking()
            .AnyAsync(i => i.UserId == dto.UserId && i.Id != id);

        if (linkedInstructorExists)
            throw new InvalidOperationException("This user is already linked to another instructor.");

        var linkedStudentExists = await _context.Students
            .AsNoTracking()
            .AnyAsync(s => s.UserId == dto.UserId);

        if (linkedStudentExists)
            throw new InvalidOperationException("This user is already linked to a student.");

        instructor.Name = dto.Name;
        instructor.UserId = dto.UserId;
        await _context.SaveChangesAsync();

        return new InstructorResponseDto
        {
            Id = instructor.Id,
            Name = instructor.Name,
            UserId = instructor.UserId
        };
    }
}