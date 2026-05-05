import { useState } from "react";
import { useAuth } from "../context/AuthContext";
import { useToast } from "../context/ToastContext";

const RegisterPage = () => {
  const { register, loading } = useAuth();
  const { addToast } = useToast();
  const [form, setForm] = useState({
    username: "",
    email: "",
    fullName: "",
    bio: "",
    password: "",
    role: "Student"
  });

  const handleChange = (event) => {
    setForm((prev) => ({ ...prev, [event.target.name]: event.target.value }));
  };

  const handleSubmit = async (event) => {
    event.preventDefault();

    if (form.role !== "Admin" && !form.fullName.trim()) {
      addToast({
        type: "error",
        title: "Registration failed",
        message: "Full name is required for students and instructors."
      });
      return;
    }

    const response = await register(form);
    if (response.success) {
      addToast({
        type: "success",
        title: "Account created",
        message: response.message || "Student registered."
      });
      setForm({
        username: "",
        email: "",
        fullName: "",
        bio: "",
        password: "",
        role: "Student"
      });
      return;
    }

    addToast({
      type: "error",
      title: "Registration failed",
      message: response.message || "Please review your details."
    });
  };

  return (
    <section className="card">
      <div className="page-header">
        <h2>Create account</h2>
        <p className="muted">Set up access for your team.</p>
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
          <label htmlFor="email">Email</label>
          <input
            id="email"
            name="email"
            type="email"
            className="input"
            value={form.email}
            onChange={handleChange}
            required
          />
        </div>
        <div className="form-group">
          <label htmlFor="fullName">Full name</label>
          <input
            id="fullName"
            name="fullName"
            className="input"
            value={form.fullName}
            onChange={handleChange}
            required={form.role !== "Admin"}
          />
        </div>
        {form.role === "Instructor" && (
          <div className="form-group">
            <label htmlFor="bio">Bio</label>
            <textarea
              id="bio"
              name="bio"
              className="input"
              rows="3"
              value={form.bio}
              onChange={handleChange}
            />
          </div>
        )}
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
        <div className="form-group">
          <label htmlFor="role">Role</label>
          <select
            id="role"
            name="role"
            className="select"
            value={form.role}
            onChange={handleChange}
          >
            <option value="Student">Student</option>
            <option value="Instructor">Instructor</option>
            <option value="Admin">Admin</option>
          </select>
        </div>
        <div className="actions">
          <button className="btn btn-primary" type="submit" disabled={loading}>
            {loading ? "Creating..." : "Register"}
          </button>
        </div>
      </form>
    </section>
  );
};

export default RegisterPage;
