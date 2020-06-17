import { REDIRECT_PARAMETER } from '@/router/names';
import AuthorisationService from '@/services/authorisation-service';

export default (context) => {
  const { from, store, next } = context;

  if (!store.state.device.isNativeApp || store.$cookies.get('SkipPreRegistrationPage') === 'true') {
    const authorisationService = new AuthorisationService(store.$env);
    const { loginUrl } = authorisationService.generateLoginUrl({
      isNativeApp: store.state.device.isNativeApp,
      redirectTo: from.query[REDIRECT_PARAMETER],
      cookies: store.$cookies,
    });

    window.location.href = loginUrl;
  } else {
    next();
  }
};
