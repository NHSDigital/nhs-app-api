const PATH = '/appointments/index';

export default {
  400: [
    PATH,
    {
      pageTitle: 'Appointment data error',
      pageHeader: 'Error retrieving data',
      header: 'There\'s been a problem getting your appointment history',
      subheader: '',
      message: 'Please try again later. If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call 111.',
      hasRetryButton: false,
      retryButtonText: '',
      redirectUrl: '',
    },
  ],
  403: [
    PATH,
    {
      pageTitle: 'Appointment booking unavailable',
      pageHeader: 'Appointment booking unavailable',
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
      pageTitle: 'Appointment data error',
      pageHeader: 'Error retrieving data',
      header: 'There\'s been a problem getting your appointment history',
      subheader: '',
      message: 'Please try again later. If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call 111.',
      hasRetryButton: false,
      retryButtonText: '',
      redirectUrl: '',
    },
  ],
  500: [
    PATH,
    {
      pageTitle: 'Appointment data error',
      pageHeader: 'Error retrieving data',
      header: 'There\'s been a problem getting your appointment history',
      subheader: '',
      message: 'Please try again later. If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call 111.',
      hasRetryButton: false,
      retryButtonText: '',
      redirectUrl: '',
    },
  ],
  502: [
    PATH,
    {
      pageTitle: 'Appointment data error',
      pageHeader: 'Error retrieving data',
      header: 'There\'s been a problem getting your appointment history',
      subheader: '',
      message: 'Please try again later. If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call 111.',
      hasRetryButton: false,
      retryButtonText: '',
      redirectUrl: '',
    },
  ],
  504: [
    PATH,
    {
      pageTitle: 'Appointment data error',
      pageHeader: 'Error retrieving data',
      header: 'There\'s been a problem getting your appointment history',
      subheader: 'Please try again',
      message: 'If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call 111.',
      hasRetryButton: true,
      retryButtonText: 'Try again',
      redirectUrl: '',
    },
  ],
};
