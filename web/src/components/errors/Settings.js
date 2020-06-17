/* eslint-disable import/extensions */
import assign from 'lodash/fp/assign';
import has from 'lodash/fp/has';
import {
  ACCOUNT_PATH,
  ACCOUNT_NOTIFICATIONS_PATH,
  ALLERGIESANDREACTIONS_PATH,
  AUTH_RETURN_PATH,
  DOCUMENT_PATH,
  DOCUMENTS_PATH,
  GP_MEDICAL_RECORD_PATH,
  HEALTH_RECORDS_PATH,
  HEALTH_INFORMATION_UPDATES_PATH,
  HEALTH_INFORMATION_UPDATES_MESSAGES_PATH,
  NOMINATED_PHARMACY_CONFIRM_PATH,
  NOMINATED_PHARMACY_SEARCH_PATH,
  ORGAN_DONATION_PATH,
  ORGAN_DONATION_REVIEW_YOUR_DECISION_PATH,
  PRESCRIPTIONS_PATH,
  PRESCRIPTION_CONFIRM_COURSES_PATH,
  PRESCRIPTIONS_VIEW_ORDERS_PATH,
  PRESCRIPTION_REPEAT_COURSES_PATH,
  TESTRESULTID_PATH,
  TESTRESULTS_PATH,
  TESTRESULTSDETAIL_PATH,
  APPOINTMENT_ADMIN_HELP_PATH,
  APPOINTMENT_GP_ADVICE_PATH,
  LOGIN_PATH,
  BEGINLOGIN_PATH,
  GP_MESSAGES_PATH,
  SYMPTOMS_PATH,
  MESSAGES_PATH,
  GP_MESSAGES_VIEW_MESSAGE_PATH,
  GP_MESSAGES_URGENCY_PATH,
  GP_MESSAGES_URGENCY_CONTACT_GP_PATH,
  GP_MESSAGES_RECIPIENTS_PATH,
  GP_MESSAGES_CREATE_PATH,
  GP_MESSAGES_DELETE_PATH,
} from '@/router/paths';

export default {
  default: {
    action: null,
    additionalInfoComponent: null,
    errorOverrideStyles: {},
    ignoredErrors: [],
    redirectUrl: null,
    showApiError: true,
  },
  pages: [
    {
      route: ACCOUNT_NOTIFICATIONS_PATH,
      action: {
        10002: 'notifications/retryToggle',
      },
      errorOverrideStyles: { 403: 'plain' },
      redirectUrl: {
        default: ACCOUNT_PATH,
        10001: ACCOUNT_NOTIFICATIONS_PATH,
      },
    },
    {
      route: ALLERGIESANDREACTIONS_PATH,
      redirectUrl: {
        default: ALLERGIESANDREACTIONS_PATH,
      },
    },
    {
      route: APPOINTMENT_ADMIN_HELP_PATH,
      ignoredErrors: [480, 580],
    },
    {
      route: APPOINTMENT_GP_ADVICE_PATH,
      ignoredErrors: [480, 580],
    },
    {
      route: AUTH_RETURN_PATH,
      redirectUrl: {
        default: LOGIN_PATH,
      },
    },
    {
      route: BEGINLOGIN_PATH,
      redirectUrl: {
        default: LOGIN_PATH,
      },
    },
    {
      route: GP_MEDICAL_RECORD_PATH,
      redirectUrl: {
        default: GP_MEDICAL_RECORD_PATH,
      },
    },
    {
      route: HEALTH_RECORDS_PATH,
      redirectUrl: {
        default: HEALTH_RECORDS_PATH,
      },
    },
    {
      route: HEALTH_INFORMATION_UPDATES_PATH,
      redirectUrl: {
        default: HEALTH_INFORMATION_UPDATES_PATH,
      },
    },
    {
      route: HEALTH_INFORMATION_UPDATES_MESSAGES_PATH,
      redirectUrl: {
        default: HEALTH_INFORMATION_UPDATES_MESSAGES_PATH,
      },
      additionalInfoComponent: 'MessagesSenderError',
    },
    {
      route: DOCUMENT_PATH,
      redirectUrl: {
        default: GP_MEDICAL_RECORD_PATH,
      },
    },
    {
      route: DOCUMENTS_PATH,
      redirectUrl: {
        default: GP_MEDICAL_RECORD_PATH,
      },
    },
    {
      route: NOMINATED_PHARMACY_CONFIRM_PATH,
      redirectUrl: {
        default: PRESCRIPTIONS_PATH,
      },
    },
    {
      route: NOMINATED_PHARMACY_SEARCH_PATH,
      redirectUrl: {
        default: PRESCRIPTIONS_PATH,
      },
    },
    {
      route: ORGAN_DONATION_PATH,
      additionalInfoComponent: 'ContactOrganDonation',
      redirectUrl: {
        1: ORGAN_DONATION_PATH,
      },
    },
    {
      route: ORGAN_DONATION_REVIEW_YOUR_DECISION_PATH,
      action: {
        1: 'organDonation/submitDecision',
      },
      additionalInfoComponent: 'ContactOrganDonation',
    },
    {
      route: GP_MESSAGES_PATH,
      errorOverrideStyles: { 403: 'plain' },
      action: {
        400: 'gpMessages/clearErrorsAndLoadMessages',
      },
      redirectUrl: {
        default: MESSAGES_PATH,
      },
    },
    {
      route: GP_MESSAGES_VIEW_MESSAGE_PATH,
      action: {
        400: 'gpMessages/clearErrorsAndLoadDetails',
      },
      redirectUrl: {
        default: MESSAGES_PATH,
      },
    },
    {
      route: GP_MESSAGES_URGENCY_PATH,
      redirectUrl: {
        default: MESSAGES_PATH,
      },
    },
    {
      route: GP_MESSAGES_URGENCY_CONTACT_GP_PATH,
      redirectUrl: {
        default: MESSAGES_PATH,
      },
    },
    {
      route: GP_MESSAGES_RECIPIENTS_PATH,
      redirectUrl: {
        default: MESSAGES_PATH,
      },
    },
    {
      route: GP_MESSAGES_CREATE_PATH,
      redirectUrl: {
        default: MESSAGES_PATH,
      },
    },
    {
      route: GP_MESSAGES_DELETE_PATH,
      action: {
        400: 'gpMessages/retryMessageDelete',
      },
    },
    {
      route: PRESCRIPTIONS_PATH,
      errorOverrideStyles: { 403: 'plain' },
      redirectUrl: {
        default: PRESCRIPTIONS_PATH,
      },
    },
    {
      route: PRESCRIPTIONS_VIEW_ORDERS_PATH,
      errorOverrideStyles: { 403: 'plain' },
      redirectUrl: {
        default: PRESCRIPTIONS_PATH,
      },
    },
    {
      route: PRESCRIPTION_CONFIRM_COURSES_PATH,
      errorOverrideStyles: { 403: 'plain' },
      redirectUrl: {
        default: PRESCRIPTIONS_PATH,
      },
    },
    {
      route: PRESCRIPTION_CONFIRM_COURSES_PATH,
      errorOverrideStyles: { 466: 'plain' },
      redirectUrl: {
        default: PRESCRIPTIONS_PATH,
      },
    },
    {
      route: PRESCRIPTION_REPEAT_COURSES_PATH,
      errorOverrideStyles: { 403: 'plain' },
      redirectUrl: {
        default: PRESCRIPTIONS_PATH,
      },
    },
    {
      route: SYMPTOMS_PATH,
      redirectUrl: {
        default: SYMPTOMS_PATH,
      },
    },
    {
      route: TESTRESULTID_PATH,
      redirectUrl: {
        default: TESTRESULTID_PATH,
      },
    },
    {
      route: TESTRESULTS_PATH,
      redirectUrl: {
        default: TESTRESULTS_PATH,
      },
    },
    {
      route: TESTRESULTSDETAIL_PATH,
      redirectUrl: {
        default: TESTRESULTSDETAIL_PATH,
      },
    },
  ],
  forPage(routePath) {
    let settings = assign({}, this.default);
    for (let i = 0, max = this.pages.length; i < max; i += 1) {
      if (this.pages[i].route === routePath) {
        settings = assign({}, this.pages[i]);
        if (!has('action', settings)) settings.action = this.default.action;
        if (!has('additionalInfoComponent', settings)) settings.additionalInfoComponent = this.default.additionalInfoComponent;
        if (!has('errorOverrideStyles', settings)) settings.errorOverrideStyles = this.default.errorOverrideStyles;
        if (!has('ignoredErrors', settings)) settings.ignoredErrors = this.default.ignoredErrors;
        if (!has('redirectUrl', settings)) settings.redirectUrl = this.default.redirectUrl;
        if (!has('showApiError', settings)) settings.showApiError = this.default.showApiError;
        break;
      }
    }

    return settings;
  },
};
