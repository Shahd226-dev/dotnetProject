import api from "./axiosInstance";
import { withQuery } from "./apiHelpers";

const studentApi = {
  async getAll() {
    const response = await api.get("/students");
    return response.data;
  },
  async getById(id) {
    const response = await api.get(`/students/${id}`);
    return response.data;
  },
  async create(payload, userId) {
    const url = withQuery("/students", { userId });
    const response = await api.post(url, payload);
    return response.data;
  },
  async update(id, payload) {
    const response = await api.put(`/students/${id}`, payload);
    return response.data;
  }
};

export default studentApi;
