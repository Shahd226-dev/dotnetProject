import { NavLink } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

const Sidebar = ({ collapsed }) => {
  const { isAuthenticated, isAdmin, isInstructor, role } = useAuth();

  return (
    <aside className={`sidebar ${collapsed ? "collapsed" : ""}`}>
      <div className="sidebar-brand">
        <span className="brand-dot" />
        {!collapsed && <span>Course Hub</span>}
      </div>
      <nav className="sidebar-nav">
        <NavLink to="/dashboard">
          <span className="nav-icon">D</span>
          {!collapsed && "Dashboard"}
        </NavLink>
        {isAuthenticated && (
          <>
            {isAdmin && (
              <NavLink to="/admin/users">
                <span className="nav-icon">U</span>
                {!collapsed && "Users"}
              </NavLink>
            )}
            {isAdmin && (
              <NavLink to="/students">
                <span className="nav-icon">S</span>
                {!collapsed && "Students"}
              </NavLink>
            )}
            {isAdmin && (
              <NavLink to="/instructors">
                <span className="nav-icon">I</span>
                {!collapsed && "Instructors"}
              </NavLink>
            )}
            <NavLink to="/courses">
              <span className="nav-icon">C</span>
              {!collapsed && "Courses"}
            </NavLink>
            {(role === "Student" || isInstructor) && (
              <NavLink to="/enrollments">
                <span className="nav-icon">E</span>
                {!collapsed && "Enrollments"}
              </NavLink>
            )}
          </>
        )}
        {!isAuthenticated && (
          <>
            <NavLink to="/login">
              <span className="nav-icon">L</span>
              {!collapsed && "Login"}
            </NavLink>
            <NavLink to="/register">
              <span className="nav-icon">R</span>
              {!collapsed && "Register"}
            </NavLink>
          </>
        )}
      </nav>
    </aside>
  );
};

export default Sidebar;
