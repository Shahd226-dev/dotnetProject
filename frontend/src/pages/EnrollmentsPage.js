import { useEffect, useMemo, useState } from "react";
import enrollmentApi from "../api/enrollmentApi";
import courseApi from "../api/courseApi";
import { useAuth } from "../context/AuthContext";
import { useToast } from "../context/ToastContext";
import SkeletonTable from "../components/SkeletonTable";
import EmptyState from "../components/EmptyState";

const EnrollmentsPage = () => {
  const { role } = useAuth();
  const { addToast } = useToast();
  const [form, setForm] = useState({ courseId: "" });
  const [courses, setCourses] = useState([]);
  const [enrollments, setEnrollments] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const loadStudentData = async () => {
    setLoading(true);
    setError("");
    try {
      const [coursesRes, enrollmentsRes] = await Promise.all([
        courseApi.getAll(),
        enrollmentApi.getMyEnrollments()
      ]);

      if (coursesRes.success && enrollmentsRes.success) {
        setCourses(coursesRes.data || []);
        setEnrollments(enrollmentsRes.data || []);
      } else {
        setError("Unable to load enrollments.");
      }
    } catch (err) {
      setError(err.message || "Unable to load enrollments.");
    } finally {
      setLoading(false);
    }
  };

  const loadInstructorData = async () => {
    setLoading(true);
    setError("");
    try {
      const response = await enrollmentApi.getInstructorEnrollments();
      if (response.success) {
        setEnrollments(response.data || []);
      } else {
        setError(response.message || "Unable to load enrollments.");
      }
    } catch (err) {
      setError(err.message || "Unable to load enrollments.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (role === "Student") {
      loadStudentData();
    } else if (role === "Instructor") {
      loadInstructorData();
    }
  }, [role]);

  const enrolledCourseIds = useMemo(
    () => new Set(enrollments.map((item) => item.course?.id)),
    [enrollments]
  );

  const availableCourses = courses.filter((course) => !enrolledCourseIds.has(course.id));

  const handleSubmit = async (event) => {
    event.preventDefault();

    try {
      const response = await enrollmentApi.enroll({
        courseId: Number(form.courseId)
      });
      if (response.success) {
        addToast({
          type: "success",
          title: "Enrollment successful",
          message: response.message
        });
        setForm({ courseId: "" });
        loadStudentData();
        return;
      }
      addToast({
        type: "error",
        title: "Enrollment failed",
        message: response.message || "Failed to enroll student."
      });
    } catch (err) {
      addToast({
        type: "error",
        title: "Enrollment failed",
        message: err.message || "Failed to enroll student."
      });
    }
  };

  const handleUnenroll = async (courseId) => {
    try {
      const response = await enrollmentApi.unenroll(courseId);
      if (response.success) {
        addToast({
          type: "success",
          title: "Unenrolled",
          message: response.message
        });
        loadStudentData();
        return;
      }
      addToast({
        type: "error",
        title: "Unenroll failed",
        message: response.message || "Unable to unenroll."
      });
    } catch (err) {
      addToast({
        type: "error",
        title: "Unenroll failed",
        message: err.message || "Unable to unenroll."
      });
    }
  };

  return (
    <section>
      <div className="page-header">
        <h2>Enrollments</h2>
        <p className="muted">
          {role === "Student"
            ? "Pick courses to join and track your enrollment progress."
            : "Review student enrollment activity for your courses."}
        </p>
      </div>

      {role === "Student" && (
        <div className="card">
          <h3>Enroll in a course</h3>
          <form onSubmit={handleSubmit} className="form-grid">
            <div className="form-group">
              <label>Course</label>
              <select
                name="courseId"
                className="select"
                value={form.courseId}
                onChange={(event) =>
                  setForm((prev) => ({ ...prev, courseId: event.target.value }))
                }
                required
              >
                <option value="">Select a course</option>
                {availableCourses.map((course) => (
                  <option key={course.id} value={course.id}>
                    {course.title}
                  </option>
                ))}
              </select>
            </div>
            <div className="actions">
              <button className="btn btn-primary" type="submit">
                Enroll
              </button>
            </div>
          </form>
        </div>
      )}

      <div className="card">
        <h3>{role === "Student" ? "My enrollments" : "Enrollment activity"}</h3>
        {loading ? (
          <SkeletonTable />
        ) : enrollments.length ? (
          <table className="table">
            <thead>
              <tr>
                <th>Course</th>
                <th>Student</th>
                <th>Enrolled</th>
                {role === "Student" && <th>Actions</th>}
              </tr>
            </thead>
            <tbody>
              {enrollments.map((enrollment) => (
                <tr key={`${enrollment.course?.id}-${enrollment.student?.id}`}>
                  <td>{enrollment.course?.title || "-"}</td>
                  <td>{enrollment.student?.fullName || "-"}</td>
                  <td>
                    {enrollment.enrolledAt
                      ? new Date(enrollment.enrolledAt).toLocaleDateString()
                      : "-"}
                  </td>
                  {role === "Student" && (
                    <td>
                      <button
                        className="btn btn-secondary"
                        onClick={() => handleUnenroll(enrollment.course?.id)}
                      >
                        Unenroll
                      </button>
                    </td>
                  )}
                </tr>
              ))}
            </tbody>
          </table>
        ) : (
          <EmptyState
            title="No enrollments yet"
            description={
              role === "Student"
                ? "Enroll in a course to start learning."
                : "No students enrolled yet."
            }
          />
        )}
      </div>

      {error && (
        <div className="card">
          <EmptyState title="Unable to load enrollments" description={error} />
        </div>
      )}
    </section>
  );
};

export default EnrollmentsPage;
