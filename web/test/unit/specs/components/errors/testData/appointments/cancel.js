/* eslint-disable import/extensions */
import Routes from '@/routes';

const PATH = '/appointments/cancel';

export default {
  403: [
    PATH,
    {
      pageHeader: 'Error sending request',
      header: 'Sorry, there\'s been a problem sending your request',
      subheader: 'Please go back and try again.',
      message: 'If the problem persists and you need to book or cancel an appointment now, contact your GP surgery directly.',
      hasRetryButton: true,
      retryButtonText: 'Back to my appointments',
      redirectUrl: Routes.APPOINTMENTS.path,
    },
  ],
  409: [
    PATH,
    {
      pageHeader: 'Error sending request',
      header: 'Sorry, there\'s been a problem sending your request',
      subheader: 'Please go back and try again.',
      message: 'If the problem persists and you need to book or cancel an appointment now, contact your GP surgery directly.',
      hasRetryButton: true,
      retryButtonText: 'Back to my appointments',
      redirectUrl: Routes.APPOINTMENTS.path,
    },
  ],
  500: [
    PATH,
    {
      pageHeader: 'Error sending request',
      header: 'Sorry, there\'s been a problem sending your request',
      subheader: 'Please go back and try again.',
      message: 'If the problem persists and you need to book or cancel an appointment now, contact your GP surgery directly.',
      hasRetryButton: true,
      retryButtonText: 'Back to my appointments',
      redirectUrl: Routes.APPOINTMENTS.path,
    },
  ],
  502: [
    PATH,
    {
      pageHeader: 'Error sending request',
      header: 'Sorry, there\'s been a problem sending your request',
      subheader: 'Please go back and try again.',
      message: 'If the problem persists and you need to book or cancel an appointment now, contact your GP surgery directly.',
      hasRetryButton: true,
      retryButtonText: 'Back to my appointments',
      redirectUrl: Routes.APPOINTMENTS.path,
    },
  ],
  504: [
    PATH,
    {
      pageHeader: 'Error sending request',
      header: 'Sorry, there\'s been a problem sending your request',
      subheader: 'Please go back and try again.',
      message: 'If the problem persists and you need to book or cancel an appointment now, contact your GP surgery directly.',
      hasRetryButton: true,
      retryButtonText: 'Back to my appointments',
      redirectUrl: Routes.APPOINTMENTS.path,
    },
  ],
};
