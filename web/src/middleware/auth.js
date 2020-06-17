import AuthorisationService from '@/services/authorisation-service';
import get from 'lodash/fp/get';
import {
  BEGINLOGIN_NAME,
  REDIRECT_PARAMETER,
  INDEX_NAME,
  INTERSTITIAL_REDIRECTOR_NAME,
  LOGIN_NAME,
} from '@/router/names';
import { EMPTY_PATH, INTERSTITIAL_REDIRECTOR_PATH } from '@/router/paths';
import { isAnonymous } from '@/router';
import { createRouteByNameObject, createRoutePathObject, checkIfPathShouldHavePatientPrefix } from '@/lib/utils';

export default ({ store, to, next }) => {
  const isLoggedIn = store.getters['session/isLoggedIn']();
  const santizedPath = checkIfPathShouldHavePatientPrefix({ path: to.path, store });
  if (!isAnonymous(to) && !isLoggedIn) {
    if (to.path === EMPTY_PATH || !santizedPath) {
      return next({ name: LOGIN_NAME });
    }

    let queryParam = to.name;
    let navigateToRedirector = to.name === INTERSTITIAL_REDIRECTOR_NAME;
    if (!to.name) {
      queryParam = santizedPath;
      navigateToRedirector = santizedPath === INTERSTITIAL_REDIRECTOR_PATH;
    }

    const query = { [REDIRECT_PARAMETER]: queryParam };
    if (navigateToRedirector) {
      const redirectParam = get(REDIRECT_PARAMETER)(to.query);
      if (redirectParam) {
        query[REDIRECT_PARAMETER] = redirectParam;
      } else {
        return next({ name: LOGIN_NAME });
      }
    }
    return next({ name: LOGIN_NAME, query });
  }

  if (isLoggedIn) {
    if (to.name === LOGIN_NAME || (to.name !== INDEX_NAME && to.path === EMPTY_PATH)) {
      return next(createRouteByNameObject({
        name: INDEX_NAME,
        query: to.query,
        params: to.params,
        store,
      }));
    }

    if (to.matched.length === 0 && santizedPath) {
      return next(createRoutePathObject({
        path: to.path,
        query: to.query,
        params: to.params,
        store,
      }));
    }
  }

  if (to.name === BEGINLOGIN_NAME) {
    const authorisationService = new AuthorisationService(store.$env);
    const { loginUrl } = authorisationService.generateLoginUrl({
      isNativeApp: store.state.device.isNativeApp,
      redirectTo: to.query[REDIRECT_PARAMETER],
      cookies: store.$cookies,
    });

    window.location.href = loginUrl;
    return next(false);
  }
  return next();
};
