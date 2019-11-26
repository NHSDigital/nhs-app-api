export default {
  onError(_, errorMessage) {
    const timestamp = new Date();
    const createLogRequest = {
      TimeStamp: timestamp,
      Level: 'Error',
      Message: `${errorMessage}`,
    };

    this.dispatch('spinner/prevent', true);
    this.app.$http.postV1ApiLog(
      { createLogRequest, ignoreError: true },
    ).finally(() => {
      this.dispatch('spinner/prevent', false);
    });
  },
};
