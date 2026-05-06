import { useEffect, useState } from "react";
import authApi from "../api/authApi";
import courseApi from "../api/courseApi";
import studentApi from "../api/studentApi";
import instructorApi from "../api/instructorApi";
import LoadingSpinner from "../components/LoadingSpinner";
import EmptyState from "../components/EmptyState";

const AdminDashboardPage = () => {
  const [stats, setStats] = useState({
    users: 0,
    courses: 0,
    students: 0,
    instructors: 0
  });
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    const loadStats = async () => {
      setLoading(true);
      setError("");
      try {
        const [usersRes, coursesRes, studentsRes, instructorsRes] =
          await Promise.all([
            authApi.getUsers(),
            courseApi.getAll(),
            studentApi.getAll(),
            instructorApi.getAll()
          ]);

        if (usersRes.success && coursesRes.success && studentsRes.success && instructorsRes.success) {
          setStats({
            users: usersRes.data?.length || 0,
            courses: coursesRes.data?.length || 0,
            students: studentsRes.data?.length || 0,
            instructors: instructorsRes.data?.length || 0
          });
        } else {
          setError("Unable to load dashboard data.");
        }
      } catch (err) {
        setError(err.message || "Unable to load dashboard data.");
      } finally {
        setLoading(false);
      }
    };

    loadStats();
  }, []);

  return (
    <section>
      <div className="page-header">
        <h2>Admin dashboard</h2>
        <p className="muted">Monitor users, courses, and platform activity.</p>
      </div>

      {loading ? (
        <LoadingSpinner label="Loading dashboard..." />
      ) : error ? (
        <div className="card">
          <EmptyState title="Dashboard unavailable" description={error} />
        </div>
      ) : (
        <div className="stats-grid">
          <div className="stat-card">
            <p className="stat-label">Total users</p>
            <h3>{stats.users}</h3>
          </div>
          <div className="stat-card">
            <p className="stat-label">Courses</p>
            <h3>{stats.courses}</h3>
          </div>
          <div className="stat-card">
            <p className="stat-label">Students</p>
            <h3>{stats.students}</h3>
          </div>
          <div className="stat-card">
            <p className="stat-label">Instructors</p>
            <h3>{stats.instructors}</h3>
          </div>
        </div>
      )}
    </section>
  );
};

export default AdminDashboardPage;
