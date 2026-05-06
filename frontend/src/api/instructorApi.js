import api from "./axiosInstance";
import { withQuery } from "./apiHelpers";

const instructorApi = {
  async getAll() {
    const response = await api.get("/instructors");
    return response.data;
  },
  async getById(id) {
    const response = await api.get(`/instructors/${id}`);
    return response.data;
  },
  async create(payload, userId) {
    const url = withQuery("/instructors", { userId });
    const response = await api.post(url, payload);
    return response.data;
  },
  async update(id, payload) {
    const response = await api.put(`/instructors/${id}`, payload);
    return response.data;
  }
};

export default instructorApi;
