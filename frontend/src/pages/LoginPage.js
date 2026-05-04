import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { useToast } from "../context/ToastContext";

const LoginPage = () => {
  const { login, loading } = useAuth();
  const navigate = useNavigate();
  const { addToast } = useToast();
  const [form, setForm] = useState({ username: "", password: "" });

  const handleChange = (event) => {
    setForm((prev) => ({ ...prev, [event.target.name]: event.target.value }));
  };

  const handleSubmit = async (event) => {
    event.preventDefault();

    const response = await login(form);
    if (response.success) {
      addToast({ type: "success", title: "Welcome back", message: response.message });
      navigate("/dashboard");
      return;
    }

    addToast({
      type: "error",
      title: "Login failed",
      message: response.message || "Please check your credentials."
    });
  };

  return (
    <section className="card">
      <div className="page-header">
        <h2>Welcome back</h2>
        <p className="muted">Sign in to continue managing your campus.</p>
      </div>
      <form onSubmit={handleSubmit} className="form-grid">
        <div className="form-group">
          <label htmlFor="username">Username</label>
          <input
            id="username"
            name="username"
            className="input"
            value={form.username}
            onChange={handleChange}
            required
          />
        </div>
        <div className="form-group">
          <label htmlFor="password">Password</label>
          <input
            id="password"
            name="password"
            type="password"
            className="input"
            value={form.password}
            onChange={handleChange}
            required
          />
        </div>
        <div className="actions">
          <button className="btn btn-primary" type="submit" disabled={loading}>
            {loading ? "Signing in..." : "Login"}
          </button>
        </div>
      </form>
    </section>
  );
};

export default LoginPage;
