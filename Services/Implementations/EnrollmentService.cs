using Microsoft.EntityFrameworkCore;

public class EnrollmentService : IEnrollmentService
{
    private readonly AppDbContext _context;

    public EnrollmentService(AppDbContext context)
    {
        _context = context;
    }

    public async Task EnrollAsync(EnrollStudentDto dto)
    {
        var enrollment = new Enrollment
        {
            StudentId = dto.StudentId,
            CourseId = dto.CourseId
        };

        _context.Enrollments.Add(enrollment);
        await _context.SaveChangesAsync();
    }
}