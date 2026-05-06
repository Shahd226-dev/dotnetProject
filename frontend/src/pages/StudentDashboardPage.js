import { useEffect, useMemo, useState } from "react";
import courseApi from "../api/courseApi";
import enrollmentApi from "../api/enrollmentApi";
import LoadingSpinner from "../components/LoadingSpinner";
import EmptyState from "../components/EmptyState";

const StudentDashboardPage = () => {
  const [courses, setCourses] = useState([]);
  const [enrollments, setEnrollments] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    const loadData = async () => {
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
          setError("Unable to load student data.");
        }
      } catch (err) {
        setError(err.message || "Unable to load student data.");
      } finally {
        setLoading(false);
      }
    };

    loadData();
  }, []);

  const enrolledCourseIds = useMemo(
    () => new Set(enrollments.map((item) => item.course?.id)),
    [enrollments]
  );

  const availableCourses = courses.filter((course) => !enrolledCourseIds.has(course.id));

  return (
    <section>
      <div className="page-header">
        <h2>Student dashboard</h2>
        <p className="muted">Stay on top of your courses and enrollments.</p>
      </div>

      {loading ? (
        <LoadingSpinner label="Loading dashboard..." />
      ) : error ? (
        <div className="card">
          <EmptyState title="Dashboard unavailable" description={error} />
        </div>
      ) : (
        <>
          <div className="stats-grid">
            <div className="stat-card">
              <p className="stat-label">Enrolled courses</p>
              <h3>{enrollments.length}</h3>
            </div>
            <div className="stat-card">
              <p className="stat-label">Available courses</p>
              <h3>{availableCourses.length}</h3>
            </div>
          </div>

          <div className="card">
            <h3>Next up</h3>
            {availableCourses.length ? (
              <ul className="list-grid">
                {availableCourses.slice(0, 4).map((course) => (
                  <li key={course.id} className="list-item">
                    <div>
                      <p className="list-title">{course.title}</p>
                      <p className="muted">{course.description || "No description"}</p>
                    </div>
                    <span className="chip">{course.instructor?.fullName || "Instructor"}</span>
                  </li>
                ))}
              </ul>
            ) : (
              <EmptyState title="All caught up" description="You are enrolled in every available course." />
            )}
          </div>
        </>
      )}
    </section>
  );
};

export default StudentDashboardPage;
