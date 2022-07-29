const sendRequest = async (store, level, message) => {
  store.dispatch('spinner/prevent', true);
  return store.app.$http.postV1ApiLog({
    createLogRequest: { TimeStamp: new Date(), Level: level, Message: message },
    ignoreError: true,
  }).finally(() => {
    store.dispatch('spinner/prevent', false);
  });
};

export default {
  async onError(_, errorMessage) {
    await sendRequest(this, 'Error', errorMessage);
  },
  async onInfo(_, message) {
    await sendRequest(this, 'Information', message);
  },
  async postOperationAudit(_, { operation, details }) {
    try {
      await this.app.$http.postV1ApiMetricsPostOperationAudit({
        operationAuditData: {
          operation,
          details,
        },
      });
    } catch {
      // do nothing as this is just logging
    }
  },
};
