export const getDashboardPathForRole = (role) => {
  switch (role) {
    case "Admin":
      return "/dashboard/admin";
    case "Instructor":
      return "/dashboard/instructor";
    case "Student":
      return "/dashboard/student";
    default:
      return "/login";
  }
};
