import { useEffect, useState } from "react";
import courseService from "../services/courseService";
import { useAuth } from "../context/AuthContext";
import { useToast } from "../context/ToastContext";
import SkeletonTable from "../components/SkeletonTable";
import EmptyState from "../components/EmptyState";
import Modal from "../components/Modal";

const CoursesPage = () => {
  const { isAdmin } = useAuth();
  const { addToast } = useToast();
  const [courses, setCourses] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [form, setForm] = useState({ title: "", instructorId: "" });
  const [editingId, setEditingId] = useState(null);
  const [editForm, setEditForm] = useState({ title: "", instructorId: "" });

  const loadCourses = async () => {
    setLoading(true);
    setError("");
    try {
      const response = await courseService.getAll();
      if (response.success) {
        setCourses(response.data || []);
      } else {
        setError(response.message || "Failed to load courses.");
      }
    } catch (err) {
      setError(err.message || "Failed to load courses.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadCourses();
  }, []);

  const handleCreate = async (event) => {
    event.preventDefault();
    setError("");

    try {
      const response = await courseService.create({
        title: form.title,
        instructorId: Number(form.instructorId)
      });
      if (response.success) {
        addToast({ type: "success", title: "Course created", message: response.message });
        setCourses((prev) => [...prev, response.data]);
        setForm({ title: "", instructorId: "" });
        return;
      }
      addToast({
        type: "error",
        title: "Create failed",
        message: response.message || "Failed to create course."
      });
    } catch (err) {
      addToast({
        type: "error",
        title: "Create failed",
        message: err.message || "Failed to create course."
      });
    }
  };

  const startEdit = (course) => {
    setEditingId(course.id);
    setEditForm({
      title: course.title,
      instructorId: course.instructorId || ""
    });
  };

  const handleUpdate = async (event) => {
    event.preventDefault();
    if (!editingId) return;
    setError("");

    try {
      const response = await courseService.update(editingId, {
        title: editForm.title,
        instructorId: Number(editForm.instructorId)
      });
      if (response.success) {
        addToast({ type: "success", title: "Course updated", message: response.message });
        setCourses((prev) =>
          prev.map((item) => (item.id === editingId ? response.data : item))
        );
        setEditingId(null);
        return;
      }
      addToast({
        type: "error",
        title: "Update failed",
        message: response.message || "Failed to update course."
      });
    } catch (err) {
      addToast({
        type: "error",
        title: "Update failed",
        message: err.message || "Failed to update course."
      });
    }
  };

  return (
    <section>
      <div className="page-header">
        <h2>Courses</h2>
        <p className="muted">Create and manage course offerings.</p>
      </div>

      {isAdmin && (
        <div className="card">
          <h3>Create course</h3>
          <form onSubmit={handleCreate} className="form-grid">
            <div className="form-group">
              <label>Title</label>
              <input
                name="title"
                className="input"
                value={form.title}
                onChange={(event) =>
                  setForm((prev) => ({ ...prev, title: event.target.value }))
                }
                required
              />
            </div>
            <div className="form-group">
              <label>Instructor Id</label>
              <input
                name="instructorId"
                type="number"
                className="input"
                value={form.instructorId}
                onChange={(event) =>
                  setForm((prev) => ({
                    ...prev,
                    instructorId: event.target.value
                  }))
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
        <h3>Course list</h3>
        {loading ? (
          <SkeletonTable />
        ) : courses.length > 0 ? (
          <table className="table">
            <thead>
              <tr>
                <th>Id</th>
                <th>Title</th>
                <th>Instructor Id</th>
                {isAdmin && <th>Actions</th>}
              </tr>
            </thead>
            <tbody>
              {courses.map((course) => (
                <tr key={course.id}>
                  <td>{course.id}</td>
                  <td>{course.title}</td>
                  <td>{course.instructorId}</td>
                  {isAdmin && (
                    <td>
                      <button
                        className="btn btn-secondary"
                        onClick={() => startEdit(course)}
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
          <EmptyState title="No courses yet" description="Create your first course offering." />
        )}
      </div>

      {isAdmin && editingId && (
        <Modal
          title={`Edit course #${editingId}`}
          description="Adjust course title and instructor assignment."
          onClose={() => setEditingId(null)}
        >
          <form onSubmit={handleUpdate} className="form-grid">
            <div className="form-group">
              <label>Title</label>
              <input
                name="title"
                className="input"
                value={editForm.title}
                onChange={(event) =>
                  setEditForm((prev) => ({ ...prev, title: event.target.value }))
                }
                required
              />
            </div>
            <div className="form-group">
              <label>Instructor Id</label>
              <input
                name="instructorId"
                type="number"
                className="input"
                value={editForm.instructorId}
                onChange={(event) =>
                  setEditForm((prev) => ({
                    ...prev,
                    instructorId: event.target.value
                  }))
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
          <EmptyState title="Unable to load courses" description={error} />
        </div>
      )}
    </section>
  );
};

export default CoursesPage;
