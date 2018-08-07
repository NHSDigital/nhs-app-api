import Routes from '@/routes';

const PATH = '/prescriptions/confirm-prescription-details';

export default {
  400: [
    PATH,
    {
      pageHeader: 'Error sending request',
      header: 'Sorry, there\'s been a problem sending your request',
      subheader: 'Please go back and try again.',
      message: 'If the problem persists and you need to order a repeat prescription now, please contact your GP surgery directly.',
      hasRetryButton: true,
      retryButtonText: 'Back to my repeat prescriptions',
      redirectUrl: Routes.PRESCRIPTIONS.path,
    },
  ],
  403: [
    PATH,
    {
      pageHeader: 'Error sending request',
      header: 'Sorry, there\'s been a problem sending your request',
      subheader: 'Please go back and try again.',
      message: 'If the problem persists and you need to order a repeat prescription now, please contact your GP surgery directly.',
      hasRetryButton: true,
      retryButtonText: 'Back to my repeat prescriptions',
      redirectUrl: Routes.PRESCRIPTIONS.path,
    },
  ],
  409: [
    PATH,
    {
      pageHeader: 'Error sending request',
      header: 'Sorry, there\'s been a problem sending your request',
      subheader: 'Please go back and try again.',
      message: 'If the problem persists and you need to order a repeat prescription now, please contact your GP surgery directly.',
      hasRetryButton: true,
      retryButtonText: 'Back to my repeat prescriptions',
      redirectUrl: Routes.PRESCRIPTIONS.path,
    },
  ],
  500: [
    PATH,
    {
      pageHeader: 'Error sending request',
      header: 'Sorry, there\'s been a problem sending your request',
      subheader: 'Please go back and try again.',
      message: 'If the problem persists and you need to order a repeat prescription now, please contact your GP surgery directly.',
      hasRetryButton: true,
      retryButtonText: 'Back to my repeat prescriptions',
      redirectUrl: Routes.PRESCRIPTIONS.path,
    },
  ],
  502: [
    PATH,
    {
      pageHeader: 'Error sending request',
      header: 'Sorry, there\'s been a problem sending your request',
      subheader: 'Please go back and try again.',
      message: 'If the problem persists and you need to order a repeat prescription now, please contact your GP surgery directly.',
      hasRetryButton: true,
      retryButtonText: 'Back to my repeat prescriptions',
      redirectUrl: Routes.PRESCRIPTIONS.path,
    },
  ],
  504: [
    PATH,
    {
      pageHeader: 'Error sending request',
      header: 'Sorry, there\'s been a problem sending your request',
      subheader: 'Please go back and try again.',
      message: 'If the problem persists and you need to order a repeat prescription now, please contact your GP surgery directly.',
      hasRetryButton: true,
      retryButtonText: 'Back to my repeat prescriptions',
      redirectUrl: Routes.PRESCRIPTIONS.path,
    },
  ],
};
