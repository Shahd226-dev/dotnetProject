const UnauthorizedPage = () => {
  return (
    <section className="card">
      <div className="page-header">
        <h2>Access denied</h2>
        <p className="muted">
          You do not have permission to view this page. Switch roles or contact
          an administrator.
        </p>
      </div>
    </section>
  );
};

export default UnauthorizedPage;
