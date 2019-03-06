import { ORGAN_DONATION_REVIEW_YOUR_DECISION } from '@/lib/routes';

const PATH = ORGAN_DONATION_REVIEW_YOUR_DECISION.path;

export default {
  500: [
    PATH,
    {
      pageTitle: 'Something went wrong',
      pageHeader: 'Something went wrong',
      header: 'Something went wrong',
      subheader: '',
      message: 'You need to contact NHS Blood and Transplant to get help with this.',
      retryButtonText: '',
      redirectUrl: '',
      additionalInfoComponent: 'ContactOrganDonation',
    },
  ],
  502: [
    PATH,
    {
      pageTitle: 'Something went wrong',
      pageHeader: 'Something went wrong',
      header: 'Something went wrong',
      subheader: '',
      message: 'You need to contact NHS Blood and Transplant to get help with this.',
      retryButtonText: '',
      redirectUrl: '',
      additionalInfoComponent: 'ContactOrganDonation',
    },
  ],
  5021: [
    PATH,
    {
      pageTitle: 'Something went wrong',
      pageHeader: 'Something went wrong',
      header: 'Something went wrong',
      subheader: '',
      message: 'If the problem persists you can contact NHS Blood and Transplant to get help with this.',
      hasRetryButton: true,
      retryButtonText: 'Try again',
      redirectUrl: '',
      additionalInfoComponent: 'ContactOrganDonation',
    },
  ],
};
