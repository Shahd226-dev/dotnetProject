import api from "./axiosInstance";

const enrollmentApi = {
  async enroll(payload) {
    const response = await api.post("/enrollments", payload);
    return response.data;
  },
  async unenroll(courseId) {
    const response = await api.delete(`/enrollments/${courseId}`);
    return response.data;
  },
  async getMyEnrollments() {
    const response = await api.get("/enrollments/me");
    return response.data;
  },
  async getInstructorEnrollments() {
    const response = await api.get("/enrollments/instructor");
    return response.data;
  }
};

export default enrollmentApi;
