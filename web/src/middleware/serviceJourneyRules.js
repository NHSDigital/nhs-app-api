import { isAnonymous } from '@/lib/routes';

export default async ({ route, store }) => {
  const isLoggedIn = store.getters['session/isLoggedIn'];

  if (!isAnonymous(route.name)
      && isLoggedIn()
      && !store.state.serviceJourneyRules.isLoaded) {
    await store.dispatch('serviceJourneyRules/load');
  }

  return Promise.resolve();
};
