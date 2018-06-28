const PATH = '/appointments';

export default {
  403: [
    PATH,
    {
      pageHeader: 'Service unavailable',
      header: 'Sorry, you don\'t currently have access to this service',
      subheader: '',
      message: 'Contact your GP surgery for more information.',
      hasRetryButton: false,
      retryButtonText: '',
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
    },
  ],
};
