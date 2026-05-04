import { useAuth } from "../context/AuthContext";

const DashboardPage = () => {
  const { user } = useAuth();

  return (
    <section className="card">
      <div className="page-header">
        <h2>Dashboard</h2>
        <p className="muted">
          Welcome back{user?.username ? `, ${user.username}` : ""}. Track
          enrollments, courses, and academic activity at a glance.
        </p>
      </div>
      <div className="card card-muted">
        <h3>Quick actions</h3>
        <p className="muted">
          Use the navigation to manage students, instructors, courses, and
          enrollments.
        </p>
      </div>
    </section>
  );
};

export default DashboardPage;
