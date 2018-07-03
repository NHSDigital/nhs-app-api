const PATH = '/prescriptions';

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
      redirectUrl: '',
    },
  ],
  409: [
    PATH,
    {
      pageHeader: 'Error retrieving data',
      header: 'Sorry, there\'s been a problem getting your prescription information',
      subheader: 'Please try again later. If the problem persists and you need this information now, please contact your GP surgery directly.',
      message: '',
      hasRetryButton: false,
      retryButtonText: '',
      redirectUrl: '',
    },
  ],
  500: [
    PATH,
    {
      pageHeader: 'Error retrieving data',
      header: 'Sorry, there\'s been a problem getting your prescription information',
      subheader: 'Please try again later. If the problem persists and you need this information now, please contact your GP surgery directly.',
      message: '',
      hasRetryButton: false,
      retryButtonText: '',
      redirectUrl: '',
    },
  ],
  502: [
    PATH,
    {
      pageHeader: 'Error retrieving data',
      header: 'Sorry, there\'s been a problem getting your prescription information',
      subheader: 'Please try again later. If the problem persists and you need this information now, please contact your GP surgery directly.',
      message: '',
      hasRetryButton: false,
      retryButtonText: '',
      redirectUrl: '',
    },
  ],
  504: [
    PATH,
    {
      pageHeader: 'Error retrieving data',
      header: 'Sorry, there\'s been a problem getting your prescription information',
      subheader: 'Please try again',
      message: 'If the problem persists and you need this information now, please contact your GP surgery directly.',
      hasRetryButton: true,
      retryButtonText: 'Try again',
      redirectUrl: '',
    },
  ],
};
