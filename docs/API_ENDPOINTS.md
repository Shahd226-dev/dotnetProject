# API Endpoints

## Auth

- POST /api/auth/login
  - Access: Anonymous
  - Body: LoginDto
  - Returns: success message and sets HttpOnly access token cookie

- POST /api/auth/register
  - Access: Anonymous
  - Body: RegisterDto
  - Returns: success message

- POST /api/auth/revoke
  - Access: Authenticated
  - Returns: revokes active tokens for current user and clears auth cookie

## Students

- GET /api/students
  - Access: Authenticated
  - Returns: list of StudentResponseDto

- GET /api/students/{id}
  - Access: Authenticated
  - Returns: StudentResponseDto

- POST /api/students
  - Access: Admin
  - Body: CreateStudentDto
  - Returns: created StudentResponseDto

- PUT /api/students/{id}
  - Access: Admin
  - Body: UpdateStudentDto
  - Returns: updated StudentResponseDto

## Instructors

- GET /api/instructors
  - Access: Authenticated
  - Returns: list of InstructorResponseDto

- GET /api/instructors/{id}
  - Access: Authenticated
  - Returns: InstructorResponseDto

- POST /api/instructors
  - Access: Admin
  - Body: CreateInstructorDto
  - Returns: created InstructorResponseDto

- PUT /api/instructors/{id}
  - Access: Admin
  - Body: UpdateInstructorDto
  - Returns: updated InstructorResponseDto

## Courses

- GET /api/courses
  - Access: Authenticated
  - Returns: list of CourseResponseDto

- GET /api/courses/{id}
  - Access: Authenticated
  - Returns: CourseResponseDto

- POST /api/courses
  - Access: Admin
  - Body: CreateCourseDto
  - Returns: created CourseResponseDto

- PUT /api/courses/{id}
  - Access: Admin
  - Body: UpdateCourseDto
  - Returns: updated CourseResponseDto

## Enrollments

- POST /api/enrollments
  - Access: Instructor, Admin
  - Body: EnrollStudentDto
  - Returns: success message

## Validation Summary

Validation attributes include:

- Required
- MaxLength
- MinLength
- EmailAddress
- Range

Invalid requests return 400 Bad Request under ApiController model validation behavior.

## Background Jobs

- Hangfire dashboard: /hangfire
