import {
  ACCOUNT_PATH,
  APPOINTMENTS_PATH,
  INDEX_PATH,
  HEALTH_RECORDS_PATH,
  MESSAGES_PATH,
  GP_MESSAGES_PATH,
  PRESCRIPTIONS_PATH,
} from '@/router/paths';
import {
  ACCOUNT_COOKIES_NAME,
  GP_APPOINTMENTS_NAME,
  APPOINTMENT_BOOKING_SUCCESS_NAME,
  APPOINTMENT_CANCELLING_SUCCESS_NAME,
  LINKED_PROFILES_SHUTTER_APPOINTMENTS_NAME,
  LINKED_PROFILES_SHUTTER_PRESCRIPTIONS_NAME,
  ORGAN_DONATION_NAME,
  ORGAN_DONATION_VIEW_DECISION_NAME,
  PRESCRIPTION_REPEAT_COURSES_NAME,
  PRESCRIPTIONS_VIEW_ORDERS_NAME,
  GP_MESSAGES_NAME,
  GP_MESSAGES_URGENCY_NAME,
  GP_MESSAGES_VIEW_MESSAGE_NAME,
  SWITCH_PROFILE_NAME,
  LINKED_PROFILES_NAME,
} from '@/router/names';

/**
 * Overrides for the back link on native
 *
 * For these routes, when clicking the back nav on native, the defaultPath will be used if
 * there is no state.navigation.backLinkOverride path set in the store. If this value is
 * to be ignored and the default path is to always be used, be sure to set ignoreStore: true
 */
export default {
  [ACCOUNT_COOKIES_NAME]: {
    ignoreStore: true,
    defaultPath: ACCOUNT_PATH,
  },
  [APPOINTMENT_BOOKING_SUCCESS_NAME]: {
    ignoreStore: true,
    defaultPath: APPOINTMENTS_PATH,
  },
  [GP_APPOINTMENTS_NAME]: {
    ignoreStore: true,
    defaultPath: APPOINTMENTS_PATH,
  },
  [APPOINTMENT_CANCELLING_SUCCESS_NAME]: {
    ignoreStore: true,
    defaultPath: APPOINTMENTS_PATH,
  },
  [LINKED_PROFILES_NAME]: {
    defaultPath: ACCOUNT_PATH,
  },
  [LINKED_PROFILES_SHUTTER_APPOINTMENTS_NAME]: {
    ignoreStore: true,
    defaultPath: APPOINTMENTS_PATH,
  },
  [LINKED_PROFILES_SHUTTER_PRESCRIPTIONS_NAME]: {
    ignoreStore: true,
    defaultPath: INDEX_PATH,
  },
  [ORGAN_DONATION_NAME]: {
    defaultPath: HEALTH_RECORDS_PATH,
  },
  [ORGAN_DONATION_VIEW_DECISION_NAME]: {
    defaultPath: HEALTH_RECORDS_PATH,
  },
  [PRESCRIPTION_REPEAT_COURSES_NAME]: {
    ignoreStore: true,
    defaultPath: PRESCRIPTIONS_PATH,
  },
  [PRESCRIPTIONS_VIEW_ORDERS_NAME]: {
    ignoreStore: true,
    defaultPath: PRESCRIPTIONS_PATH,
  },
  [GP_MESSAGES_NAME]: {
    ignoreStore: true,
    defaultPath: MESSAGES_PATH,
  },
  [GP_MESSAGES_URGENCY_NAME]: {
    defaultPath: GP_MESSAGES_PATH,
  },
  [GP_MESSAGES_VIEW_MESSAGE_NAME]: {
    ignoreStore: true,
    defaultPath: GP_MESSAGES_PATH,
  },
  [SWITCH_PROFILE_NAME]: {
    ignoreStore: true,
    defaultPath: INDEX_PATH,
  },
};
