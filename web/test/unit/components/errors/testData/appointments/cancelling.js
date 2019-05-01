import { APPOINTMENTS, APPOINTMENT_CANCELLING } from '@/lib/routes';

const PATH = APPOINTMENT_CANCELLING.path;

export default {
  400: [
    PATH,
    {
      pageTitle: 'Appointment request error',
      pageHeader: 'Error sending request',
      header: 'There\'s been a problem sending your request',
      subheader: '',
      message: 'Go back and try again. If the problem continues and you need to book or cancel an appointment now, contact your GP surgery directly. For urgent medical advice, call 111.',
      messageLabel: 'Go back and try again. If the problem continues and you need to book or cancel an appointment now, contact your GP surgery directly. For urgent medical advice, call one one one.',
      hasRetryButton: true,
      retryButtonText: 'Back to my appointments',
      redirectUrl: APPOINTMENTS.path,
    },
  ],
  403: [
    PATH,
    {
      pageTitle: 'Appointment request error',
      pageHeader: 'Cancel appointment',
      header: 'Contact your GP surgery to cancel',
      subheader: '',
      message: 'You can\'t cancel appointments online right now. Call your GP surgery as soon as possible to let them know you need to cancel.',
      hasRetryButton: true,
      retryButtonText: 'Back to my appointments',
      redirectUrl: APPOINTMENTS.path,
    },
  ],
  409: [
    PATH,
    {
      pageTitle: 'Appointment request error',
      pageHeader: 'Error sending request',
      header: 'There\'s been a problem sending your request',
      subheader: '',
      message: 'Go back and try again. If the problem continues and you need to book or cancel an appointment now, contact your GP surgery directly. For urgent medical advice, call 111.',
      messageLabel: 'Go back and try again. If the problem continues and you need to book or cancel an appointment now, contact your GP surgery directly. For urgent medical advice, call one one one.',
      hasRetryButton: true,
      retryButtonText: 'Back to my appointments',
      redirectUrl: APPOINTMENTS.path,
    },
  ],
  461: [
    PATH,
    {
      pageTitle: 'Appointment request error',
      pageHeader: 'Cancel appointment',
      header: 'Contact your GP surgery to cancel',
      subheader: '',
      message: 'It\'s too late to cancel this appointment online. Call your GP surgery as soon as possible to let them know you need to cancel.',
      hasRetryButton: true,
      retryButtonText: 'Back to my appointments',
      redirectUrl: APPOINTMENTS.path,
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
      messageLabel: 'Go back and try again. If the problem continues and you need to book or cancel an appointment now, contact your GP surgery directly. For urgent medical advice, call one one one.',
      hasRetryButton: true,
      retryButtonText: 'Back to my appointments',
      redirectUrl: APPOINTMENTS.path,
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
      messageLabel: 'Go back and try again. If the problem continues and you need to book or cancel an appointment now, contact your GP surgery directly. For urgent medical advice, call one one one.',
      hasRetryButton: true,
      retryButtonText: 'Back to my appointments',
      redirectUrl: APPOINTMENTS.path,
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
      messageLabel: 'Go back and try again. If the problem continues and you need to book or cancel an appointment now, contact your GP surgery directly. For urgent medical advice, call one one one.',
      hasRetryButton: true,
      retryButtonText: 'Back to my appointments',
      redirectUrl: APPOINTMENTS.path,
    },
  ],
};
