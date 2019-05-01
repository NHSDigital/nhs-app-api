import { PRESCRIPTIONS } from '@/lib/routes';

const PATH = PRESCRIPTIONS.path;

export default {
  400: [
    PATH,
    {
      pageTitle: 'Prescription data error',
      pageHeader: 'Prescription data error',
      header: 'There\'s been a problem getting your prescription information',
      subheader: '',
      message: 'Try again later. If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call 111.',
      retryButtonText: '',
      hasRetryButton: false,
      redirectUrl: '',
    },
  ],
  403: [
    PATH,
    {
      pageTitle: 'Repeat prescriptions unavailable',
      pageHeader: 'Repeat prescriptions unavailable',
      header: 'You are not currently able to order repeat prescriptions online',
      subheader: '',
      message: 'Contact your GP surgery for more information. For urgent medical help, call 111.',
      hasRetryButton: false,
      retryButtonText: '',
      redirectUrl: '',
    },
  ],
  409: [
    PATH,
    {
      pageTitle: 'Prescription data error',
      pageHeader: 'Prescription data error',
      header: 'There\'s been a problem getting your prescription information',
      subheader: '',
      message: 'Try again later. If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call 111.',
      hasRetryButton: false,
      retryButtonText: '',
      redirectUrl: '',
    },
  ],
  500: [
    PATH,
    {
      pageTitle: 'Prescription data error',
      pageHeader: 'Prescription data error',
      header: 'There\'s been a problem getting your prescription information',
      subheader: '',
      message: 'Try again later. If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call 111.',
      hasRetryButton: false,
      retryButtonText: '',
      redirectUrl: '',
    },
  ],
  502: [
    PATH,
    {
      pageTitle: 'Prescription data error',
      pageHeader: 'Prescription data error',
      header: 'There\'s been a problem getting your prescription information',
      subheader: '',
      message: 'Try again later. If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call 111.',
      hasRetryButton: false,
      retryButtonText: '',
      redirectUrl: '',
    },
  ],
  504: [
    PATH,
    {
      pageTitle: 'Prescription data error',
      pageHeader: 'Prescription data error',
      header: 'There\'s been a problem getting your prescription information',
      subheader: '',
      message: 'Try again now. If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call 111.',
      hasRetryButton: true,
      retryButtonText: 'Try again',
      redirectUrl: '',
    },
  ],
};
