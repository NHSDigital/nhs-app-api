import { NOMINATED_PHARMACY_SEARCH_PATH } from '@/router/paths';

const PATH = NOMINATED_PHARMACY_SEARCH_PATH;

export default {
  400: [
    PATH,
    {
      pageTitle: 'Service currently unavailable',
      pageHeader: 'Service currently unavailable',
      header: 'This service is not available right now',
      subheader: '',
      message: 'Try again in a few moments',
      retryButtonText: 'Back to prescriptions',
      hasRetryButton: true,
      redirectUrl: '/prescriptions',
    },
  ],
  403: [
    PATH,
    {
      pageTitle: 'Service currently unavailable',
      pageHeader: 'Service currently unavailable',
      header: 'This service is not available right now',
      subheader: '',
      message: 'Try again in a few moments',
      retryButtonText: 'Back to prescriptions',
      hasRetryButton: true,
      redirectUrl: '/prescriptions',
    },
  ],
  409: [
    PATH,
    {
      pageTitle: 'Service currently unavailable',
      pageHeader: 'Service currently unavailable',
      header: 'This service is not available right now',
      subheader: '',
      message: 'Try again in a few moments',
      retryButtonText: 'Back to prescriptions',
      hasRetryButton: true,
      redirectUrl: '/prescriptions',
    },
  ],
  500: [
    PATH,
    {
      pageTitle: 'Service currently unavailable',
      pageHeader: 'Service currently unavailable',
      header: 'This service is not available right now',
      subheader: '',
      message: 'Try again in a few moments',
      retryButtonText: 'Back to prescriptions',
      hasRetryButton: true,
      redirectUrl: '/prescriptions',
    },
  ],
  502: [
    PATH,
    {
      pageTitle: 'Service currently unavailable',
      pageHeader: 'Service currently unavailable',
      header: 'This service is not available right now',
      subheader: '',
      message: 'Try again in a few moments',
      retryButtonText: 'Back to prescriptions',
      hasRetryButton: true,
      redirectUrl: '/prescriptions',
    },
  ],
  504: [
    PATH,
    {
      pageTitle: 'Service currently unavailable',
      pageHeader: 'Service currently unavailable',
      header: 'This service is not available right now',
      subheader: '',
      message: 'Try again in a few moments',
      retryButtonText: 'Back to prescriptions',
      hasRetryButton: true,
      redirectUrl: '/prescriptions',
    },
  ],
};
