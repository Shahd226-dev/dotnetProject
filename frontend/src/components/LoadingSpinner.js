const LoadingSpinner = ({ label = "Loading..." }) => {
  return (
    <div className="spinner">
      <span className="spinner-circle" />
      <span className="muted">{label}</span>
    </div>
  );
};

export default LoadingSpinner;
