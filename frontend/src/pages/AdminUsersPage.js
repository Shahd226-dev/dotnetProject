import { useEffect, useMemo, useState } from "react";
import authApi from "../api/authApi";
import { useToast } from "../context/ToastContext";
import EmptyState from "../components/EmptyState";
import SkeletonTable from "../components/SkeletonTable";
import Modal from "../components/Modal";

const AdminUsersPage = () => {
  const { addToast } = useToast();
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [roleFilter, setRoleFilter] = useState("All");
  const [deleteTarget, setDeleteTarget] = useState(null);

  const loadUsers = async () => {
    setLoading(true);
    setError("");
    try {
      const response = await authApi.getUsers();
      if (response.success) {
        setUsers(response.data || []);
      } else {
        setError(response.message || "Failed to load users.");
      }
    } catch (err) {
      setError(err.message || "Failed to load users.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadUsers();
  }, []);

  const filteredUsers = useMemo(() => {
    if (roleFilter === "All") return users;
    return users.filter((user) => user.role === roleFilter);
  }, [users, roleFilter]);

  const confirmDelete = async () => {
    if (!deleteTarget) return;

    try {
      const response = await authApi.deleteUser(deleteTarget.id);
      if (response.success) {
        addToast({
          type: "success",
          title: "User deleted",
          message: response.message
        });
        setUsers((prev) => prev.filter((item) => item.id !== deleteTarget.id));
        setDeleteTarget(null);
        return;
      }

      addToast({
        type: "error",
        title: "Delete failed",
        message: response.message || "Unable to delete user."
      });
    } catch (err) {
      addToast({
        type: "error",
        title: "Delete failed",
        message: err.message || "Unable to delete user."
      });
    }
  };

  return (
    <section>
      <div className="page-header">
        <h2>Users</h2>
        <p className="muted">Manage user access and roles across the platform.</p>
      </div>

      <div className="card">
        <div className="filter-row">
          <div className="form-group">
            <label>Role filter</label>
            <select
              className="select"
              value={roleFilter}
              onChange={(event) => setRoleFilter(event.target.value)}
            >
              <option value="All">All roles</option>
              <option value="Admin">Admin</option>
              <option value="Instructor">Instructor</option>
              <option value="Student">Student</option>
            </select>
          </div>
          <button className="btn btn-secondary" onClick={loadUsers}>
            Refresh
          </button>
        </div>

        {loading ? (
          <SkeletonTable />
        ) : filteredUsers.length ? (
          <table className="table">
            <thead>
              <tr>
                <th>Id</th>
                <th>Username</th>
                <th>Email</th>
                <th>Role</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {filteredUsers.map((user) => (
                <tr key={user.id}>
                  <td>{user.id}</td>
                  <td>{user.username}</td>
                  <td>{user.email}</td>
                  <td>
                    <span className="badge badge-neutral">{user.role}</span>
                  </td>
                  <td>
                    <button
                      className="btn btn-danger"
                      onClick={() => setDeleteTarget(user)}
                    >
                      Delete
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        ) : (
          <EmptyState title="No users found" description="Try adjusting the filters." />
        )}
      </div>

      {error && (
        <div className="card">
          <EmptyState title="Unable to load users" description={error} />
        </div>
      )}

      {deleteTarget && (
        <Modal
          title={`Delete ${deleteTarget.username}?`}
          description="This action cannot be undone."
          onClose={() => setDeleteTarget(null)}
        >
          <div className="actions">
            <button className="btn btn-danger" onClick={confirmDelete}>
              Confirm delete
            </button>
            <button
              className="btn btn-secondary"
              type="button"
              onClick={() => setDeleteTarget(null)}
            >
              Cancel
            </button>
          </div>
        </Modal>
      )}
    </section>
  );
};

export default AdminUsersPage;
