import { useEffect, useState } from "react";
import courseApi from "../api/courseApi";
import enrollmentApi from "../api/enrollmentApi";
import LoadingSpinner from "../components/LoadingSpinner";
import EmptyState from "../components/EmptyState";

const InstructorDashboardPage = () => {
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
          enrollmentApi.getInstructorEnrollments()
        ]);

        if (coursesRes.success && enrollmentsRes.success) {
          setCourses(coursesRes.data || []);
          setEnrollments(enrollmentsRes.data || []);
        } else {
          setError("Unable to load instructor data.");
        }
      } catch (err) {
        setError(err.message || "Unable to load instructor data.");
      } finally {
        setLoading(false);
      }
    };

    loadData();
  }, []);

  const activeCourses = courses.length;
  const totalEnrollments = enrollments.length;
  const recentCourses = courses.slice(0, 4);

  return (
    <section>
      <div className="page-header">
        <h2>Instructor dashboard</h2>
        <p className="muted">Track your courses and active enrollments.</p>
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
              <p className="stat-label">Courses</p>
              <h3>{activeCourses}</h3>
            </div>
            <div className="stat-card">
              <p className="stat-label">Enrollments</p>
              <h3>{totalEnrollments}</h3>
            </div>
          </div>

          <div className="card">
            <h3>Recent courses</h3>
            {recentCourses.length ? (
              <ul className="list-grid">
                {recentCourses.map((course) => (
                  <li key={course.id} className="list-item">
                    <div>
                      <p className="list-title">{course.title}</p>
                      <p className="muted">{course.description || "No description"}</p>
                    </div>
                    <span className="chip">{course.instructor?.fullName || "Assigned"}</span>
                  </li>
                ))}
              </ul>
            ) : (
              <EmptyState title="No courses yet" description="Create your first course to get started." />
            )}
          </div>
        </>
      )}
    </section>
  );
};

export default InstructorDashboardPage;
