using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _service;

    public CoursesController(ICourseService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var courses = await _service.GetAllAsync();
        return Ok(ApiResponse<List<CourseResponseDto>>.Ok(courses, "Courses retrieved."));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var course = await _service.GetByIdAsync(id);
        if (course == null)
            return NotFound(ApiResponse<object?>.Fail("Course not found."));

        return Ok(ApiResponse<CourseResponseDto>.Ok(course, "Course retrieved."));
    }

    [HttpPost]
    [Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.Instructor)]
    public async Task<IActionResult> Create(CourseCreateDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id },
            ApiResponse<CourseResponseDto>.Ok(created, "Course created."));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.Instructor)]
    public async Task<IActionResult> Update(int id, CourseUpdateDto dto)
    {
        var updated = await _service.UpdateAsync(id, dto);
        if (updated == null)
            return NotFound(ApiResponse<object?>.Fail("Course not found."));

        return Ok(ApiResponse<CourseResponseDto>.Ok(updated, "Course updated."));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.Instructor)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted)
            return NotFound(ApiResponse<object?>.Fail("Course not found."));

        return Ok(ApiResponse<object?>.Ok(null, "Course deleted."));
    }
}
