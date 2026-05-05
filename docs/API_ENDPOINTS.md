# API Endpoints

## Response Format

All responses follow:

```json
{
  "success": true,
  "data": {},
  "message": "..."
}
```

## Auth

- POST /api/auth/login
  - Access: Anonymous
  - Body: LoginDto
  - Returns: AuthResponseDto (accessToken + user)

- POST /api/auth/register
  - Access: Anonymous
  - Body: RegisterDto
  - Returns: AuthResponseDto (accessToken + user)

- GET /api/auth/users
  - Access: Admin
  - Returns: list of UserResponseDto

- DELETE /api/auth/users/{id}
  - Access: Admin
  - Returns: success message

## Students

- GET /api/students
  - Access: Admin
  - Returns: list of StudentDto (id, fullName, userId)

- GET /api/students/{id}
  - Access: Admin
  - Returns: StudentDto (id, fullName, userId)

- POST /api/students
  - Access: Admin
  - Body: StudentDto
  - Returns: created StudentDto

- PUT /api/students/{id}
  - Access: Admin
  - Body: StudentDto
  - Returns: updated StudentDto

## Instructors

- GET /api/instructors
  - Access: Admin
  - Returns: list of InstructorDto

- GET /api/instructors/{id}
  - Access: Admin
  - Returns: InstructorDto

- POST /api/instructors
  - Access: Admin
  - Body: InstructorDto
  - Returns: created InstructorDto

- PUT /api/instructors/{id}
  - Access: Admin
  - Body: InstructorDto
  - Returns: updated InstructorDto

## Courses

- GET /api/courses
  - Access: Student, Instructor, Admin
  - Returns: list of CourseResponseDto (id, title, description, instructorId)

- GET /api/courses/{id}
  - Access: Student, Instructor, Admin
  - Returns: CourseResponseDto (id, title, description, instructorId)

- POST /api/courses
  - Access: Admin, Instructor
  - Body: CourseCreateDto
  - Returns: created CourseResponseDto

- PUT /api/courses/{id}
  - Access: Admin, Instructor
  - Body: CourseUpdateDto
  - Returns: updated CourseResponseDto

- DELETE /api/courses/{id}
  - Access: Admin, Instructor
  - Returns: success message

## Enrollments

- POST /api/enrollments
  - Access: Student
  - Body: EnrollmentDto (courseId required)
  - Returns: EnrollmentDto

- DELETE /api/enrollments/{courseId}
  - Access: Student
  - Returns: success message

- GET /api/enrollments/me
  - Access: Student
  - Returns: list of EnrollmentDto

- GET /api/enrollments/instructor
  - Access: Instructor
  - Returns: list of EnrollmentDto

## Validation Summary

Validation attributes include:

- Required
- MaxLength
- MinLength
- EmailAddress
- Range

Invalid requests return 400 Bad Request under ApiController model validation behavior.
