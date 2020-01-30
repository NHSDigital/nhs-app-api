import AuthorisationService from '@/services/authorisation-service';
import get from 'lodash/fp/get';
import { BEGINLOGIN, LOGIN, INDEX, INTERSTITIAL_REDIRECTOR, REDIRECT_PARAMETER, isAnonymous } from '@/lib/routes';

export default function ({ app, store, redirect, route }) {
  const isLoggedIn = store.getters['session/isLoggedIn']();
  if (!isAnonymous(route.name) && !isLoggedIn) {
    const redirectParam = get(REDIRECT_PARAMETER)(route.query);
    if (route.path === INDEX.path || !route.name ||
      (route.path === INTERSTITIAL_REDIRECTOR.path && !redirectParam)) {
      return redirect(LOGIN.path);
    }
    if (route.path === INTERSTITIAL_REDIRECTOR.path && redirectParam) {
      return redirect(`${LOGIN.path}?${REDIRECT_PARAMETER}=${redirectParam}`);
    }

    return redirect(`${LOGIN.path}?${REDIRECT_PARAMETER}=${route.name}`);
  }

  if (route.name === LOGIN.name && isLoggedIn) {
    return redirect(INDEX.path);
  }

  if (route.name === BEGINLOGIN.name) {
    const authorisationService = new AuthorisationService(app.$env);
    const { loginUrl } = authorisationService.generateLoginUrl({
      isNativeApp: store.state.device.isNativeApp,
      redirectTo: route.query[REDIRECT_PARAMETER],
      cookies: store.$cookies,
    });

    return redirect(loginUrl);
  }

  return undefined;
}
