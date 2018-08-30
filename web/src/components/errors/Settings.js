/* eslint-disable import/extensions */
/* eslint-disable import/extensions */
import { assign, has } from 'lodash/fp';
import Routes from '@/Routes';

export default {
  default: {
    redirectUrl: null,
    showApiError: true,
    ignoredErrors: [],
    errorOverrideStyles: {},
  },
  pages: [
    {
      route: Routes.APPOINTMENTS.path,
      errorOverrideStyles: { 403: 'plain' },
    },
    {
      route: Routes.APPOINTMENT_CANCELLING.path,
      redirectUrl: {
        default: Routes.APPOINTMENTS.path,
      },
    },
    {
      route: Routes.APPOINTMENT_BOOKING.path,
      errorOverrideStyles: { 403: 'plain' },
    },
    {
      route: Routes.APPOINTMENT_CONFIRMATIONS.path,
      redirectUrl: {
        409: Routes.APPOINTMENT_BOOKING.path,
        default: Routes.APPOINTMENTS.path,
      },
    },
    {
      route: Routes.PRESCRIPTION_CONFIRM_COURSES.path,
      redirectUrl: {
        default: Routes.PRESCRIPTIONS.path,
      },
    },
    {
      route: Routes.AUTH_RETURN.path,
      redirectUrl: {
        default: Routes.LOGIN.path,
      },
    },
    {
      route: Routes.MYRECORDTESTRESULT.path,
      redirectUrl: {
        default: Routes.MYRECORD.path,
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
