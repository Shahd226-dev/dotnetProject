public class CourseResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public InstructorSummaryDto Instructor { get; set; } = new InstructorSummaryDto();
}
