import { useState } from "react";
import Sidebar from "./Sidebar";
import Topbar from "./Topbar";
import ToastContainer from "./ToastContainer";
import { useAuth } from "../context/AuthContext";

const Layout = ({ children }) => {
  const { isAuthenticated } = useAuth();
  const [collapsed, setCollapsed] = useState(false);

  if (!isAuthenticated) {
    return (
      <div className="auth-shell">
        <main className="auth-content">{children}</main>
        <ToastContainer />
      </div>
    );
  }

  return (
    <div className="app-shell">
      <Sidebar collapsed={collapsed} />
      <div className="app-body">
        <Topbar collapsed={collapsed} onToggle={() => setCollapsed(!collapsed)} />
        <main className="app-content">{children}</main>
      </div>
      <ToastContainer />
    </div>
  );
};

export default Layout;
