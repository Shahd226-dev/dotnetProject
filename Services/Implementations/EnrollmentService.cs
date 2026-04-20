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
        var studentExists = await _context.Students
            .AsNoTracking()
            .AnyAsync(s => s.Id == dto.StudentId);

        if (!studentExists)
            throw new InvalidOperationException("Student not found.");

        var courseExists = await _context.Courses
            .AsNoTracking()
            .AnyAsync(c => c.Id == dto.CourseId);

        if (!courseExists)
            throw new InvalidOperationException("Course not found.");

        var alreadyEnrolled = await _context.Enrollments
            .AsNoTracking()
            .AnyAsync(e => e.StudentId == dto.StudentId && e.CourseId == dto.CourseId);

        if (alreadyEnrolled)
            throw new InvalidOperationException("Student is already enrolled in this course.");

        var enrollment = new Enrollment
        {
            StudentId = dto.StudentId,
            CourseId = dto.CourseId
        };

        _context.Enrollments.Add(enrollment);
        await _context.SaveChangesAsync();
    }
}