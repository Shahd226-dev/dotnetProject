using System.ComponentModel.DataAnnotations;

public class EnrollStudentDto
{
    [Range(1, int.MaxValue)]
    public int StudentId { get; set; }

    [Range(1, int.MaxValue)]
    public int CourseId { get; set; }
}