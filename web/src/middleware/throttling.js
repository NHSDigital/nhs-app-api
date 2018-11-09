import {
  LOGIN,
  GP_FINDER,
  INDEX,
} from '@/lib/routes';

export default function ({ redirect, store, route, env }) {
  if (!env.THROTTLING_ENABLED) {
    return route.path.startsWith(GP_FINDER.path) ? redirect(INDEX.path) : undefined;
  }

  const betaCookie = store.$cookies.get('BetaCookie');

  if (!betaCookie || !betaCookie.ODSCode) {
    if (route.name === LOGIN.name) {
      return redirect(GP_FINDER.path);
    }
    if (!betaCookie) {
      store.$cookies.set('BetaCookie', { ODSCode: '', ParticipatingPractice: false });
    }
  }
  return undefined;
}
