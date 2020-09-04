export default {
  notifications: {
    pageTitle: 'Notifications error',
    pageHeader: 'Notifications error',
    header: 'Sorry, there is a problem with the service',
    subheader: '',
    message: 'Go back to settings and try again.',
    retryButtonText: 'Back to settings',
    500: {
      10001: {
        header: 'Notifications are turned off on your device',
        message: 'To turn on notifications, go to your device settings and allow notifications. Then return to the app and try again.',
        retryButtonText: 'Try again',
      },
      10002: {
        header: 'Sorry, we could not change your notifications choice',
        message: 'This might be because notifications are turned off in your device settings.',
        additionalInfo: 'Go to your device settings and check notifications are turned on, then try again.',
        retryButtonText: 'Try again',
      },
    },
  },
};
