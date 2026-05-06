import api from "./axiosInstance";

const authApi = {
  async login(payload) {
    const response = await api.post("/auth/login", payload);
    return response.data;
  },
  async register(payload) {
    const response = await api.post("/auth/register", payload);
    return response.data;
  },
  async getUsers() {
    const response = await api.get("/auth/users");
    return response.data;
  },
  async deleteUser(userId) {
    const response = await api.delete(`/auth/users/${userId}`);
    return response.data;
  }
};

export default authApi;
