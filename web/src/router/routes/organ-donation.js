import OrganDonationPage from '@/pages/organ-donation';
import OrganDonationAdditionalDetailsPage from '@/pages/organ-donation/additional-details';
import OrganDonationAmendPage from '@/pages/organ-donation/amend';
import OrganDonationFaithPage from '@/pages/organ-donation/faith';
import OrganDonationMoreAboutOrgansPage from '@/pages/organ-donation/more-about-organs';
import OrganDonationSomeOrgansPage from '@/pages/organ-donation/some-organs';
import OrganDonationReviewYourDecisionPage from '@/pages/organ-donation/review-your-decision';
import OrganDonationViewDecisionPage from '@/pages/organ-donation/view-decision';
import OrganDonationWithdrawReasonPage from '@/pages/organ-donation/withdraw-reason';
import OrganDonationWithdrawnPage from '@/pages/organ-donation/withdrawn';
import OrganDonationYourChoicePage from '@/pages/organ-donation/your-choice';

import breadcrumbs from '@/breadcrumbs/organ-donation';

import {
  ORGAN_DONATION_PATH,
  ORGAN_DONATION_ADDITIONAL_DETAILS_PATH,
  ORGAN_DONATION_AMEND_PATH,
  ORGAN_DONATION_FAITH_PATH,
  ORGAN_DONATION_MORE_ABOUT_ORGANS_PATH,
  ORGAN_DONATION_SOME_ORGANS_PATH,
  ORGAN_DONATION_REVIEW_YOUR_DECISION_PATH,
  ORGAN_DONATION_VIEW_DECISION_PATH,
  ORGAN_DONATION_WITHDRAW_REASON_PATH,
  ORGAN_DONATION_WITHDRAWN_PATH,
  ORGAN_DONATION_YOUR_CHOICE_PATH,
} from '@/router/paths';
import {
  ORGAN_DONATION_NAME,
  ORGAN_DONATION_ADDITIONAL_DETAILS_NAME,
  ORGAN_DONATION_AMEND_NAME,
  ORGAN_DONATION_FAITH_NAME,
  ORGAN_DONATION_MORE_ABOUT_ORGANS_NAME,
  ORGAN_DONATION_SOME_ORGANS_NAME,
  ORGAN_DONATION_REVIEW_YOUR_DECISION_NAME,
  ORGAN_DONATION_VIEW_DECISION_NAME,
  ORGAN_DONATION_WITHDRAW_REASON_NAME,
  ORGAN_DONATION_WITHDRAWN_NAME,
  ORGAN_DONATION_YOUR_CHOICE_NAME,
} from '@/router/names';
import { UPLIFT_MORE } from '@/router/routes/more';

import { MORE_MENU_ITEM } from '@/middleware/nativeNavigation';

import proofLevel from '@/lib/proofLevel';
import { organDonationHelpUrl } from '@/router/externalLinks';

export const ORGAN_DONATION = {
  path: ORGAN_DONATION_PATH,
  name: ORGAN_DONATION_NAME,
  component: OrganDonationPage,
  meta: {
    headerKey: 'navigation.pages.headers.organDonation',
    titleKey: 'navigation.pages.titles.organDonation',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_MORE,
    crumb: breadcrumbs.ORGAN_DONATION_CRUMB,
    helpUrl: organDonationHelpUrl,
    nativeNavigation: MORE_MENU_ITEM,
  },
};

export const ORGAN_DONATION_ADDITIONAL_DETAILS = {
  path: ORGAN_DONATION_ADDITIONAL_DETAILS_PATH,
  name: ORGAN_DONATION_ADDITIONAL_DETAILS_NAME,
  component: OrganDonationAdditionalDetailsPage,
  meta: {
    headerKey: 'navigation.pages.headers.organDonation',
    titleKey: 'navigation.pages.titles.organDonation',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_MORE,
    crumb: breadcrumbs.ORGAN_DONATION_ADDITIONAL_DETAILS_CRUMB,
    helpUrl: organDonationHelpUrl,
    nativeNavigation: MORE_MENU_ITEM,
  },
};

export const ORGAN_DONATION_AMEND = {
  path: ORGAN_DONATION_AMEND_PATH,
  name: ORGAN_DONATION_AMEND_NAME,
  component: OrganDonationAmendPage,
  meta: {
    headerKey: 'navigation.pages.headers.organDonation',
    titleKey: 'navigation.pages.titles.organDonation',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_MORE,
    crumb: breadcrumbs.ORGAN_DONATION_AMEND_CRUMB,
    helpUrl: organDonationHelpUrl,
    nativeNavigation: MORE_MENU_ITEM,
  },
};

export const ORGAN_DONATION_FAITH = {
  path: ORGAN_DONATION_FAITH_PATH,
  name: ORGAN_DONATION_FAITH_NAME,
  component: OrganDonationFaithPage,
  meta: {
    headerKey: 'navigation.pages.headers.organDonation',
    titleKey: 'navigation.pages.titles.organDonation',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_MORE,
    crumb: breadcrumbs.ORGAN_DONATION_FAITH_CRUMB,
    helpUrl: organDonationHelpUrl,
    nativeNavigation: MORE_MENU_ITEM,
  },
};

export const ORGAN_DONATION_MORE_ABOUT_ORGANS = {
  path: ORGAN_DONATION_MORE_ABOUT_ORGANS_PATH,
  name: ORGAN_DONATION_MORE_ABOUT_ORGANS_NAME,
  component: OrganDonationMoreAboutOrgansPage,
  meta: {
    headerKey: 'navigation.pages.headers.organDonation',
    titleKey: 'navigation.pages.titles.organDonation',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_MORE,
    crumb: breadcrumbs.ORGAN_DONATION_MORE_ABOUT_ORGANS_CRUMB,
    helpUrl: organDonationHelpUrl,
    nativeNavigation: MORE_MENU_ITEM,
  },
};

export const ORGAN_DONATION_SOME_ORGANS = {
  path: ORGAN_DONATION_SOME_ORGANS_PATH,
  name: ORGAN_DONATION_SOME_ORGANS_NAME,
  component: OrganDonationSomeOrgansPage,
  meta: {
    headerKey: 'navigation.pages.headers.organDonation',
    titleKey: 'navigation.pages.titles.organDonation',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_MORE,
    crumb: breadcrumbs.ORGAN_DONATION_SOME_ORGANS_CRUMB,
    helpUrl: organDonationHelpUrl,
    nativeNavigation: MORE_MENU_ITEM,
  },
};

export const ORGAN_DONATION_REVIEW_YOUR_DECISION = {
  path: ORGAN_DONATION_REVIEW_YOUR_DECISION_PATH,
  name: ORGAN_DONATION_REVIEW_YOUR_DECISION_NAME,
  component: OrganDonationReviewYourDecisionPage,
  meta: {
    headerKey: 'navigation.pages.headers.organDonation',
    titleKey: 'navigation.pages.titles.organDonation',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_MORE,
    crumb: breadcrumbs.ORGAN_DONATION_REVIEW_YOUR_DECISION_CRUMB,
    helpUrl: organDonationHelpUrl,
    nativeNavigation: MORE_MENU_ITEM,
  },
};

export const ORGAN_DONATION_VIEW_DECISION = {
  path: ORGAN_DONATION_VIEW_DECISION_PATH,
  name: ORGAN_DONATION_VIEW_DECISION_NAME,
  component: OrganDonationViewDecisionPage,
  meta: {
    headerKey: 'navigation.pages.headers.organDonation',
    titleKey: 'navigation.pages.titles.organDonation',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_MORE,
    crumb: breadcrumbs.ORGAN_DONATION_VIEW_DECISION_CRUMB,
    helpUrl: organDonationHelpUrl,
    nativeNavigation: MORE_MENU_ITEM,
  },
};

export const ORGAN_DONATION_WITHDRAW_REASON = {
  path: ORGAN_DONATION_WITHDRAW_REASON_PATH,
  name: ORGAN_DONATION_WITHDRAW_REASON_NAME,
  component: OrganDonationWithdrawReasonPage,
  meta: {
    headerKey: 'navigation.pages.headers.organDonation',
    titleKey: 'navigation.pages.titles.organDonationWithdraw',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_MORE,
    crumb: breadcrumbs.ORGAN_DONATION_WITHDRAW_REASON_CRUMB,
    helpUrl: organDonationHelpUrl,
    nativeNavigation: MORE_MENU_ITEM,
  },
};

export const ORGAN_DONATION_WITHDRAWN = {
  path: ORGAN_DONATION_WITHDRAWN_PATH,
  name: ORGAN_DONATION_WITHDRAWN_NAME,
  component: OrganDonationWithdrawnPage,
  meta: {
    headerKey: 'navigation.pages.headers.organDonation',
    titleKey: 'navigation.pages.titles.organDonationWithdraw',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_MORE,
    crumb: breadcrumbs.ORGAN_DONATION_WITHDRAWN_CRUMB,
    helpUrl: organDonationHelpUrl,
    nativeNavigation: MORE_MENU_ITEM,
  },
};

export const ORGAN_DONATION_YOUR_CHOICE = {
  path: ORGAN_DONATION_YOUR_CHOICE_PATH,
  name: ORGAN_DONATION_YOUR_CHOICE_NAME,
  component: OrganDonationYourChoicePage,
  meta: {
    headerKey: 'navigation.pages.headers.organDonation',
    titleKey: 'navigation.pages.titles.organDonation',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_MORE,
    crumb: breadcrumbs.ORGAN_DONATION_YOUR_CHOICE_CRUMB,
    helpUrl: organDonationHelpUrl,
    nativeNavigation: MORE_MENU_ITEM,
  },
};

export default [
  ORGAN_DONATION,
  ORGAN_DONATION_ADDITIONAL_DETAILS,
  ORGAN_DONATION_AMEND,
  ORGAN_DONATION_FAITH,
  ORGAN_DONATION_MORE_ABOUT_ORGANS,
  ORGAN_DONATION_SOME_ORGANS,
  ORGAN_DONATION_REVIEW_YOUR_DECISION,
  ORGAN_DONATION_VIEW_DECISION,
  ORGAN_DONATION_WITHDRAW_REASON,
  ORGAN_DONATION_WITHDRAWN,
  ORGAN_DONATION_YOUR_CHOICE,
];
