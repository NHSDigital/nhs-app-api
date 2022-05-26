export default {
  account_and_settings: {
    manage_notifications: {
      pageTitle: 'Cannot update notification preferences',
      pageHeader: 'Cannot update notification preferences',
      header: ' ',
      subheader: '',
      message: 'To try again, go back to <strong>Account and settings</strong> and choose <strong>Manage notifications.</strong>',
      retryButtonText: 'Back to Account and settings',
      500: {
        10001: {
          header: 'Notifications are turned off on your device',
          message: 'To turn on notifications, go to your device settings and allow notifications. Then return to the app and try again.',
          retryButtonText: 'Try again',
        },
        10002: {
          pageTitle: 'Cannot change notifications choice',
          pageHeader: 'Cannot change notifications choice',
          header: 'This may be because notifications are turned off in your device settings.',
          message: 'Go to your device settings and check notifications are turned on, then try again.',
          deviceSettings: 'Go to device settings',
        },
      },
    },
  },
};
