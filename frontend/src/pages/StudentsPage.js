import { useEffect, useState } from "react";
import studentService from "../services/studentService";
import authService from "../services/authService";
import { useAuth } from "../context/AuthContext";
import { useToast } from "../context/ToastContext";
import SkeletonTable from "../components/SkeletonTable";
import EmptyState from "../components/EmptyState";
import Modal from "../components/Modal";

const StudentsPage = () => {
  const { isAdmin } = useAuth();
  const { addToast } = useToast();
  const [students, setStudents] = useState([]);
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [form, setForm] = useState({ fullName: "", userId: "" });
  const [editingId, setEditingId] = useState(null);
  const [editForm, setEditForm] = useState({ fullName: "", userId: "" });

  const loadStudents = async () => {
    setLoading(true);
    setError("");
    try {
      const response = await studentService.getAll();
      if (response.success) {
        setStudents(response.data || []);
      } else {
        setError(response.message || "Failed to load students.");
      }
    } catch (err) {
      setError(err.message || "Failed to load students.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadStudents();
  }, []);

  useEffect(() => {
    if (isAdmin) {
      loadUsers();
    }
  }, [isAdmin]);

  const loadUsers = async () => {
    try {
      const response = await authService.getUsers();
      if (response.success) {
        setUsers(response.data || []);
      }
    } catch {
      // Ignore user list failures; admin can refresh or use API directly.
    }
  };

  const studentUsers = users.filter((user) => user.role === "Student");

  const handleCreate = async (event) => {
    event.preventDefault();
    setError("");

    try {
      const response = await studentService.create({
        fullName: form.fullName,
        userId: Number(form.userId)
      });
      if (response.success) {
        addToast({ type: "success", title: "Student created", message: response.message });
        setStudents((prev) => [...prev, response.data]);
        setForm({ fullName: "", userId: "" });
        return;
      }
      addToast({
        type: "error",
        title: "Create failed",
        message: response.message || "Failed to create student."
      });
    } catch (err) {
      addToast({
        type: "error",
        title: "Create failed",
        message: err.message || "Failed to create student."
      });
    }
  };

  const startEdit = (student) => {
    setEditingId(student.id);
    setEditForm({ fullName: student.fullName, userId: student.userId || "" });
  };

  const handleUpdate = async (event) => {
    event.preventDefault();
    if (!editingId) return;
    setError("");

    try {
      const response = await studentService.update(editingId, {
        fullName: editForm.fullName,
        userId: Number(editForm.userId)
      });
      if (response.success) {
        addToast({ type: "success", title: "Student updated", message: response.message });
        setStudents((prev) =>
          prev.map((item) => (item.id === editingId ? response.data : item))
        );
        setEditingId(null);
        return;
      }
      addToast({
        type: "error",
        title: "Update failed",
        message: response.message || "Failed to update student."
      });
    } catch (err) {
      addToast({
        type: "error",
        title: "Update failed",
        message: err.message || "Failed to update student."
      });
    }
  };

  return (
    <section>
      <div className="page-header">
        <h2>Students</h2>
        <p className="muted">View and manage student records.</p>
      </div>

      {isAdmin && (
        <div className="card">
          <h3>Create student</h3>
          <form onSubmit={handleCreate} className="form-grid">
            <div className="form-group">
              <label>Full name</label>
              <input
                name="fullName"
                className="input"
                value={form.fullName}
                onChange={(event) =>
                  setForm((prev) => ({ ...prev, fullName: event.target.value }))
                }
                required
              />
            </div>
            <div className="form-group">
              <label>Account</label>
              <select
                name="userId"
                className="select"
                value={form.userId}
                onChange={(event) =>
                  setForm((prev) => ({ ...prev, userId: event.target.value }))
                }
                required
              >
                <option value="">Select a student account</option>
                {studentUsers.map((user) => (
                  <option key={user.id} value={user.id}>
                    {user.username} ({user.email})
                  </option>
                ))}
              </select>
            </div>
            <div className="actions">
              <button className="btn btn-primary" type="submit">
                Create
              </button>
            </div>
          </form>
        </div>
      )}

      <div className="card">
        <h3>Student list</h3>
        {loading ? (
          <SkeletonTable />
        ) : students.length > 0 ? (
          <table className="table">
            <thead>
              <tr>
                <th>Id</th>
                <th>Full name</th>
                {isAdmin && <th>Actions</th>}
              </tr>
            </thead>
            <tbody>
              {students.map((student) => (
                <tr key={student.id}>
                  <td>{student.id}</td>
                  <td>{student.fullName}</td>
                  {isAdmin && (
                    <td>
                      <button
                        className="btn btn-secondary"
                        onClick={() => startEdit(student)}
                      >
                        Edit
                      </button>
                    </td>
                  )}
                </tr>
              ))}
            </tbody>
          </table>
        ) : (
          <EmptyState title="No students yet" description="Add your first student record to get started." />
        )}
      </div>

      {isAdmin && editingId && (
        <Modal
          title={`Edit student #${editingId}`}
          description="Update the student profile details."
          onClose={() => setEditingId(null)}
        >
          <form onSubmit={handleUpdate} className="form-grid">
            <div className="form-group">
              <label>Full name</label>
              <input
                name="fullName"
                className="input"
                value={editForm.fullName}
                onChange={(event) =>
                  setEditForm((prev) => ({ ...prev, fullName: event.target.value }))
                }
                required
              />
            </div>
            <div className="form-group">
              <label>Account</label>
              <select
                name="userId"
                className="select"
                value={editForm.userId}
                onChange={(event) =>
                  setEditForm((prev) => ({ ...prev, userId: event.target.value }))
                }
                required
              >
                <option value="">Select a student account</option>
                {studentUsers.map((user) => (
                  <option key={user.id} value={user.id}>
                    {user.username} ({user.email})
                  </option>
                ))}
              </select>
            </div>
            <div className="actions">
              <button className="btn btn-primary" type="submit">
                Save changes
              </button>
              <button
                className="btn btn-secondary"
                type="button"
                onClick={() => setEditingId(null)}
              >
                Cancel
              </button>
            </div>
          </form>
        </Modal>
      )}

      {error && (
        <div className="card">
          <EmptyState title="Unable to load students" description={error} />
        </div>
      )}
    </section>
  );
};

export default StudentsPage;
