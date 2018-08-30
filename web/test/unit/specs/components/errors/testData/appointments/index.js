const PATH = '/appointments';

export default {
  400: [
    PATH,
    {
      pageHeader: 'Error retrieving data',
      header: 'Sorry, there\'s been a problem getting your appointment history',
      subheader: '',
      message: 'Please try again later. If the problem persists and you need this information now, please contact your GP surgery directly.',
      hasRetryButton: false,
      retryButtonText: '',
      redirectUrl: '',
    },
  ],
  403: [
    PATH,
    {
      pageHeader: 'Service unavailable',
      header: 'You are not currently able to book appointments online.',
      subheader: 'Contact your GP surgery for more information.',
      message: 'For urgent medical help, call 111.',
      hasRetryButton: false,
      retryButtonText: '',
      redirectUrl: '',
    },
  ],
  409: [
    PATH,
    {
      pageHeader: 'Error retrieving data',
      header: 'Sorry, there\'s been a problem getting your appointment history',
      subheader: '',
      message: 'Please try again later. If the problem persists and you need this information now, please contact your GP surgery directly.',
      hasRetryButton: false,
      retryButtonText: '',
      redirectUrl: '',
    },
  ],
  500: [
    PATH,
    {
      pageHeader: 'Error retrieving data',
      header: 'Sorry, there\'s been a problem getting your appointment history',
      subheader: '',
      message: 'Please try again later. If the problem persists and you need this information now, please contact your GP surgery directly.',
      hasRetryButton: false,
      retryButtonText: '',
      redirectUrl: '',
    },
  ],
  502: [
    PATH,
    {
      pageHeader: 'Error retrieving data',
      header: 'Sorry, there\'s been a problem getting your appointment history',
      subheader: '',
      message: 'Please try again later. If the problem persists and you need this information now, please contact your GP surgery directly.',
      hasRetryButton: false,
      retryButtonText: '',
      redirectUrl: '',
    },
  ],
  504: [
    PATH,
    {
      pageHeader: 'Error retrieving data',
      header: 'Sorry, there\'s been a problem getting your appointment history',
      subheader: 'Please try again',
      message: 'If the problem persists and you need this information now, please contact your GP surgery directly.',
      hasRetryButton: true,
      retryButtonText: 'Try again',
      redirectUrl: '',
    },
  ],
};
