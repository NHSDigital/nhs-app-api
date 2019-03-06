/* eslint-disable import/extensions */
/* eslint-disable import/extensions */
import { assign, has } from 'lodash/fp';
import {
  APPOINTMENTS,
  APPOINTMENT_BOOKING,
  APPOINTMENT_CANCELLING,
  APPOINTMENT_CONFIRMATIONS,
  AUTH_RETURN,
  LOGIN,
  BEGINLOGIN,
  MYRECORD,
  MYRECORDTESTRESULT,
  ORGAN_DONATION,
  ORGAN_DONATION_REVIEW_YOUR_DECISION,
  PRESCRIPTIONS,
  PRESCRIPTION_CONFIRM_COURSES,
  PRESCRIPTION_REPEAT_COURSES,
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
      route: MYRECORDTESTRESULT.path,
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
        1: 'organDonation/submitRegistration',
      },
    },
    {
      route: PRESCRIPTIONS.path,
      errorOverrideStyles: { 403: 'plain' },
    },
    {
      route: PRESCRIPTION_REPEAT_COURSES.path,
      errorOverrideStyles: { 403: 'plain' },
    },
    {
      route: PRESCRIPTION_CONFIRM_COURSES.path,
      errorOverrideStyles: { 403: 'plain' },
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
