/* eslint-disable import/extensions */
import assign from 'lodash/fp/assign';
import has from 'lodash/fp/has';
import {
  ACCOUNT,
  ACCOUNT_NOTIFICATIONS,
  ALLERGIESANDREACTIONS,
  APPOINTMENT_ADMIN_HELP,
  APPOINTMENT_GP_ADVICE,
  AUTH_RETURN,
  BEGINLOGIN,
  DOCUMENT,
  DOCUMENTS,
  GP_MEDICAL_RECORD,
  HEALTH_RECORDS,
  LOGIN,
  HEALTH_INFORMATION_UPDATES,
  HEALTH_INFORMATION_UPDATES_MESSAGES,
  MESSAGES,
  MYRECORD,
  MYRECORDTESTRESULT,
  NOMINATED_PHARMACY_CONFIRM,
  NOMINATED_PHARMACY_SEARCH,
  ORGAN_DONATION,
  ORGAN_DONATION_REVIEW_YOUR_DECISION,
  GP_MESSAGES,
  GP_MESSAGES_VIEW_MESSAGE,
  GP_MESSAGES_URGENCY,
  GP_MESSAGES_URGENCY_CONTACT_GP,
  GP_MESSAGES_RECIPIENTS,
  GP_MESSAGES_CREATE,
  GP_MESSAGES_DELETE,
  PRESCRIPTIONS,
  PRESCRIPTION_CONFIRM_COURSES,
  PRESCRIPTION_REPEAT_COURSES,
  TESTRESULTID,
  TESTRESULTS,
  TESTRESULTSDETAIL,
} from '@/lib/routes';

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
      route: ACCOUNT_NOTIFICATIONS.path,
      action: {
        10002: 'notifications/retryToggle',
      },
      errorOverrideStyles: { 403: 'plain' },
      redirectUrl: {
        default: ACCOUNT.path,
        10001: ACCOUNT_NOTIFICATIONS.path,
      },
    },
    {
      route: ALLERGIESANDREACTIONS.path,
      redirectUrl: {
        default: ALLERGIESANDREACTIONS.path,
      },
    },
    {
      route: APPOINTMENT_ADMIN_HELP.path,
      ignoredErrors: [480, 580],
    },
    {
      route: APPOINTMENT_GP_ADVICE.path,
      ignoredErrors: [480, 580],
    },
    {
      route: AUTH_RETURN.path,
      redirectUrl: {
        default: LOGIN.path,
      },
    },
    {
      route: BEGINLOGIN.path,
      redirectUrl: {
        default: LOGIN.path,
      },
    },
    {
      route: GP_MEDICAL_RECORD.path,
      redirectUrl: {
        default: GP_MEDICAL_RECORD.path,
      },
    },
    {
      route: HEALTH_RECORDS.path,
      redirectUrl: {
        default: HEALTH_RECORDS.path,
      },
    },
    {
      route: HEALTH_INFORMATION_UPDATES.path,
      redirectUrl: {
        default: HEALTH_INFORMATION_UPDATES.path,
      },
    },
    {
      route: HEALTH_INFORMATION_UPDATES_MESSAGES.path,
      redirectUrl: {
        default: HEALTH_INFORMATION_UPDATES_MESSAGES.path,
      },
      additionalInfoComponent: 'MessagesSenderError',
    },
    {
      route: MYRECORD.path,
      redirectUrl: {
        default: MYRECORD.path,
      },
    },
    {
      route: MYRECORDTESTRESULT.path,
      redirectUrl: {
        default: MYRECORD.path,
      },
    },
    {
      route: DOCUMENT.path,
      redirectUrl: {
        default: GP_MEDICAL_RECORD.path,
      },
    },
    {
      route: DOCUMENTS.path,
      redirectUrl: {
        default: GP_MEDICAL_RECORD.path,
      },
    },
    {
      route: NOMINATED_PHARMACY_CONFIRM.path,
      redirectUrl: {
        default: PRESCRIPTIONS.path,
      },
    },
    {
      route: NOMINATED_PHARMACY_SEARCH.path,
      redirectUrl: {
        default: PRESCRIPTIONS.path,
      },
    },
    {
      route: ORGAN_DONATION.path,
      additionalInfoComponent: 'ContactOrganDonation',
      redirectUrl: {
        1: ORGAN_DONATION.path,
      },
    },
    {
      route: ORGAN_DONATION_REVIEW_YOUR_DECISION.path,
      action: {
        1: 'organDonation/submitDecision',
      },
      additionalInfoComponent: 'ContactOrganDonation',
    },
    {
      route: GP_MESSAGES.path,
      errorOverrideStyles: { 403: 'plain' },
      action: {
        400: 'gpMessages/clearErrorsAndLoadMessages',
      },
      redirectUrl: {
        default: MESSAGES.path,
      },
    },
    {
      route: GP_MESSAGES_VIEW_MESSAGE.path,
      action: {
        400: 'gpMessages/clearErrorsAndLoadDetails',
      },
      redirectUrl: {
        default: MESSAGES.path,
      },
    },
    {
      route: GP_MESSAGES_URGENCY.path,
      redirectUrl: {
        default: MESSAGES.path,
      },
    },
    {
      route: GP_MESSAGES_URGENCY_CONTACT_GP.path,
      redirectUrl: {
        default: MESSAGES.path,
      },
    },
    {
      route: GP_MESSAGES_RECIPIENTS.path,
      redirectUrl: {
        default: MESSAGES.path,
      },
    },
    {
      route: GP_MESSAGES_CREATE.path,
      redirectUrl: {
        default: MESSAGES.path,
      },
    },
    {
      route: GP_MESSAGES_DELETE.path,
      action: {
        400: 'gpMessages/retryMessageDelete',
      },
    },
    {
      route: PRESCRIPTIONS.path,
      errorOverrideStyles: { 403: 'plain' },
      redirectUrl: {
        default: PRESCRIPTIONS.path,
      },
    },
    {
      route: PRESCRIPTION_CONFIRM_COURSES.path,
      errorOverrideStyles: { 403: 'plain' },
      redirectUrl: {
        default: PRESCRIPTIONS.path,
      },
    },
    {
      route: PRESCRIPTION_CONFIRM_COURSES.path,
      errorOverrideStyles: { 466: 'plain' },
      redirectUrl: {
        default: PRESCRIPTIONS.path,
      },
    },
    {
      route: PRESCRIPTION_REPEAT_COURSES.path,
      errorOverrideStyles: { 403: 'plain' },
      redirectUrl: {
        default: PRESCRIPTIONS.path,
      },
    },
    {
      route: TESTRESULTID.path,
      redirectUrl: {
        default: TESTRESULTID.path,
      },
    },
    {
      route: TESTRESULTS.path,
      redirectUrl: {
        default: TESTRESULTS.path,
      },
    },
    {
      route: TESTRESULTSDETAIL.path,
      redirectUrl: {
        default: TESTRESULTSDETAIL.path,
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
