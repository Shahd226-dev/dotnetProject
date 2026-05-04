import { useToast } from "../context/ToastContext";

const ToastContainer = () => {
  const { toasts } = useToast();

  return (
    <div className="toast-container" aria-live="polite">
      {toasts.map((toast) => (
        <div
          key={toast.id}
          className={`toast toast-${toast.type || "info"}`}
        >
          <strong>{toast.title}</strong>
          {toast.message && <p>{toast.message}</p>}
        </div>
      ))}
    </div>
  );
};

export default ToastContainer;
