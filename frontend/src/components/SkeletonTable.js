const SkeletonTable = ({ rows = 4, columns = 4 }) => {
  const rowArray = Array.from({ length: rows });
  const colArray = Array.from({ length: columns });

  return (
    <div className="skeleton-table">
      {rowArray.map((_, rowIndex) => (
        <div key={`row-${rowIndex}`} className="skeleton-row">
          {colArray.map((__, colIndex) => (
            <div key={`col-${colIndex}`} className="skeleton-cell" />
          ))}
        </div>
      ))}
    </div>
  );
};

export default SkeletonTable;
