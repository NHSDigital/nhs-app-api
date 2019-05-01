import { ORGAN_DONATION } from '@/lib/routes';

const PATH = ORGAN_DONATION.path;

export default {
  500: [
    PATH,
    {
      pageTitle: 'Something went wrong',
      pageHeader: 'Something went wrong',
      header: 'Something went wrong',
      subheader: '',
      message: 'You can contact NHS Blood and Transplant to get help with this.',
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
      message: 'You can contact NHS Blood and Transplant to get help with this.',
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
      redirectUrl: '/organ-donation',
      additionalInfoComponent: 'ContactOrganDonation',
    },
  ],
};
