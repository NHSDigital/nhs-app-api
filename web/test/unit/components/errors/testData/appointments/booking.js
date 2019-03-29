import { APPOINTMENT_BOOKING } from '@/lib/routes';

const PATH = APPOINTMENT_BOOKING.path;

export default {
  400: [
    PATH,
    {
      pageTitle: 'Appointment data error',
      pageHeader: 'Appointment data error',
      header: 'There\'s been a problem loading this page',
      subheader: '',
      message: 'Try again later. If the problem continues and you need to book an appointment now, contact your GP surgery directly. For urgent medical advice, call 111.',
      messageLabel: 'Try again later. If the problem continues and you need to book an appointment now, contact your GP surgery directly. For urgent medical advice, call one one one.',
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
      header: 'You are not currently able to book appointments online',
      subheader: '',
      message: 'Contact your GP surgery for more information. For urgent medical help, call 111.',
      messageLabel: 'Contact your GP surgery for more information. For urgent medical help, call one one one.',
      hasRetryButton: false,
      retryButtonText: '',
      redirectUrl: '',
    },
  ],
  409: [
    PATH,
    {
      pageTitle: 'Appointment data error',
      pageHeader: 'Appointment data error',
      header: 'There\'s been a problem loading this page',
      subheader: '',
      message: 'Try again later. If the problem continues and you need to book an appointment now, contact your GP surgery directly. For urgent medical advice, call 111.',
      messageLabel: 'Try again later. If the problem continues and you need to book an appointment now, contact your GP surgery directly. For urgent medical advice, call one one one.',
      hasRetryButton: false,
      retryButtonText: '',
      redirectUrl: '',
    },
  ],
  500: [
    PATH,
    {
      pageTitle: 'Appointment data error',
      pageHeader: 'Appointment data error',
      header: 'There\'s been a problem loading this page',
      subheader: '',
      message: 'Try again later. If the problem continues and you need to book an appointment now, contact your GP surgery directly. For urgent medical advice, call 111.',
      messageLabel: 'Try again later. If the problem continues and you need to book an appointment now, contact your GP surgery directly. For urgent medical advice, call one one one.',
      hasRetryButton: false,
      retryButtonText: '',
      redirectUrl: '',
    },
  ],
  502: [
    PATH,
    {
      pageTitle: 'Appointment data error',
      pageHeader: 'Appointment data error',
      header: 'There\'s been a problem loading this page',
      subheader: '',
      message: 'Try again later. If the problem continues and you need to book an appointment now, contact your GP surgery directly. For urgent medical advice, call 111.',
      messageLabel: 'Try again later. If the problem continues and you need to book an appointment now, contact your GP surgery directly. For urgent medical advice, call one one one.',
      hasRetryButton: false,
      retryButtonText: '',
      redirectUrl: '',
    },
  ],
  504: [
    PATH,
    {
      pageTitle: 'Appointment data error',
      pageHeader: 'Appointment data error',
      header: 'There\'s been a problem loading this page',
      subheader: '',
      message: 'Try again now. If the problem continues and you need to book an appointment now, contact your GP surgery directly. For urgent medical advice, call 111.',
      messageLabel: 'Try again now. If the problem continues and you need to book an appointment now, contact your GP surgery directly. For urgent medical advice, call one one one.',
      hasRetryButton: true,
      retryButtonText: 'Try again',
      redirectUrl: '/appointments/booking',
    },
  ],
};
