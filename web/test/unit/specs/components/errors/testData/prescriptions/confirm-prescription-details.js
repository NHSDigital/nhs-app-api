import { PRESCRIPTIONS, PRESCRIPTION_CONFIRM_COURSES } from '@/lib/routes';

const PATH = PRESCRIPTION_CONFIRM_COURSES.path;

export default {
  400: [
    PATH,
    {
      pageTitle: 'Prescription order error',
      pageHeader: 'Error sending order',
      header: 'There\'s been a problem sending your order',
      subheader: '',
      message: 'Go back and try again. If the problem continues and you need to order a repeat prescription now, contact your GP surgery directly. For urgent medical advice, call 111.',
      retryButtonText: 'Back to my repeat prescriptions',
      hasRetryButton: true,
      redirectUrl: PRESCRIPTIONS.path,
    },
  ],
  403: [
    PATH,
    {
      pageTitle: 'Prescription order error',
      pageHeader: 'Error sending order',
      header: 'There\'s been a problem sending your order',
      subheader: '',
      message: 'Go back and try again. If the problem continues and you need to order a repeat prescription now, contact your GP surgery directly. For urgent medical advice, call 111.',
      hasRetryButton: true,
      retryButtonText: 'Back to my repeat prescriptions',
      redirectUrl: PRESCRIPTIONS.path,
    },
  ],
  409: [
    PATH,
    {
      pageTitle: 'Prescription order error',
      pageHeader: 'Error sending order',
      header: 'There\'s been a problem sending your order',
      subheader: '',
      message: 'Go back and try again. If the problem continues and you need to order a repeat prescription now, contact your GP surgery directly. For urgent medical advice, call 111.',
      hasRetryButton: true,
      retryButtonText: 'Back to my repeat prescriptions',
      redirectUrl: PRESCRIPTIONS.path,
    },
  ],
  466: [
    PATH,
    {
      pageTitle: 'Error submitting request',
      pageHeader: 'Error submitting request',
      header: 'We cannot complete this order',
      subheader: 'You previously ordered at least one of these medications in the last 30 days.',
      message: 'If you need more medication sooner, contact your GP.',
      hasRetryButton: true,
      retryButtonText: 'Back to my repeat prescriptions',
      redirectUrl: PRESCRIPTIONS.path,
    },
  ],
  500: [
    PATH,
    {
      pageTitle: 'Prescription order error',
      pageHeader: 'Error sending order',
      header: 'There\'s been a problem sending your order',
      subheader: '',
      message: 'Go back and try again. If the problem continues and you need to order a repeat prescription now, contact your GP surgery directly. For urgent medical advice, call 111.',
      hasRetryButton: true,
      retryButtonText: 'Back to my repeat prescriptions',
      redirectUrl: PRESCRIPTIONS.path,
    },
  ],
  502: [
    PATH,
    {
      pageTitle: 'Prescription order error',
      pageHeader: 'Error sending order',
      header: 'There\'s been a problem sending your order',
      subheader: '',
      message: 'Go back and try again. If the problem continues and you need to order a repeat prescription now, contact your GP surgery directly. For urgent medical advice, call 111.',
      hasRetryButton: true,
      retryButtonText: 'Back to my repeat prescriptions',
      redirectUrl: PRESCRIPTIONS.path,
    },
  ],
  504: [
    PATH,
    {
      pageTitle: 'Prescription order error',
      pageHeader: 'Error sending order',
      header: 'There\'s been a problem sending your order',
      subheader: '',
      message: 'Go back and try again. If the problem continues and you need to order a repeat prescription now, contact your GP surgery directly. For urgent medical advice, call 111.',
      hasRetryButton: true,
      retryButtonText: 'Back to my repeat prescriptions',
      redirectUrl: PRESCRIPTIONS.path,
    },
  ],
};
