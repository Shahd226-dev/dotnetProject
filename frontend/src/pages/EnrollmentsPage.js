import { useState } from "react";
import enrollmentService from "../services/enrollmentService";
import { useToast } from "../context/ToastContext";

const EnrollmentsPage = () => {
  const [form, setForm] = useState({ studentId: "", courseId: "" });
  const { addToast } = useToast();

  const handleSubmit = async (event) => {
    event.preventDefault();

    try {
      const response = await enrollmentService.enroll({
        studentId: Number(form.studentId),
        courseId: Number(form.courseId)
      });
      if (response.success) {
        addToast({
          type: "success",
          title: "Enrollment successful",
          message: response.message
        });
        setForm({ studentId: "", courseId: "" });
        return;
      }
      addToast({
        type: "error",
        title: "Enrollment failed",
        message: response.message || "Failed to enroll student."
      });
    } catch (err) {
      addToast({
        type: "error",
        title: "Enrollment failed",
        message: err.message || "Failed to enroll student."
      });
    }
  };

  return (
    <section>
      <div className="page-header">
        <h2>Enrollments</h2>
        <p className="muted">Enroll students into courses.</p>
      </div>

      <div className="card">
        <h3>Enroll student</h3>
        <form onSubmit={handleSubmit} className="form-grid">
          <div className="form-group">
            <label>Student Id</label>
            <input
              name="studentId"
              type="number"
              className="input"
              value={form.studentId}
              onChange={(event) =>
                setForm((prev) => ({
                  ...prev,
                  studentId: event.target.value
                }))
              }
              required
            />
          </div>
          <div className="form-group">
            <label>Course Id</label>
            <input
              name="courseId"
              type="number"
              className="input"
              value={form.courseId}
              onChange={(event) =>
                setForm((prev) => ({ ...prev, courseId: event.target.value }))
              }
              required
            />
          </div>
          <div className="actions">
            <button className="btn btn-primary" type="submit">
              Enroll
            </button>
          </div>
        </form>
      </div>
    </section>
  );
};

export default EnrollmentsPage;
