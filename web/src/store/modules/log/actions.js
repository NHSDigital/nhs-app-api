export default {
  onError(_, errorMessage) {
    const timestamp = new Date();
    const createLogRequest = {
      TimeStamp: timestamp,
      Level: 'Error',
      Message: `${errorMessage}`,
    };

    this.app.$http.postV1ApiLog(
      { createLogRequest },
    );
  },
};
