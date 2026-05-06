import { createContext, useCallback, useContext, useEffect, useMemo, useState } from "react";
import authApi from "../api/authApi";
import { setUnauthorizedHandler } from "../api/axiosInstance";
import { isTokenExpired } from "../utils/token";

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
      const response = await authApi.login(credentials);
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
      return await authApi.register(payload);
    } catch (error) {
      return { success: false, message: error.message || "Registration failed." };
    } finally {
      setLoading(false);
    }
  };

  const logout = useCallback(() => {
    localStorage.removeItem("accessToken");
    localStorage.removeItem("authUser");
    setToken(null);
    setUser(null);
  }, []);

  useEffect(() => {
    if (token && isTokenExpired(token)) {
      logout();
    }
  }, [token, logout]);

  useEffect(() => {
    setUnauthorizedHandler(() => {
      logout();
    });

    return () => {
      setUnauthorizedHandler(null);
    };
  }, [logout]);

  useEffect(() => {
    if (!token) return;

    const interval = setInterval(() => {
      if (isTokenExpired(token)) {
        logout();
      }
    }, 15000);

    return () => clearInterval(interval);
  }, [token, logout]);

  const value = useMemo(
    () => ({
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
    }),
    [user, token, role, loading, isAuthenticated, login, register, logout]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

export const useAuth = () => useContext(AuthContext);
