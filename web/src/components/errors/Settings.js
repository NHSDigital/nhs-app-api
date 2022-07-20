/* eslint-disable import/extensions */
import assign from 'lodash/fp/assign';
import has from 'lodash/fp/has';
import {
  ALLERGIESANDREACTIONS_PATH,
  AUTH_RETURN_PATH,
  DOCUMENT_PATH,
  DOCUMENTS_PATH,
  GP_MEDICAL_RECORD_PATH,
  HEALTH_RECORDS_PATH,
  MORE_ACCOUNTANDSETTINGS_PATH,
  MORE_ACCOUNTANDSETTINGS_MANAGENOTIFICATIONS_PATH,
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
  TESTRESULTS_V2_PATH,
  CHOOSE_TEST_RESULT_YEAR_PATH,
  TEST_RESULTS_FOR_YEAR_PATH,
  TESTRESULTSDETAIL_PATH,
  APPOINTMENT_ADMIN_HELP_PATH,
  GP_ADVICE_PATH,
  LOGIN_PATH,
  BEGINLOGIN_PATH,
  GP_MESSAGES_PATH,
  ADVICE_PATH,
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
      route: GP_ADVICE_PATH,
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
      route: MORE_ACCOUNTANDSETTINGS_MANAGENOTIFICATIONS_PATH,
      errorOverrideStyles: { 403: 'plain', 500: 'plain', 502: 'plain' },
      redirectUrl: {
        default: MORE_ACCOUNTANDSETTINGS_PATH,
        10001: MORE_ACCOUNTANDSETTINGS_MANAGENOTIFICATIONS_PATH,
      },
      backLinks: {
        500: MORE_ACCOUNTANDSETTINGS_PATH,
        10001: null,
        10002: null,
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
      errorOverrideStyles: { 500: 'plain', 502: 'plain' },
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
      errorOverrideStyles: { 403: 'plain', 400: 'plain' },
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
      hideBreadCrumb: {
        466: true,
      },
      backLinks: {
        466: PRESCRIPTIONS_PATH,
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
      route: ADVICE_PATH,
      redirectUrl: {
        default: ADVICE_PATH,
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
      route: TESTRESULTS_V2_PATH,
      redirectUrl: {
        default: TESTRESULTS_V2_PATH,
      },
    },
    {
      route: CHOOSE_TEST_RESULT_YEAR_PATH,
      redirectUrl: {
        default: CHOOSE_TEST_RESULT_YEAR_PATH,
      },
    },
    {
      route: TEST_RESULTS_FOR_YEAR_PATH,
      redirectUrl: {
        default: TEST_RESULTS_FOR_YEAR_PATH,
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
