import { useEffect, useState } from "react";
import instructorService from "../services/instructorService";
import authService from "../services/authService";
import { useAuth } from "../context/AuthContext";
import { useToast } from "../context/ToastContext";
import SkeletonTable from "../components/SkeletonTable";
import EmptyState from "../components/EmptyState";
import Modal from "../components/Modal";

const InstructorsPage = () => {
  const { isAdmin } = useAuth();
  const { addToast } = useToast();
  const [instructors, setInstructors] = useState([]);
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [form, setForm] = useState({ fullName: "", userId: "" });
  const [editingId, setEditingId] = useState(null);
  const [editForm, setEditForm] = useState({ fullName: "", userId: "" });

  const loadInstructors = async () => {
    setLoading(true);
    setError("");
    try {
      const response = await instructorService.getAll();
      if (response.success) {
        setInstructors(response.data || []);
      } else {
        setError(response.message || "Failed to load instructors.");
      }
    } catch (err) {
      setError(err.message || "Failed to load instructors.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadInstructors();
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

  const instructorUsers = users.filter((user) => user.role === "Instructor");

  const handleCreate = async (event) => {
    event.preventDefault();
    setError("");

    try {
      const response = await instructorService.create({
        fullName: form.fullName,
        userId: Number(form.userId)
      });
      if (response.success) {
        addToast({
          type: "success",
          title: "Instructor created",
          message: response.message
        });
        setInstructors((prev) => [...prev, response.data]);
        setForm({ fullName: "", userId: "" });
        return;
      }
      addToast({
        type: "error",
        title: "Create failed",
        message: response.message || "Failed to create instructor."
      });
    } catch (err) {
      addToast({
        type: "error",
        title: "Create failed",
        message: err.message || "Failed to create instructor."
      });
    }
  };

  const startEdit = (instructor) => {
    setEditingId(instructor.id);
    setEditForm({ fullName: instructor.fullName, userId: instructor.userId || "" });
  };

  const handleUpdate = async (event) => {
    event.preventDefault();
    if (!editingId) return;
    setError("");

    try {
      const response = await instructorService.update(editingId, {
        fullName: editForm.fullName,
        userId: Number(editForm.userId)
      });
      if (response.success) {
        addToast({
          type: "success",
          title: "Instructor updated",
          message: response.message
        });
        setInstructors((prev) =>
          prev.map((item) => (item.id === editingId ? response.data : item))
        );
        setEditingId(null);
        return;
      }
      addToast({
        type: "error",
        title: "Update failed",
        message: response.message || "Failed to update instructor."
      });
    } catch (err) {
      addToast({
        type: "error",
        title: "Update failed",
        message: err.message || "Failed to update instructor."
      });
    }
  };

  return (
    <section>
      <div className="page-header">
        <h2>Instructors</h2>
        <p className="muted">Manage instructor records and assignments.</p>
      </div>

      {isAdmin && (
        <div className="card">
          <h3>Create instructor</h3>
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
                <option value="">Select an instructor account</option>
                {instructorUsers.map((user) => (
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
        <h3>Instructor list</h3>
        {loading ? (
          <SkeletonTable />
        ) : instructors.length > 0 ? (
          <table className="table">
            <thead>
              <tr>
                <th>Id</th>
                <th>Full name</th>
                {isAdmin && <th>Actions</th>}
              </tr>
            </thead>
            <tbody>
              {instructors.map((instructor) => (
                <tr key={instructor.id}>
                  <td>{instructor.id}</td>
                  <td>{instructor.fullName}</td>
                  {isAdmin && (
                    <td>
                      <button
                        className="btn btn-secondary"
                        onClick={() => startEdit(instructor)}
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
          <EmptyState title="No instructors yet" description="Create your first instructor profile." />
        )}
      </div>

      {isAdmin && editingId && (
        <Modal
          title={`Edit instructor #${editingId}`}
          description="Update the instructor details."
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
                <option value="">Select an instructor account</option>
                {instructorUsers.map((user) => (
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
          <EmptyState title="Unable to load instructors" description={error} />
        </div>
      )}
    </section>
  );
};

export default InstructorsPage;
