import LinkedProfilesPage from '@/pages/linked-profiles/index';
import LinkedProfilesSummaryPage from '@/pages/linked-profiles/summary';
import LinkedProfilesMoreShutterPage from '@/pages/linked-profiles/shutter/more';
import LinkedProfilesAdviceShutterPage from '@/pages/linked-profiles/shutter/advice';
import LinkedProfilesSettingsShutterPage from '@/pages/linked-profiles/shutter/settings';
import LinkedProfilesAppointmentsShutterPage from '@/pages/linked-profiles/shutter/appointments';
import LinkedProfilesPrescriptionsShutterPage from '@/pages/linked-profiles/shutter/prescriptions';
import SwitchProfilePage from '@/pages/switch-profile';

import proofLevel from '@/lib/proofLevel';
import { appointmentsHelpUrl, prescriptionsHelpUrl, proxyHelpUrl } from '@/router/externalLinks';
import breadcrumbs from '@/breadcrumbs/linked-profiles';
import {
  LINKED_PROFILES_PATH,
  LINKED_PROFILES_SUMMARY_PATH,
  LINKED_PROFILES_SHUTTER_MORE_PATH,
  LINKED_PROFILES_SHUTTER_ADVICE_PATH,
  LINKED_PROFILES_SHUTTER_SETTINGS_PATH,
  LINKED_PROFILES_SHUTTER_APPOINTMENTS_PATH,
  LINKED_PROFILES_SHUTTER_PRESCRIPTIONS_PATH,
  SWITCH_PROFILE_PATH,
} from '@/router/paths';
import {
  LINKED_PROFILES_NAME,
  LINKED_PROFILES_SUMMARY_NAME,
  LINKED_PROFILES_SHUTTER_MORE_NAME,
  LINKED_PROFILES_SHUTTER_ADVICE_NAME,
  LINKED_PROFILES_SHUTTER_SETTINGS_NAME,
  LINKED_PROFILES_SHUTTER_APPOINTMENTS_NAME,
  LINKED_PROFILES_SHUTTER_PRESCRIPTIONS_NAME,
  SWITCH_PROFILE_NAME,
} from '@/router/names';
import {
  MORE_MENU_ITEM,
  ADVICE_MENU_ITEM,
  CLEAR_SELECTED_MENU_ITEM,
  APPOINTMENTS_MENU_ITEM,
  PRESCRIPTIONS_MENU_ITEM,
} from '@/middleware/nativeNavigation';
import get from 'lodash/fp/get';

export const LINKED_PROFILES = {
  path: LINKED_PROFILES_PATH,
  name: LINKED_PROFILES_NAME,
  component: LinkedProfilesPage,
  meta: {
    headerKey: 'navigation.pages.headers.linkedProfiles',
    titleKey: 'navigation.pages.titles.linkedProfiles',
    crumb: breadcrumbs.LINKED_PROFILES_CRUMB,
    proofLevel: proofLevel.P5,
    helpUrl: proxyHelpUrl,
    // nativeNavigation: ?,
  },
};

export const LINKED_PROFILES_SUMMARY = {
  path: LINKED_PROFILES_SUMMARY_PATH,
  name: LINKED_PROFILES_SUMMARY_NAME,
  component: LinkedProfilesSummaryPage,
  meta: {
    headerKey: (store, i18n) => {
      const linkedAccount = store.getters['linkedAccounts/getSelectedLinkedAccount'];
      return i18n.t('navigation.pages.headers.linkedProfilesSummary', { fullName: linkedAccount.fullName });
    },
    titleKey: (store, i18n) => {
      const linkedAccount = store.getters['linkedAccounts/getSelectedLinkedAccount'];
      return i18n.t('navigation.pages.titles.linkedProfilesSummary', { fullName: linkedAccount.fullName });
    },
    crumb: breadcrumbs.LINKED_PROFILES_CRUMB,
    proofLevel: proofLevel.P5,
    helpUrl: proxyHelpUrl,
  },
};

export const LINKED_PROFILES_SHUTTER_MORE = {
  path: LINKED_PROFILES_SHUTTER_MORE_PATH,
  name: LINKED_PROFILES_SHUTTER_MORE_NAME,
  component: LinkedProfilesMoreShutterPage,
  meta: {
    headerKey: 'profiles.shutter.more.header',
    titleKey: 'profiles.shutter.more.header',
    crumb: breadcrumbs.LINKED_PROFILES_MORE_SHUTTER_CRUMB,
    proofLevel: proofLevel.P5,
    helpUrl: proxyHelpUrl,
    nativeNavigation: MORE_MENU_ITEM,
  },
};

export const LINKED_PROFILES_SHUTTER_SETTINGS = {
  path: LINKED_PROFILES_SHUTTER_SETTINGS_PATH,
  name: LINKED_PROFILES_SHUTTER_SETTINGS_NAME,
  component: LinkedProfilesSettingsShutterPage,
  meta: {
    headerKey: 'profiles.shutter.settings.header',
    titleKey: 'profiles.shutter.settings.header',
    crumb: breadcrumbs.LINKED_PROFILES_SETTINGS_SHUTTER_CRUMB,
    proofLevel: proofLevel.P5,
    helpUrl: proxyHelpUrl,
    nativeNavigation: CLEAR_SELECTED_MENU_ITEM,
  },
};

export const LINKED_PROFILES_SHUTTER_ADVICE = {
  path: LINKED_PROFILES_SHUTTER_ADVICE_PATH,
  name: LINKED_PROFILES_SHUTTER_ADVICE_NAME,
  component: LinkedProfilesAdviceShutterPage,
  meta: {
    headerKey: 'profiles.shutter.advice.header',
    titleKey: 'profiles.shutter.advice.header',
    crumb: breadcrumbs.LINKED_PROFILES_ADVICE_SHUTTER_CRUMB,
    proofLevel: proofLevel.P5,
    helpUrl: proxyHelpUrl,
    nativeNavigation: ADVICE_MENU_ITEM,
  },
};

export const LINKED_PROFILES_SHUTTER_APPOINTMENTS = {
  name: LINKED_PROFILES_SHUTTER_APPOINTMENTS_NAME,
  path: LINKED_PROFILES_SHUTTER_APPOINTMENTS_PATH,
  component: LinkedProfilesAppointmentsShutterPage,
  meta: {
    headerKey: (store, i18n) => {
      const givenName = get('state.linkedAccounts.actingAsUser.givenName')(store);
      return i18n.t('profiles.shutter.appointments.header', { name: givenName });
    },
    titleKey: (store, i18n) => {
      const givenName = get('state.linkedAccounts.actingAsUser.givenName')(store);
      return i18n.t('profiles.shutter.appointments.header', { name: givenName });
    },
    crumb: breadcrumbs.LINKED_PROFILES_APPOINTMENTS_SHUTTER_CRUMB,
    proofLevel: proofLevel.P5,
    helpUrl: appointmentsHelpUrl,
    nativeNavigation: APPOINTMENTS_MENU_ITEM,
  },
};

export const LINKED_PROFILES_SHUTTER_PRESCRIPTIONS = {
  name: LINKED_PROFILES_SHUTTER_PRESCRIPTIONS_NAME,
  path: LINKED_PROFILES_SHUTTER_PRESCRIPTIONS_PATH,
  component: LinkedProfilesPrescriptionsShutterPage,
  meta: {
    headerKey: (store, i18n) => {
      const givenName = get('state.linkedAccounts.actingAsUser.givenName')(store);
      return i18n.t('profiles.shutter.prescriptions.header', { name: givenName });
    },
    titleKey: (store, i18n) => {
      const givenName = get('state.linkedAccounts.actingAsUser.givenName')(store);
      return i18n.t('profiles.shutter.prescriptions.header', { name: givenName });
    },
    crumb: breadcrumbs.LINKED_PROFILES_PRESCRIPTIONS_SHUTTER_CRUMB,
    proofLevel: proofLevel.P5,
    helpUrl: prescriptionsHelpUrl,
    nativeNavigation: PRESCRIPTIONS_MENU_ITEM,
  },
};

export const SWITCH_PROFILE = {
  name: SWITCH_PROFILE_PATH,
  path: SWITCH_PROFILE_NAME,
  component: SwitchProfilePage,
  meta: {
    headerKey: 'navigation.pages.headers.switchProfile',
    titleKey: 'navigation.pages.titles.switchProfile',
    crumb: breadcrumbs.SWITCH_PROFILE_CRUMB,
    proofLevel: proofLevel.P5,
    helpUrl: proxyHelpUrl,
  },
};

export default [
  LINKED_PROFILES,
  LINKED_PROFILES_SUMMARY,
  LINKED_PROFILES_SHUTTER_MORE,
  LINKED_PROFILES_SHUTTER_SETTINGS,
  LINKED_PROFILES_SHUTTER_ADVICE,
  LINKED_PROFILES_SHUTTER_APPOINTMENTS,
  LINKED_PROFILES_SHUTTER_PRESCRIPTIONS,
  SWITCH_PROFILE,
];
