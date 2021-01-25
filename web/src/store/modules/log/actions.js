const sendRequest = (store, level, message) => {
  store.dispatch('spinner/prevent', true);
  store.app.$http.postV1ApiLog({
    createLogRequest: { TimeStamp: new Date(), Level: level, Message: message },
    ignoreError: true,
  }).finally(() => {
    store.dispatch('spinner/prevent', false);
  });
};

export default {
  onError(_, errorMessage) {
    sendRequest(this, 'Error', errorMessage);
  },
  onInfo(_, message) {
    sendRequest(this, 'Information', message);
  },
};
