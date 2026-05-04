import api from "./api";

const instructorService = {
  async getAll() {
    const response = await api.get("/instructors");
    return response.data;
  },
  async getById(id) {
    const response = await api.get(`/instructors/${id}`);
    return response.data;
  },
  async create(payload) {
    const response = await api.post("/instructors", payload);
    return response.data;
  },
  async update(id, payload) {
    const response = await api.put(`/instructors/${id}`, payload);
    return response.data;
  }
};

export default instructorService;
