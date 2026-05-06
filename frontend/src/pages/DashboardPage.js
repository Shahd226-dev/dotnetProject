import { Navigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { getDashboardPathForRole } from "../utils/roleRoutes";

const DashboardPage = () => {
  const { role } = useAuth();
  const nextPath = getDashboardPathForRole(role);

  return <Navigate to={nextPath} replace />;
};

export default DashboardPage;
