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
                Name = i.Name
            })
            .ToListAsync();
    }

    public async Task<InstructorResponseDto> CreateAsync(CreateInstructorDto dto)
    {
        var instructor = new Instructor
        {
            Name = dto.Name
        };

        _context.Instructors.Add(instructor);
        await _context.SaveChangesAsync();

        return new InstructorResponseDto
        {
            Id = instructor.Id,
            Name = instructor.Name
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
                Name = i.Name
            })
            .FirstOrDefaultAsync();
    }

    public async Task<InstructorResponseDto?> UpdateAsync(int id, UpdateInstructorDto dto)
    {
        var instructor = await _context.Instructors.FindAsync(id);
        if (instructor == null) return null;

        instructor.Name = dto.Name;
        await _context.SaveChangesAsync();

        return new InstructorResponseDto
        {
            Id = instructor.Id,
            Name = instructor.Name
        };
    }
}