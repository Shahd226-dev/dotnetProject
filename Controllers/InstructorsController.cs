using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InstructorsController : ControllerBase
{
    private readonly IInstructorService _service;

    public InstructorsController(IInstructorService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize(Roles = RoleConstants.Admin)]
    public async Task<IActionResult> Get()
    {
        var instructors = await _service.GetAllAsync();
        return Ok(ApiResponse<List<InstructorResponseDto>>.Ok(instructors, "Instructors retrieved."));
    }

    [HttpGet("{id}")]
    [Authorize(Roles = RoleConstants.Admin)]
    public async Task<IActionResult> GetById(int id)
    {
        var instructor = await _service.GetByIdAsync(id);
        if (instructor == null)
            return NotFound(ApiResponse<object?>.Fail("Instructor not found."));

        return Ok(ApiResponse<InstructorResponseDto>.Ok(instructor, "Instructor retrieved."));
    }

    [HttpPost]
    [Authorize(Roles = RoleConstants.Admin)]
    public async Task<IActionResult> Create(CreateInstructorDto dto, [FromQuery] int userId)
    {
        var created = await _service.CreateAsync(dto, userId);
        return CreatedAtAction(nameof(GetById), new { id = created.Id },
            ApiResponse<InstructorResponseDto>.Ok(created, "Instructor created."));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = RoleConstants.Admin)]
    public async Task<IActionResult> Update(int id, UpdateInstructorDto dto)
    {
        var updated = await _service.UpdateAsync(id, dto);
        if (updated == null)
            return NotFound(ApiResponse<object?>.Fail("Instructor not found."));

        return Ok(ApiResponse<InstructorResponseDto>.Ok(updated, "Instructor updated."));
    }
}
