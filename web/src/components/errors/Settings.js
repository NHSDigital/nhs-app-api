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
  MYRECORD,
  MYRECORDTESTRESULT,
  PRESCRIPTIONS,
  PRESCRIPTION_CONFIRM_COURSES,
} from '@/lib/routes';

export default {
  default: {
    redirectUrl: null,
    showApiError: true,
    ignoredErrors: [],
    errorOverrideStyles: {},
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
    },
    {
      route: APPOINTMENT_CONFIRMATIONS.path,
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
      route: MYRECORDTESTRESULT.path,
      redirectUrl: {
        default: MYRECORD.path,
      },
    },
  ],
  forPage(routePath) {
    let settings = assign({}, this.default);
    for (let i = 0, max = this.pages.length; i < max; i += 1) {
      if (this.pages[i].route === routePath) {
        settings = assign({}, this.pages[i]);
        if (!has('redirectUrl', settings)) settings.redirectUrl = this.default.redirectUrl;
        if (!has('showApiError', settings)) settings.showApiError = this.default.showApiError;
        if (!has('ignoredErrors', settings)) settings.ignoredErrors = this.default.ignoredErrors;
        if (!has('errorOverrideStyles', settings)) settings.errorOverrideStyles = this.default.errorOverrideStyles;
        break;
      }
    }

    return settings;
  },
};
