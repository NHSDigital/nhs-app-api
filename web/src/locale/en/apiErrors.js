import account from './apiErrorsAccount';
import more from './apiErrorsMore';
import healthRecords from './apiErrorsHealthRecords';
import messages from './apiErrorsMessages';
import organDonation from './apiErrorsOrganDonation';
import prescriptions from './apiErrorsPrescriptions';

export default {
  pageHeader: 'The service is unavailable',
  header: 'This might be a temporary problem.',
  reportAProblem: 'Report a problem',
  referenceReference: 'Reference: {reference}',
  tryAgainNow: 'Try again now.',
  goBackAndTryAgain: 'Go back and try again',
  ifYouNeedToBookAnAppointment: 'If you need to book an appointment or get a prescription now, use your GP surgery\'s website or call the surgery directly.',
  subheader: '',
  message: {
    text: 'For urgent medical advice, go to {111link} or call 111.',
    label: 'For urgent medical advice, go to 111.nhs.uk or call one one one.',
  },
  404: {
    pageTitle: 'Page not found',
    header: 'Page not found',
    subheader: 'If you entered a web address, check it was correct.',
    message: {
      text: 'You can go directly to book an appointment or order a repeat prescription, or use the menu buttons to find the service you need.  For urgent medical advice, go to {111link} or call 111.',
      label: 'You can go directly to book an appointment or order a repeat prescription, or use the menu buttons to find the service you need. For urgent medical advice, go to 111.nhs.uk or call one one one.',
    },
  },
  502: {
    pageTitle: 'The service is unavailable',
    pageHeader: 'The service is unavailable',
    header: 'This might be a temporary problem',
    retryButtonText: 'Try again',
    message: {
      text: 'For urgent medical advice, go to {111link} or call 111.',
      label: 'For urgent medical advice, go to 111.nhs.uk or call one one one.',
    },
  },
  components: {
    account,
    more,
    health_records: healthRecords,
    organ_donation: organDonation,
    messages,
    prescriptions,
  },
};
