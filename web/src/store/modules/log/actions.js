export default {
  onError(_, errorMessage) {
    const timestamp = new Date();
    const createLogRequest = {
      TimeStamp: timestamp,
      Level: 'Error',
      Message: `${errorMessage}`,
    };

    this.dispatch('spinner/prevent', false);
    this.app.$http.postV1ApiLog(
      { createLogRequest, ignoreError: true },
    );
    this.dispatch('spinner/prevent', true);
  },
};
