/* eslint-disable import/extensions */
/* eslint-disable import/extensions */
import { assign, has } from 'lodash/fp';
import {
  ALLERGIESANDREACTIONS,
  APPOINTMENTS,
  APPOINTMENT_BOOKING,
  APPOINTMENT_CANCELLING,
  APPOINTMENT_CONFIRMATIONS,
  AUTH_RETURN,
  LOGIN,
  BEGINLOGIN,
  MYRECORD,
  MY_RECORD_DOCUMENT,
  MY_RECORD_DOCUMENTS,
  MYRECORDTESTRESULT,
  GP_MEDICAL_RECORD,
  ORGAN_DONATION,
  ORGAN_DONATION_REVIEW_YOUR_DECISION,
  PRESCRIPTIONS,
  PRESCRIPTION_CONFIRM_COURSES,
  PRESCRIPTION_REPEAT_COURSES,
  NOMINATED_PHARMACY_CONFIRM,
  NOMINATED_PHARMACY_SEARCH,
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
      route: APPOINTMENTS.path,
      errorOverrideStyles: { 403: 'plain' },
      redirectUrl: {
        default: APPOINTMENTS.path,
      },
    },
    {
      route: APPOINTMENT_CANCELLING.path,
      redirectUrl: {
        default: APPOINTMENTS.path,
      },
    },
    {
      route: APPOINTMENT_BOOKING.path,
      errorOverrideStyles: { 403: 'plain' },
      redirectUrl: {
        504: APPOINTMENT_BOOKING.path,
        default: APPOINTMENTS.path,
      },
    },
    {
      route: APPOINTMENT_CONFIRMATIONS.path,
      errorOverrideStyles: { 460: 'plain' },
      redirectUrl: {
        409: APPOINTMENT_BOOKING.path,
        default: APPOINTMENTS.path,
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
      route: MYRECORD.path,
      redirectUrl: {
        default: MYRECORD.path,
      },
    },
    {
      route: GP_MEDICAL_RECORD.path,
      redirectUrl: {
        default: GP_MEDICAL_RECORD.path,
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
    {
      route: ALLERGIESANDREACTIONS.path,
      redirectUrl: {
        default: ALLERGIESANDREACTIONS.path,
      },
    },
    {
      route: MYRECORDTESTRESULT.path,
      redirectUrl: {
        default: MYRECORD.path,
      },
    },
    {
      route: MY_RECORD_DOCUMENT.path,
      redirectUrl: {
        default: MYRECORD.path,
      },
    },
    {
      route: MY_RECORD_DOCUMENTS.path,
      redirectUrl: {
        default: MYRECORD.path,
      },
    },
    {
      route: ORGAN_DONATION.path,
      redirectUrl: {
        1: ORGAN_DONATION.path,
      },
      additionalInfoComponent: 'ContactOrganDonation',
    },
    {
      route: ORGAN_DONATION_REVIEW_YOUR_DECISION.path,
      additionalInfoComponent: 'ContactOrganDonation',
      action: {
        1: 'organDonation/submitDecision',
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
      route: PRESCRIPTION_REPEAT_COURSES.path,
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
