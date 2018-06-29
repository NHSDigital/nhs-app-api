module.exports = {
  setRes(response) {
    if (process.client) return;
    this.res = response;
  },

  setSessionId(sessionId) {
    if (process.client) return;
    this.res.setHeader('Set-Cookie', sessionId);
  },
};
