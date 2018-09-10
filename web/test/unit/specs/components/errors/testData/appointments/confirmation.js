import Routes from '@/routes';

const PATH = Routes.APPOINTMENT_CONFIRMATIONS.path;

export default {
  400: [
    PATH,
    {
      pageTitle: 'Appointment request error',
      pageHeader: 'Error sending request',
      header: 'There\'s been a problem sending your request',
      subheader: '',
      message: 'Go back and try again. If the problem continues and you need to book or cancel an appointment now, contact your GP surgery directly. For urgent medical advice, call 111.',
      hasRetryButton: true,
      retryButtonText: 'Back to my appointments',
      redirectUrl: Routes.APPOINTMENTS.path,
    },
  ],
  403: [
    PATH,
    {
      pageTitle: 'Appointment request error',
      pageHeader: 'Error sending request',
      header: 'There\'s been a problem sending your request',
      subheader: '',
      message: 'Go back and try again. If the problem continues and you need to book or cancel an appointment now, contact your GP surgery directly. For urgent medical advice, call 111.',
      hasRetryButton: true,
      retryButtonText: 'Back to my appointments',
      redirectUrl: Routes.APPOINTMENTS.path,
    },
  ],
  409: [
    PATH,
    {
      pageTitle: 'Appointment request error',
      pageHeader: 'Confirm appointment',
      header: 'This slot is no longer available',
      subheader: '',
      message: 'Please select a different time.',
      hasRetryButton: true,
      retryButtonText: 'Back',
      redirectUrl: Routes.APPOINTMENT_BOOKING.path,
    },
  ],
  460: [
    PATH,
    {
      pageTitle: 'Appointment request error',
      pageHeader: 'Appointment limit reached',
      header: 'You can\'t book any more appointments right now',
      subheader: 'Contact your GP surgery if you still need to book one.',
      message: 'You can go back to see what you\'ve already booked and cancel any appointments that you may no longer need.',
      additionalInfo: 'If it\'s urgent and you don\'t know what to do, call 111 to get help near you.',
      hasRetryButton: true,
      retryButtonText: 'Back to my appointments',
      redirectUrl: Routes.APPOINTMENTS.path,
    },
  ],
  500: [
    PATH,
    {
      pageTitle: 'Appointment request error',
      pageHeader: 'Error sending request',
      header: 'There\'s been a problem sending your request',
      subheader: '',
      message: 'Go back and try again. If the problem continues and you need to book or cancel an appointment now, contact your GP surgery directly. For urgent medical advice, call 111.',
      hasRetryButton: true,
      retryButtonText: 'Back to my appointments',
      redirectUrl: Routes.APPOINTMENTS.path,
    },
  ],
  502: [
    PATH,
    {
      pageTitle: 'Appointment request error',
      pageHeader: 'Error sending request',
      header: 'There\'s been a problem sending your request',
      subheader: '',
      message: 'Go back and try again. If the problem continues and you need to book or cancel an appointment now, contact your GP surgery directly. For urgent medical advice, call 111.',
      hasRetryButton: true,
      retryButtonText: 'Back to my appointments',
      redirectUrl: Routes.APPOINTMENTS.path,
    },
  ],
  504: [
    PATH,
    {
      pageTitle: 'Appointment request error',
      pageHeader: 'Error sending request',
      header: 'There\'s been a problem sending your request',
      subheader: '',
      message: 'Go back and try again. If the problem continues and you need to book or cancel an appointment now, contact your GP surgery directly. For urgent medical advice, call 111.',
      hasRetryButton: true,
      retryButtonText: 'Back to my appointments',
      redirectUrl: Routes.APPOINTMENTS.path,
    },
  ],
};
