import { AUTH_RETURN } from '@/lib/routes';

const PATH = AUTH_RETURN.path;

export default {
  464: [
    PATH,
    {
      pageTitle: 'Session error',
      pageHeader: 'Service unavailable',
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
      pageTitle: 'Session error',
      pageHeader: 'Service unavailable',
      header: 'Service unavailable',
      subheader: 'You cannot currently use this service',
      message: 'As you’re under 16, you cannot currently access the NHS App. You can still call or visit your GP surgery to access your NHS services. For urgent medical advice, call 111.',
      hasRetryButton: false,
      retryButtonText: '',
      redirectUrl: '',
      isInformationError: true,
    },
  ],
  // default error....
  500: [
    PATH,
    {
      pageTitle: 'Session error',
      pageHeader: 'Session error',
      header: 'Session error',
      subheader: 'There\'s been a problem loading this page.',
      message: 'Go back to the home screen and log in again.',
      additionalInfo: 'If the problem continues and you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, call 111.',
      hasRetryButton: true,
      retryButtonText: 'Back to home',
      redirectUrl: '/login',
    },
  ],
};
