import { assign, has } from 'lodash/fp';
import Routes from '../../Routes';

export default {
  default: {
    redirectUrl: '',
    showApiError: true,
    ignoredErrors: [],
  },
  pages: [
    {
      route: Routes.APPOINTMENT_CANCELLING.path,
      redirectUrl: Routes.APPOINTMENTS.path,
    },
    {
      route: Routes.APPOINTMENT_CONFIRMATIONS.path,
      redirectUrl: Routes.APPOINTMENTS.path,
      ignoredErrors: [409],
    },
    {
      route: Routes.PRESCRIPTION_CONFIRM_COURSES.path,
      redirectUrl: Routes.PRESCRIPTIONS.path,
    },
    {
      route: Routes.AUTH_RETURN.path,
      redirectUrl: Routes.LOGIN.path,
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
        break;
      }
    }

    return settings;
  },
};
