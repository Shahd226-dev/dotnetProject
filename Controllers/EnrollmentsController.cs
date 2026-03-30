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
    [Authorize(Roles = RoleConstants.Instructor + "," + RoleConstants.Admin)]
    public async Task<IActionResult> Enroll(EnrollStudentDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _service.EnrollAsync(dto);
        return Ok("Student Enrolled");
    }
}