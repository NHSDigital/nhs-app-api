/* eslint-disable no-param-reassign */
/* eslint-disable import/extensions */
import { isAnonymous } from '@/lib/routes';

export default function ({ store, redirect, route }) {
  if (!isAnonymous(route.name) && !store.getters['session/isLoggedIn']()) {
    redirect('/login');
  }
}
