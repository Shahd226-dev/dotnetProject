const decodeBase64 = (value) => {
  const normalized = value.replace(/-/g, "+").replace(/_/g, "/");
  const padded = normalized.padEnd(normalized.length + ((4 - (normalized.length % 4)) % 4), "=");
  return atob(padded);
};

export const parseJwt = (token) => {
  try {
    const payload = token.split(".")[1];
    if (!payload) return null;
    return JSON.parse(decodeBase64(payload));
  } catch {
    return null;
  }
};

export const isTokenExpired = (token, leewaySeconds = 30) => {
  const payload = parseJwt(token);
  if (!payload?.exp) return false;
  const now = Date.now() / 1000;
  return payload.exp < now + leewaySeconds;
};
