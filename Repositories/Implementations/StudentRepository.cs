using Microsoft.EntityFrameworkCore;

public class StudentRepository : IStudentRepository
{
    private readonly AppDbContext _context;

    public StudentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Student>> GetAllAsync()
    {
        return await _context.Students
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Student?> GetByIdAsync(int id)
    {
        return await _context.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public Task<Student?> GetByIdForUpdateAsync(int id)
    {
        return _context.Students.FindAsync(id).AsTask();
    }

    public async Task<Student?> GetByUserIdAsync(int userId)
    {
        return await _context.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.UserId == userId);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Students
            .AsNoTracking()
            .AnyAsync(s => s.Id == id);
    }

    public async Task<bool> UserLinkedToStudentAsync(int userId, int? excludeStudentId = null)
    {
        var query = _context.Students
            .AsNoTracking()
            .Where(s => s.UserId == userId);

        if (excludeStudentId.HasValue)
        {
            query = query.Where(s => s.Id != excludeStudentId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task AddAsync(Student student)
    {
        await _context.Students.AddAsync(student);
    }

    public Task SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }
}
