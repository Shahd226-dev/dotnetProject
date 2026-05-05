using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EnrollmentsController : ControllerBase
{
    private readonly IEnrollmentService _service;

    public EnrollmentsController(IEnrollmentService service)
    {
        _service = service;
    }

    [HttpPost]
    [Authorize(Roles = RoleConstants.Student)]
    public async Task<IActionResult> Enroll(EnrollmentDto dto)
    {
        var enrollment = await _service.EnrollAsync(dto);
        return Ok(ApiResponse<EnrollmentDto>.Ok(enrollment, "Enrolled successfully."));
    }

    [HttpDelete("{courseId}")]
    [Authorize(Roles = RoleConstants.Student)]
    public async Task<IActionResult> Unenroll(int courseId)
    {
        var removed = await _service.UnenrollAsync(courseId);
        if (!removed)
            return NotFound(ApiResponse<object?>.Fail("Enrollment not found."));

        return Ok(ApiResponse<object?>.Ok(null, "Unenrolled successfully."));
    }

    [HttpGet("me")]
    [Authorize(Roles = RoleConstants.Student)]
    public async Task<IActionResult> GetMyEnrollments()
    {
        var enrollments = await _service.GetMyEnrollmentsAsync();
        return Ok(ApiResponse<List<EnrollmentDto>>.Ok(enrollments, "Enrollments retrieved."));
    }

    [HttpGet("instructor")]
    [Authorize(Roles = RoleConstants.Instructor)]
    public async Task<IActionResult> GetInstructorEnrollments()
    {
        var enrollments = await _service.GetEnrollmentsForInstructorAsync();
        return Ok(ApiResponse<List<EnrollmentDto>>.Ok(enrollments, "Enrollments retrieved."));
    }
}
