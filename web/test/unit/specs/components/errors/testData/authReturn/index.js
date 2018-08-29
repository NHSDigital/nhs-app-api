const PATH = '/auth-return';

export default {
  464: [
    PATH,
    {
      pageHeader: 'Service unavailable - NHS App',
      header: 'Service unavailable',
      subheader: 'You cannot currently use this service',
      message: 'You can still call or visit your GP surgery to access your NHS services. For urgent medical advice, call 111.',
      hasRetryButton: false,
      retryButtonText: '',
      redirectUrl: '',
      isInformationError: true,
    },
  ],
  465: [
    PATH,
    {
      pageHeader: 'Service unavailable - NHS App',
      header: 'Service unavailable',
      subheader: 'You cannot currently use this service',
      message: 'As you’re under 16, you cannot currently access the NHS App. You can still call or visit your GP surgery to access your NHS services. For urgent medical advice, call 111.',
      hasRetryButton: false,
      retryButtonText: '',
      redirectUrl: '',
      isInformationError: true,
    },
  ],
};
