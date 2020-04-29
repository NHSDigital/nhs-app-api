import {
  PRE_REGISTRATION_INFORMATION,
  REDIRECT_PARAMETER,
} from '@/lib/routes';
import AuthorisationService from '@/services/authorisation-service';
import setSource from './setSource';

export default ({ app, redirect, route, store }) => {
  setSource(app.context);

  if (!(route.name === PRE_REGISTRATION_INFORMATION.name
    && (!store.state.device.isNativeApp
      || store.$cookies.get('SkipPreRegistrationPage') === true))) {
    return;
  }

  const authorisationService = new AuthorisationService(app.$env);
  const { loginUrl } = authorisationService.generateLoginUrl({
    isNativeApp: store.state.device.isNativeApp,
    redirectTo: route.query[REDIRECT_PARAMETER],
    cookies: store.$cookies,
  });
  redirect(loginUrl);
};
