import healthRecords from './apiErrorsHealthRecords';
import messages from './apiErrorsMessages';
import organDonation from './apiErrorsOrganDonation';
import prescriptions from './apiErrorsPrescriptions';

export default {
  pageHeader: 'Server error',
  header: 'We\'re experiencing technical difficulties',
  reportAProblemLink: 'Report a problem',
  referenceCode: 'Reference: {reference}',
  tryAgainNow: 'Try again now.',
  subheader: '',
  message: {
    text: 'Try again later. If the problem continues and you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, call 111.',
    label: 'Try again later. If the problem continues and you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, call one one one.',
  },
  404: {
    pageTitle: 'Page not found',
    header: 'Page not found',
    subheader: 'If you entered a web address, check it was correct.',
    message: {
      text: 'You can go directly to book an appointment or order a repeat prescription, or use the menu buttons to find the service you need. For urgent medical advice, call 111.',
      label: 'You can go directly to book an appointment or order a repeat prescription, or use the menu buttons to find the service you need. For urgent medical advice, call one one one.',
    },
  },
  502: {
    pageTitle: 'Service currently unavailable',
    pageHeader: 'Service currently unavailable',
    header: 'This service is not available right now',
    retryButtonText: 'Try again',
    message: 'Try again in a few moments.',
  },
  components: {
    health_records: healthRecords,
    organ_donation: organDonation,
    messages,
    prescriptions,
  },
};
