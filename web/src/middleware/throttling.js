import {
  LOGIN,
  GP_FINDER,
  GP_FINDER_PARTICIPATION,
  INDEX,
  GP_FINDER_SENDING_EMAIL,
} from '@/lib/routes';

import moment from 'moment';
import { GP_FINDER_WAITING_LIST_JOINED } from '../lib/routes';

function hasCompletedThrottling(betaCookie) {
  if (betaCookie) {
    return (betaCookie.ODSCode && betaCookie.PracticeParticipating !== undefined &&
        betaCookie.PracticeName && betaCookie.PracticeAddress && betaCookie.Complete) ||
      betaCookie.Skipped;
  }
  return false;
}

export default function ({
  redirect,
  store,
  route,
  env,
  app,
}) {
  const isLoggedIn = store.getters['session/isLoggedIn']();

  if (isLoggedIn) {
    return route.path.startsWith(GP_FINDER.path) ? redirect(INDEX.path) : undefined;
  }

  let betaCookie = app.$cookies.get('BetaCookie');

  if (route.name === GP_FINDER.name) {
    if (route.query.reset) {
      betaCookie = {};
      app.$cookies.set('BetaCookie', betaCookie, {
        path: '/',
        maxAge: -1,
      });
    } else if (route.query.skip) {
      betaCookie = {
        Skipped: true,
      };
      app.$cookies.set('BetaCookie', betaCookie, {
        path: '/',
        maxAge: moment.duration(1, 'y').asSeconds(),
      });
    }
  }

  if ((env.THROTTLING_ENABLED !== true && env.THROTTLING_ENABLED !== 'true') || hasCompletedThrottling(betaCookie)) {
    if (hasCompletedThrottling(betaCookie) && route.name === GP_FINDER_PARTICIPATION.name) {
      return undefined;
    }

    return route.path.startsWith(GP_FINDER.path) &&
           !route.path.startsWith(GP_FINDER_SENDING_EMAIL.path) &&
           !route.path.startsWith(GP_FINDER_WAITING_LIST_JOINED.path) ?
      redirect(LOGIN.path) :
      undefined;
  }

  if (route.name === LOGIN.name) {
    return redirect(GP_FINDER.path);
  }

  return undefined;
}
