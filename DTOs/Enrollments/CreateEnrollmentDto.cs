using System.ComponentModel.DataAnnotations;

public class CreateEnrollmentDto
{
    [Required]
    [Range(1, int.MaxValue)]
    public int CourseId { get; set; }
}
