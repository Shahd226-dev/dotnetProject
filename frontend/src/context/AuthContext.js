import { createContext, useContext, useState } from "react";
import authService from "../services/authService";

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(() => {
    const storedUser = localStorage.getItem("authUser");
    return storedUser ? JSON.parse(storedUser) : null;
  });
  const [token, setToken] = useState(() => localStorage.getItem("accessToken"));
  const [loading, setLoading] = useState(false);

  const isAuthenticated = Boolean(token);
  const role = user?.role || "";

  const login = async (credentials) => {
    setLoading(true);
    try {
      const response = await authService.login(credentials);
      if (response.success && response.data?.accessToken && response.data?.user) {
        localStorage.setItem("accessToken", response.data.accessToken);
        localStorage.setItem("authUser", JSON.stringify(response.data.user));
        setToken(response.data.accessToken);
        setUser(response.data.user);
      }
      return response;
    } catch (error) {
      return { success: false, message: error.message || "Login failed." };
    } finally {
      setLoading(false);
    }
  };

  const register = async (payload) => {
    setLoading(true);
    try {
      return await authService.register(payload);
    } catch (error) {
      return { success: false, message: error.message || "Registration failed." };
    } finally {
      setLoading(false);
    }
  };

  const logout = async () => {
    setLoading(true);
    localStorage.removeItem("accessToken");
    localStorage.removeItem("authUser");
    setToken(null);
    setUser(null);
    setLoading(false);
  };

  return (
    <AuthContext.Provider
      value={{
        user,
        token,
        role,
        loading,
        isAuthenticated,
        isAdmin: role === "Admin",
        isInstructor: role === "Instructor",
        login,
        register,
        logout
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => useContext(AuthContext);
