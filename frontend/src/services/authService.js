import api from "./api";

const authService = {
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
  }
};

export default authService;
