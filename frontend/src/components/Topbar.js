import { useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

const Topbar = ({ collapsed, onToggle }) => {
  const { isAuthenticated, user, role, logout, loading } = useAuth();
  const navigate = useNavigate();

  const handleLogout = async () => {
    await logout();
    navigate("/login");
  };

  return (
    <header className="topbar">
      <div className="topbar-left">
        <button className="icon-btn" onClick={onToggle} aria-label="Toggle menu">
          {collapsed ? "›" : "‹"}
        </button>
        <div>
          <h1>Course Management</h1>
          <p className="muted">Modern academic operations dashboard</p>
        </div>
      </div>
      <div className="topbar-right">
        {isAuthenticated && (
          <>
            <div className="user-pill">
              <div className="avatar">{user?.username?.[0]?.toUpperCase() || "U"}</div>
              <div>
                <p className="user-name">{user?.username}</p>
                <span className="badge badge-neutral">{role}</span>
              </div>
            </div>
            <button
              className="btn btn-secondary"
              onClick={handleLogout}
              disabled={loading}
            >
              Logout
            </button>
          </>
        )}
      </div>
    </header>
  );
};

export default Topbar;
