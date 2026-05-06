import { useEffect, useState } from "react";
import courseApi from "../api/courseApi";
import instructorApi from "../api/instructorApi";
import { useAuth } from "../context/AuthContext";
import { useToast } from "../context/ToastContext";
import SkeletonTable from "../components/SkeletonTable";
import EmptyState from "../components/EmptyState";
import Modal from "../components/Modal";

const CoursesPage = () => {
  const { isAdmin, isInstructor } = useAuth();
  const canManageCourses = isAdmin || isInstructor;
  const { addToast } = useToast();
  const [courses, setCourses] = useState([]);
  const [instructors, setInstructors] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [form, setForm] = useState({ title: "", description: "", instructorId: "" });
  const [editingId, setEditingId] = useState(null);
  const [editForm, setEditForm] = useState({ title: "", description: "", instructorId: "" });

  const loadCourses = async () => {
    setLoading(true);
    setError("");
    try {
      const response = await courseApi.getAll();
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

  useEffect(() => {
    if (!isAdmin) return;

    const loadInstructors = async () => {
      try {
        const response = await instructorApi.getAll();
        if (response.success) {
          setInstructors(response.data || []);
        }
      } catch {
        // Instructor list is optional for non-admin actions.
      }
    };

    loadInstructors();
  }, [isAdmin]);

  const handleCreate = async (event) => {
    event.preventDefault();
    setError("");

    try {
      const response = await courseApi.create(
        { title: form.title, description: form.description },
        isAdmin ? Number(form.instructorId) : undefined
      );
      if (response.success) {
        addToast({ type: "success", title: "Course created", message: response.message });
        setCourses((prev) => [...prev, response.data]);
        setForm({ title: "", description: "", instructorId: "" });
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
      description: course.description || "",
      instructorId: course.instructor?.id || ""
    });
  };

  const handleUpdate = async (event) => {
    event.preventDefault();
    if (!editingId) return;
    setError("");

    try {
      const response = await courseApi.update(
        editingId,
        { title: editForm.title, description: editForm.description },
        isAdmin ? Number(editForm.instructorId) : undefined
      );
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

      {canManageCourses && (
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
              <label>Description</label>
              <textarea
                name="description"
                className="input"
                rows="3"
                value={form.description}
                onChange={(event) =>
                  setForm((prev) => ({
                    ...prev,
                    description: event.target.value
                  }))
                }
              />
            </div>
            {isAdmin && (
              <div className="form-group">
                <label>Assign instructor</label>
                <select
                  name="instructorId"
                  className="select"
                  value={form.instructorId}
                  onChange={(event) =>
                    setForm((prev) => ({
                      ...prev,
                      instructorId: event.target.value
                    }))
                  }
                >
                  <option value="">Select instructor (optional)</option>
                  {instructors.map((instructor) => (
                    <option key={instructor.id} value={instructor.id}>
                      {instructor.fullName}
                    </option>
                  ))}
                </select>
              </div>
            )}
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
                <th>Description</th>
                <th>Instructor</th>
                {(isAdmin || isInstructor) && <th>Actions</th>}
              </tr>
            </thead>
            <tbody>
              {courses.map((course) => (
                <tr key={course.id}>
                  <td>{course.id}</td>
                  <td>{course.title}</td>
                  <td>{course.description || "-"}</td>
                  <td>{course.instructor?.fullName || "-"}</td>
                  {(isAdmin || isInstructor) && (
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
              <label>Description</label>
              <textarea
                name="description"
                className="input"
                rows="3"
                value={editForm.description}
                onChange={(event) =>
                  setEditForm((prev) => ({
                    ...prev,
                    description: event.target.value
                  }))
                }
              />
            </div>
            {isAdmin && (
              <div className="form-group">
                <label>Assign instructor</label>
                <select
                  name="instructorId"
                  className="select"
                  value={editForm.instructorId}
                  onChange={(event) =>
                    setEditForm((prev) => ({
                      ...prev,
                      instructorId: event.target.value
                    }))
                  }
                >
                  <option value="">Keep current instructor</option>
                  {instructors.map((instructor) => (
                    <option key={instructor.id} value={instructor.id}>
                      {instructor.fullName}
                    </option>
                  ))}
                </select>
              </div>
            )}
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
