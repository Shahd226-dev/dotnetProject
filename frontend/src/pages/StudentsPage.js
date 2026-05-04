import { useEffect, useState } from "react";
import studentService from "../services/studentService";
import { useAuth } from "../context/AuthContext";
import { useToast } from "../context/ToastContext";
import SkeletonTable from "../components/SkeletonTable";
import EmptyState from "../components/EmptyState";
import Modal from "../components/Modal";

const StudentsPage = () => {
  const { isAdmin } = useAuth();
  const { addToast } = useToast();
  const [students, setStudents] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [form, setForm] = useState({ name: "", email: "", userId: "" });
  const [editingId, setEditingId] = useState(null);
  const [editForm, setEditForm] = useState({ name: "", userId: "" });

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

  const handleCreate = async (event) => {
    event.preventDefault();
    setError("");

    try {
      const response = await studentService.create({
        name: form.name,
        email: form.email,
        userId: Number(form.userId)
      });
      if (response.success) {
        addToast({ type: "success", title: "Student created", message: response.message });
        setStudents((prev) => [...prev, response.data]);
        setForm({ name: "", email: "", userId: "" });
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
    setEditForm({ name: student.name, userId: student.userId || "" });
  };

  const handleUpdate = async (event) => {
    event.preventDefault();
    if (!editingId) return;
    setError("");

    try {
      const response = await studentService.update(editingId, {
        name: editForm.name,
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
              <label>Name</label>
              <input
                name="name"
                className="input"
                value={form.name}
                onChange={(event) =>
                  setForm((prev) => ({ ...prev, name: event.target.value }))
                }
                required
              />
            </div>
            <div className="form-group">
              <label>Email</label>
              <input
                name="email"
                type="email"
                className="input"
                value={form.email}
                onChange={(event) =>
                  setForm((prev) => ({ ...prev, email: event.target.value }))
                }
                required
              />
            </div>
            <div className="form-group">
              <label>User Id</label>
              <input
                name="userId"
                type="number"
                className="input"
                value={form.userId}
                onChange={(event) =>
                  setForm((prev) => ({ ...prev, userId: event.target.value }))
                }
                required
              />
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
                <th>Name</th>
                <th>Email</th>
                <th>User Id</th>
                {isAdmin && <th>Actions</th>}
              </tr>
            </thead>
            <tbody>
              {students.map((student) => (
                <tr key={student.id}>
                  <td>{student.id}</td>
                  <td>{student.name}</td>
                  <td>{student.email}</td>
                  <td>{student.userId ?? "-"}</td>
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
              <label>Name</label>
              <input
                name="name"
                className="input"
                value={editForm.name}
                onChange={(event) =>
                  setEditForm((prev) => ({ ...prev, name: event.target.value }))
                }
                required
              />
            </div>
            <div className="form-group">
              <label>User Id</label>
              <input
                name="userId"
                type="number"
                className="input"
                value={editForm.userId}
                onChange={(event) =>
                  setEditForm((prev) => ({ ...prev, userId: event.target.value }))
                }
                required
              />
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
