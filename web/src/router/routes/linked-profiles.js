import LinkedProfilesPage from '@/pages/linked-profiles/index';
import LinkedProfilesSummaryPage from '@/pages/linked-profiles/summary';
import LinkedProfilesMoreShutterPage from '@/pages/linked-profiles/shutter/more';
import LinkedProfilesSymptomsShutterPage from '@/pages/linked-profiles/shutter/symptoms';
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
  LINKED_PROFILES_SHUTTER_SYMPTOMS_PATH,
  LINKED_PROFILES_SHUTTER_SETTINGS_PATH,
  LINKED_PROFILES_SHUTTER_APPOINTMENTS_PATH,
  LINKED_PROFILES_SHUTTER_PRESCRIPTIONS_PATH,
  SWITCH_PROFILE_PATH,
} from '@/router/paths';
import {
  LINKED_PROFILES_NAME,
  LINKED_PROFILES_SUMMARY_NAME,
  LINKED_PROFILES_SHUTTER_MORE_NAME,
  LINKED_PROFILES_SHUTTER_SYMPTOMS_NAME,
  LINKED_PROFILES_SHUTTER_SETTINGS_NAME,
  LINKED_PROFILES_SHUTTER_APPOINTMENTS_NAME,
  LINKED_PROFILES_SHUTTER_PRESCRIPTIONS_NAME,
  SWITCH_PROFILE_NAME,
} from '@/router/names';
import {
  MORE_MENU_ITEM,
  SYMPTOMS_MENU_ITEM,
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
    headerKey: 'pageHeaders.linkedProfiles',
    titleKey: 'pageTitles.linkedProfiles',
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
      return i18n.t('pageHeaders.linkedProfilesSummary', { fullName: linkedAccount.fullName });
    },
    titleKey: (store, i18n) => {
      const linkedAccount = store.getters['linkedAccounts/getSelectedLinkedAccount'];
      return i18n.t('pageTitles.linkedProfilesSummary', { fullName: linkedAccount.fullName });
    },
    crumb: breadcrumbs.LINKED_PROFILES_SUMMARY_CRUMB,
    proofLevel: proofLevel.P5,
    helpUrl: proxyHelpUrl,
  },
};

export const LINKED_PROFILES_SHUTTER_MORE = {
  path: LINKED_PROFILES_SHUTTER_MORE_PATH,
  name: LINKED_PROFILES_SHUTTER_MORE_NAME,
  component: LinkedProfilesMoreShutterPage,
  meta: {
    headerKey: 'linkedProfiles.shutter.more.header',
    titleKey: 'linkedProfiles.shutter.more.header',
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
    headerKey: 'linkedProfiles.shutter.settings.header',
    titleKey: 'linkedProfiles.shutter.settings.header',
    crumb: breadcrumbs.LINKED_PROFILES_SETTINGS_SHUTTER_CRUMB,
    proofLevel: proofLevel.P5,
    helpUrl: proxyHelpUrl,
    nativeNavigation: CLEAR_SELECTED_MENU_ITEM,
  },
};

export const LINKED_PROFILES_SHUTTER_SYMPTOMS = {
  path: LINKED_PROFILES_SHUTTER_SYMPTOMS_PATH,
  name: LINKED_PROFILES_SHUTTER_SYMPTOMS_NAME,
  component: LinkedProfilesSymptomsShutterPage,
  meta: {
    headerKey: 'linkedProfiles.shutter.symptoms.header',
    titleKey: 'linkedProfiles.shutter.symptoms.header',
    crumb: breadcrumbs.LINKED_PROFILES_SYMPTOMS_SHUTTER_CRUMB,
    proofLevel: proofLevel.P5,
    helpUrl: proxyHelpUrl,
    nativeNavigation: SYMPTOMS_MENU_ITEM,
  },
};

export const LINKED_PROFILES_SHUTTER_APPOINTMENTS = {
  name: LINKED_PROFILES_SHUTTER_APPOINTMENTS_NAME,
  path: LINKED_PROFILES_SHUTTER_APPOINTMENTS_PATH,
  component: LinkedProfilesAppointmentsShutterPage,
  meta: {
    headerKey: (store, i18n) => {
      const givenName = get('state.linkedAccounts.actingAsUser.givenName')(store);
      return i18n.t('linkedProfiles.shutter.appointments.header', { name: givenName });
    },
    titleKey: (store, i18n) => {
      const givenName = get('state.linkedAccounts.actingAsUser.givenName')(store);
      return i18n.t('linkedProfiles.shutter.appointments.header', { name: givenName });
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
      return i18n.t('linkedProfiles.shutter.prescriptions.header', { name: givenName });
    },
    titleKey: (store, i18n) => {
      const givenName = get('state.linkedAccounts.actingAsUser.givenName')(store);
      return i18n.t('linkedProfiles.shutter.prescriptions.header', { name: givenName });
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
    headerKey: 'pageHeaders.switchProfile',
    titleKey: 'pageTitles.switchProfile',
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
  LINKED_PROFILES_SHUTTER_SYMPTOMS,
  LINKED_PROFILES_SHUTTER_APPOINTMENTS,
  LINKED_PROFILES_SHUTTER_PRESCRIPTIONS,
  SWITCH_PROFILE,
];
