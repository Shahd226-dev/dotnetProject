import api from "./api";

const enrollmentService = {
  async enroll(payload) {
    const response = await api.post("/enrollments", payload);
    return response.data;
  }
};

export default enrollmentService;
