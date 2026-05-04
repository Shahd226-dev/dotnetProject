import { useEffect, useState } from "react";
import instructorService from "../services/instructorService";
import { useAuth } from "../context/AuthContext";
import { useToast } from "../context/ToastContext";
import SkeletonTable from "../components/SkeletonTable";
import EmptyState from "../components/EmptyState";
import Modal from "../components/Modal";

const InstructorsPage = () => {
  const { isAdmin } = useAuth();
  const { addToast } = useToast();
  const [instructors, setInstructors] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [form, setForm] = useState({ name: "", userId: "" });
  const [editingId, setEditingId] = useState(null);
  const [editForm, setEditForm] = useState({ name: "", userId: "" });

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

  const handleCreate = async (event) => {
    event.preventDefault();
    setError("");

    try {
      const response = await instructorService.create({
        name: form.name,
        userId: Number(form.userId)
      });
      if (response.success) {
        addToast({
          type: "success",
          title: "Instructor created",
          message: response.message
        });
        setInstructors((prev) => [...prev, response.data]);
        setForm({ name: "", userId: "" });
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
    setEditForm({ name: instructor.name, userId: instructor.userId || "" });
  };

  const handleUpdate = async (event) => {
    event.preventDefault();
    if (!editingId) return;
    setError("");

    try {
      const response = await instructorService.update(editingId, {
        name: editForm.name,
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
        <h3>Instructor list</h3>
        {loading ? (
          <SkeletonTable />
        ) : instructors.length > 0 ? (
          <table className="table">
            <thead>
              <tr>
                <th>Id</th>
                <th>Name</th>
                <th>User Id</th>
                {isAdmin && <th>Actions</th>}
              </tr>
            </thead>
            <tbody>
              {instructors.map((instructor) => (
                <tr key={instructor.id}>
                  <td>{instructor.id}</td>
                  <td>{instructor.name}</td>
                  <td>{instructor.userId ?? "-"}</td>
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
          <EmptyState title="Unable to load instructors" description={error} />
        </div>
      )}
    </section>
  );
};

export default InstructorsPage;
