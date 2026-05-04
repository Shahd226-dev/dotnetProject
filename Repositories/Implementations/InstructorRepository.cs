using Microsoft.EntityFrameworkCore;

public class InstructorRepository : IInstructorRepository
{
    private readonly AppDbContext _context;

    public InstructorRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Instructor>> GetAllAsync()
    {
        return await _context.Instructors
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Instructor?> GetByIdAsync(int id)
    {
        return await _context.Instructors
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public Task<Instructor?> GetByIdForUpdateAsync(int id)
    {
        return _context.Instructors.FindAsync(id).AsTask();
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Instructors
            .AsNoTracking()
            .AnyAsync(i => i.Id == id);
    }

    public async Task<bool> UserLinkedToInstructorAsync(int userId, int? excludeInstructorId = null)
    {
        var query = _context.Instructors
            .AsNoTracking()
            .Where(i => i.UserId == userId);

        if (excludeInstructorId.HasValue)
        {
            query = query.Where(i => i.Id != excludeInstructorId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task AddAsync(Instructor instructor)
    {
        await _context.Instructors.AddAsync(instructor);
    }

    public Task SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }
}
