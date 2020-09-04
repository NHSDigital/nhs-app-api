import NominatedPharmacyChangeSuccessPage from '@/pages/nominated-pharmacy/change-success';
import NominatedPharmacyChooseTypePage from '@/pages/nominated-pharmacy/choose-type';
import NominatedPharmacyCheckPage from '@/pages/nominated-pharmacy/check';
import NominatedPharmacyConfirmPage from '@/pages/nominated-pharmacy/confirm';
import NominatedPharmacyDSPInterruptPage from '@/pages/nominated-pharmacy/dsp-interrupt';
import NominatedPharmacyIndexPage from '@/pages/nominated-pharmacy/index';
import NominatedPharmacyInterruptPage from '@/pages/nominated-pharmacy/interrupt';
import NominatedPharmacyOnlineOnlyChoicesPage from '@/pages/nominated-pharmacy/online-only-choices';
import NominatedPharmacyOnlyOnlySearchPage from '@/pages/nominated-pharmacy/online-only-search';
import NominatedPharmacySearchResultsPage from '@/pages/nominated-pharmacy/results';
import NominatedPharmacySearchPage from '@/pages/nominated-pharmacy/search';
import { nominatedPharmacyHelpUrl } from '@/router/externalLinks';
import { PRESCRIPTIONS_MENU_ITEM } from '@/middleware/nativeNavigation';
import breadcrumbs from '@/breadcrumbs/nominatedPharmacy';
import proofLevel from '@/lib/proofLevel';
import PharmacyTypeChoice from '@/lib/pharmacy-detail/pharmacy-type-choice';
import {
  NOMINATED_PHARMACY_NAME,
  NOMINATED_PHARMACY_SEARCH_NAME,
  NOMINATED_PHARMACY_CONFIRM_NAME,
  NOMINATED_PHARMACY_CHANGE_SUCCESS_NAME,
  NOMINATED_PHARMACY_INTERRUPT_NAME,
  NOMINATED_PHARMACY_DSP_INTERRUPT_NAME,
  NOMINATED_PHARMACY_SEARCH_RESULTS_NAME,
  NOMINATED_PHARMACY_CHECK_NAME,
  NOMINATED_PHARMACY_CHOOSE_TYPE_NAME,
  NOMINATED_PHARMACY_ONLINE_ONLY_CHOICES_NAME,
  NOMINATED_PHARMACY_ONLINE_ONLY_SEARCH_NAME,
} from '@/router/names';
import {
  NOMINATED_PHARMACY_PATH,
  NOMINATED_PHARMACY_SEARCH_PATH,
  NOMINATED_PHARMACY_CONFIRM_PATH,
  NOMINATED_PHARMACY_CHANGE_SUCCESS_PATH,
  NOMINATED_PHARMACY_INTERRUPT_PATH,
  NOMINATED_PHARMACY_DSP_INTERRUPT_PATH,
  NOMINATED_PHARMACY_SEARCH_RESULTS_PATH,
  NOMINATED_PHARMACY_CHECK_PATH,
  NOMINATED_PHARMACY_CHOOSE_TYPE_PATH,
  NOMINATED_PHARMACY_ONLINE_ONLY_CHOICES_PATH,
  NOMINATED_PHARMACY_ONLINE_ONLY_SEARCH_PATH,
} from '@/router/paths';
import { UPLIFT_PRESCRIPTIONS } from '@/router/routes/prescriptions';

export const NOMINATED_PHARMACY_CHANGE_SUCCESS = {
  path: NOMINATED_PHARMACY_CHANGE_SUCCESS_PATH,
  name: NOMINATED_PHARMACY_CHANGE_SUCCESS_NAME,
  component: NominatedPharmacyChangeSuccessPage,
  meta: {
    headerKey: 'navigation.pages.headers.nominatedPharmacyChangeSuccess',
    titleKey: 'navigation.pages.titles.nominatedPharmacyChangeSuccess',
    crumb: breadcrumbs.NOMINATED_PHARMACY_CHANGE_SUCCESS_CRUMB,
    upliftRoute: UPLIFT_PRESCRIPTIONS,
    proofLevel: proofLevel.P9,
    helpUrl: nominatedPharmacyHelpUrl,
    nativeNavigation: PRESCRIPTIONS_MENU_ITEM,
  },
};

export const NOMINATED_PHARMACY_CHOOSE_TYPE = {
  path: NOMINATED_PHARMACY_CHOOSE_TYPE_PATH,
  name: NOMINATED_PHARMACY_CHOOSE_TYPE_NAME,
  component: NominatedPharmacyChooseTypePage,
  meta: {
    headerKey: 'navigation.pages.headers.nominatedPharmacyChooseType',
    titleKey: 'navigation.pages.titles.nominatedPharmacyChooseType',
    crumb: breadcrumbs.NOMINATED_PHARMACY_CHOOSE_TYPE_CRUMB,
    upliftRoute: UPLIFT_PRESCRIPTIONS,
    proofLevel: proofLevel.P9,
    helpUrl: nominatedPharmacyHelpUrl,
    nativeNavigation: PRESCRIPTIONS_MENU_ITEM,
  },
};

export const NOMINATED_PHARMACY_CHECK = {
  path: NOMINATED_PHARMACY_CHECK_PATH,
  name: NOMINATED_PHARMACY_CHECK_NAME,
  component: NominatedPharmacyCheckPage,
  meta: {
    headerKey: 'navigation.pages.headers.prescriptions',
    titleKey: 'navigation.pages.titles.prescriptions',
    crumb: breadcrumbs.NOMINATED_PHARMACY_CHECK_CRUMB,
    upliftRoute: UPLIFT_PRESCRIPTIONS,
    proofLevel: proofLevel.P9,
    helpUrl: nominatedPharmacyHelpUrl,
    nativeNavigation: PRESCRIPTIONS_MENU_ITEM,
  },
};

export const NOMINATED_PHARMACY_CONFIRM = {
  path: NOMINATED_PHARMACY_CONFIRM_PATH,
  name: NOMINATED_PHARMACY_CONFIRM_NAME,
  component: NominatedPharmacyConfirmPage,
  meta: {
    headerKey: 'navigation.pages.headers.confirmNominatedPharmacy',
    titleKey: 'navigation.pages.titles.confirmNominatedPharmacy',
    crumb: breadcrumbs.NOMINATED_PHARMACY_CONFIRM_CRUMB,
    upliftRoute: UPLIFT_PRESCRIPTIONS,
    proofLevel: proofLevel.P9,
    helpUrl: nominatedPharmacyHelpUrl,
    nativeNavigation: PRESCRIPTIONS_MENU_ITEM,
  },
};

export const NOMINATED_PHARMACY_DSP_INTERRUPT = {
  path: NOMINATED_PHARMACY_DSP_INTERRUPT_PATH,
  name: NOMINATED_PHARMACY_DSP_INTERRUPT_NAME,
  component: NominatedPharmacyDSPInterruptPage,
  meta: {
    headerKey: 'navigation.pages.headers.nominatedPharmacyDspInterrupt',
    titleKey: 'navigation.pages.titles.nominatedPharmacyDspInterrupt',
    crumb: breadcrumbs.NOMINATED_PHARMACY_DSP_INTERRUPT_CRUMB,
    upliftRoute: UPLIFT_PRESCRIPTIONS,
    proofLevel: proofLevel.P9,
    helpUrl: nominatedPharmacyHelpUrl,
    nativeNavigation: PRESCRIPTIONS_MENU_ITEM,
  },
};

export const NOMINATED_PHARMACY_INDEX = {
  path: NOMINATED_PHARMACY_PATH,
  name: NOMINATED_PHARMACY_NAME,
  component: NominatedPharmacyIndexPage,
  meta: {
    headerKey: 'navigation.pages.headers.nominatedPharmacy',
    titleKey: 'navigation.pages.titles.nominatedPharmacy',
    crumb: breadcrumbs.NOMINATED_PHARMACY_CRUMB,
    upliftRoute: UPLIFT_PRESCRIPTIONS,
    proofLevel: proofLevel.P9,
    helpUrl: nominatedPharmacyHelpUrl,
    nativeNavigation: PRESCRIPTIONS_MENU_ITEM,
  },
};

export const NOMINATED_PHARMACY_INTERRUPT = {
  path: NOMINATED_PHARMACY_INTERRUPT_PATH,
  name: NOMINATED_PHARMACY_INTERRUPT_NAME,
  component: NominatedPharmacyInterruptPage,
  meta: {
    headerKey: (store, i18n) => (store.state.nominatedPharmacy.pharmacy.pharmacyName === undefined
      ? i18n.t('navigation.pages.headers.nominatedPharmacyNotFoundInterrupt')
      : i18n.t('navigation.pages.headers.nominatedPharmacyFoundInterrupt')),
    titleKey: (store, i18n) => (store.state.nominatedPharmacy.pharmacy.pharmacyName === undefined
      ? i18n.t('navigation.pages.titles.nominatedPharmacyNotFoundInterrupt')
      : i18n.t('navigation.pages.titles.nominatedPharmacyFoundInterrupt')),
    crumb: breadcrumbs.NOMINATED_PHARMACY_INTERRUPT_CRUMB,
    upliftRoute: UPLIFT_PRESCRIPTIONS,
    proofLevel: proofLevel.P9,
    helpUrl: nominatedPharmacyHelpUrl,
    nativeNavigation: PRESCRIPTIONS_MENU_ITEM,
  },
};

export const NOMINATED_PHARMACY_ONLINE_ONLY_CHOICES = {
  path: NOMINATED_PHARMACY_ONLINE_ONLY_CHOICES_PATH,
  name: NOMINATED_PHARMACY_ONLINE_ONLY_CHOICES_NAME,
  component: NominatedPharmacyOnlineOnlyChoicesPage,
  meta: {
    headerKey: 'navigation.pages.headers.nominatedPharmacyOnlineOnlyChoices',
    titleKey: 'navigation.pages.titles.nominatedPharmacyOnlineOnlyChoices',
    crumb: breadcrumbs.NOMINATED_PHARMACY_ONLINE_ONLY_CHOICES_CRUMB,
    upliftRoute: UPLIFT_PRESCRIPTIONS,
    proofLevel: proofLevel.P9,
    helpUrl: nominatedPharmacyHelpUrl,
    nativeNavigation: PRESCRIPTIONS_MENU_ITEM,
  },
};

export const NOMINATED_PHARMACY_ONLINE_ONLY_SEARCH = {
  path: NOMINATED_PHARMACY_ONLINE_ONLY_SEARCH_PATH,
  name: NOMINATED_PHARMACY_ONLINE_ONLY_SEARCH_NAME,
  component: NominatedPharmacyOnlyOnlySearchPage,
  meta: {
    headerKey: 'navigation.pages.headers.nominatedPharmacyOnlineOnlySearch',
    titleKey: 'navigation.pages.titles.nominatedPharmacyOnlineOnlySearch',
    crumb: breadcrumbs.NOMINATED_PHARMACY_ONLINE_ONLY_SEARCH_CRUMB,
    upliftRoute: UPLIFT_PRESCRIPTIONS,
    proofLevel: proofLevel.P9,
    helpUrl: nominatedPharmacyHelpUrl,
    nativeNavigation: PRESCRIPTIONS_MENU_ITEM,
  },
};

export const NOMINATED_PHARMACY_SEARCH_RESULTS = {
  path: NOMINATED_PHARMACY_SEARCH_RESULTS_PATH,
  name: NOMINATED_PHARMACY_SEARCH_RESULTS_NAME,
  component: NominatedPharmacySearchResultsPage,
  meta: {
    headerKey: (store, i18n) => {
      const pharmacyTypeChoice = store.state.nominatedPharmacy.chosenType;
      if (pharmacyTypeChoice === PharmacyTypeChoice.ONLINE_PHARMACY) {
        if (store.state.nominatedPharmacy.onlineOnlyKnownOption === true) {
          return i18n.t('nominatedPharmacy.searchResults.online.search.header',
            { searchQuery: store.state.nominatedPharmacy.searchQuery });
        }
        return i18n.t('nominatedPharmacy.searchResults.online.random.header');
      }
      return i18n.t('nominatedPharmacy.searchResults.highStreet.header',
        { searchQuery: store.state.nominatedPharmacy.searchQuery });
    },
    titleKey: (store, i18n) => {
      const pharmacyTypeChoice = store.state.nominatedPharmacy.chosenType;
      if (pharmacyTypeChoice === PharmacyTypeChoice.ONLINE_PHARMACY) {
        if (store.state.nominatedPharmacy.onlineOnlyKnownOption === true) {
          return i18n.t('nominatedPharmacy.searchResults.online.search.title',
            { searchQuery: store.state.nominatedPharmacy.searchQuery });
        }
        return i18n.t('nominatedPharmacy.searchResults.online.random.title');
      }
      return i18n.t('nominatedPharmacy.searchResults.highStreet.title',
        { searchQuery: store.state.nominatedPharmacy.searchQuery });
    },
    crumb: breadcrumbs.NOMINATED_PHARMACY_SEARCH_RESULTS_CRUMB,
    upliftRoute: UPLIFT_PRESCRIPTIONS,
    proofLevel: proofLevel.P9,
    helpUrl: nominatedPharmacyHelpUrl,
    nativeNavigation: PRESCRIPTIONS_MENU_ITEM,
  },
};

export const NOMINATED_PHARMACY_SEARCH = {
  path: NOMINATED_PHARMACY_SEARCH_PATH,
  name: NOMINATED_PHARMACY_SEARCH_NAME,
  component: NominatedPharmacySearchPage,
  meta: {
    headerKey: 'navigation.pages.headers.searchNominatedPharmacy',
    titleKey: 'navigation.pages.titles.searchNominatedPharmacy',
    crumb: breadcrumbs.NOMINATED_PHARMACY_SEARCH_CRUMB,
    upliftRoute: UPLIFT_PRESCRIPTIONS,
    proofLevel: proofLevel.P9,
    helpUrl: nominatedPharmacyHelpUrl,
    nativeNavigation: PRESCRIPTIONS_MENU_ITEM,
  },
};

export default [
  NOMINATED_PHARMACY_CHANGE_SUCCESS,
  NOMINATED_PHARMACY_CHOOSE_TYPE,
  NOMINATED_PHARMACY_CHECK,
  NOMINATED_PHARMACY_CONFIRM,
  NOMINATED_PHARMACY_DSP_INTERRUPT,
  NOMINATED_PHARMACY_INDEX,
  NOMINATED_PHARMACY_INTERRUPT,
  NOMINATED_PHARMACY_ONLINE_ONLY_CHOICES,
  NOMINATED_PHARMACY_ONLINE_ONLY_SEARCH,
  NOMINATED_PHARMACY_SEARCH_RESULTS,
  NOMINATED_PHARMACY_SEARCH,
];
