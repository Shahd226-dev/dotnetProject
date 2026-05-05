using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _service;

    public StudentsController(IStudentService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize(Roles = RoleConstants.Admin)]
    public async Task<IActionResult> Get()
    {
        var students = await _service.GetAllAsync();
        return Ok(ApiResponse<List<StudentDto>>.Ok(students, "Students retrieved."));
    }

    [HttpGet("{id}")]
    [Authorize(Roles = RoleConstants.Admin)]
    public async Task<IActionResult> GetById(int id)
    {
        var student = await _service.GetByIdAsync(id);
        if (student == null)
            return NotFound(ApiResponse<object?>.Fail("Student not found."));

        return Ok(ApiResponse<StudentDto>.Ok(student, "Student retrieved."));
    }

    [HttpPost]
    [Authorize(Roles = RoleConstants.Admin)]
    public async Task<IActionResult> Create(StudentDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id },
            ApiResponse<StudentDto>.Ok(created, "Student created."));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = RoleConstants.Admin)]
    public async Task<IActionResult> Update(int id, StudentDto dto)
    {
        var updated = await _service.UpdateAsync(id, dto);
        if (updated == null)
            return NotFound(ApiResponse<object?>.Fail("Student not found."));

        return Ok(ApiResponse<StudentDto>.Ok(updated, "Student updated."));
    }
}
