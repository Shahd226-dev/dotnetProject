import api from "./axiosInstance";
import { withQuery } from "./apiHelpers";

const courseApi = {
  async getAll() {
    const response = await api.get("/courses");
    return response.data;
  },
  async getById(id) {
    const response = await api.get(`/courses/${id}`);
    return response.data;
  },
  async create(payload, instructorId) {
    const url = withQuery("/courses", { instructorId });
    const response = await api.post(url, payload);
    return response.data;
  },
  async update(id, payload, instructorId) {
    const url = withQuery(`/courses/${id}`, { instructorId });
    const response = await api.put(url, payload);
    return response.data;
  },
  async remove(id) {
    const response = await api.delete(`/courses/${id}`);
    return response.data;
  }
};

export default courseApi;
