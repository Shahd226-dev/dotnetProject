public class EnrollmentResponseDto
{
    public CourseSummaryDto Course { get; set; } = new CourseSummaryDto();
    public StudentSummaryDto Student { get; set; } = new StudentSummaryDto();
    public DateTime EnrolledAt { get; set; }
}
