import { NOMINATED_PHARMACY_CONFIRM } from '@/lib/routes';

const PATH = NOMINATED_PHARMACY_CONFIRM.path;

export default {
  400: [
    PATH,
    {
      pageTitle: 'Error updating nomination',
      pageHeader: 'Error updating nomination',
      header: 'There\'s been a problem updating your pharmacy nomination',
      subheader: '',
      message: 'Go back and try again. If the problem continues and you need to nominate a pharmacy now, contact your GP surgery directly.',
      retryButtonText: 'Back to prescriptions',
      hasRetryButton: true,
      redirectUrl: '/prescriptions',
    },
  ],
  403: [
    PATH,
    {
      pageTitle: 'Error updating nomination',
      pageHeader: 'Error updating nomination',
      header: 'There\'s been a problem updating your pharmacy nomination',
      subheader: '',
      message: 'Go back and try again. If the problem continues and you need to nominate a pharmacy now, contact your GP surgery directly.',
      retryButtonText: 'Back to prescriptions',
      hasRetryButton: true,
      redirectUrl: '/prescriptions',
    },
  ],
  409: [
    PATH,
    {
      pageTitle: 'Error updating nomination',
      pageHeader: 'Error updating nomination',
      header: 'There\'s been a problem updating your pharmacy nomination',
      subheader: '',
      message: 'Go back and try again. If the problem continues and you need to nominate a pharmacy now, contact your GP surgery directly.',
      retryButtonText: 'Back to prescriptions',
      hasRetryButton: true,
      redirectUrl: '/prescriptions',
    },
  ],
  500: [
    PATH,
    {
      pageTitle: 'Error updating nomination',
      pageHeader: 'Error updating nomination',
      header: 'There\'s been a problem updating your pharmacy nomination',
      subheader: '',
      message: 'Go back and try again. If the problem continues and you need to nominate a pharmacy now, contact your GP surgery directly.',
      retryButtonText: 'Back to prescriptions',
      hasRetryButton: true,
      redirectUrl: '/prescriptions',
    },
  ],
  502: [
    PATH,
    {
      pageTitle: 'Error updating nomination',
      pageHeader: 'Error updating nomination',
      header: 'There\'s been a problem updating your pharmacy nomination',
      subheader: '',
      message: 'Go back and try again. If the problem continues and you need to nominate a pharmacy now, contact your GP surgery directly.',
      retryButtonText: 'Back to prescriptions',
      hasRetryButton: true,
      redirectUrl: '/prescriptions',
    },
  ],
  504: [
    PATH,
    {
      pageTitle: 'Error updating nomination',
      pageHeader: 'Error updating nomination',
      header: 'There\'s been a problem updating your pharmacy nomination',
      subheader: '',
      message: 'Go back and try again. If the problem continues and you need to nominate a pharmacy now, contact your GP surgery directly.',
      retryButtonText: 'Back to prescriptions',
      hasRetryButton: true,
      redirectUrl: '/prescriptions',
    },
  ],
};
