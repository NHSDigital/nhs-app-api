/* eslint-disable import/extensions */
import Routes from '@/routes';

const PATH = '/appointments/confirmation';

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
      showError: false,
    },
  ],
  460: [
    PATH,
    {
      pageHeader: 'Appointment limit reached',
      header: 'You can\'t book anymore appointments right now',
      subheader: 'Contact your GP surgery if you still need to book one.',
      message: 'You can go back to see what you\'ve already booked and cancel any appointments that you may no longer need.\r\nIf it\'s urgent and you don\'t know what to do, call 111 to get help near you.',
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
