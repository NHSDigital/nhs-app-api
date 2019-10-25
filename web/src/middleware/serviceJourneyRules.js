import { isAnonymous } from '@/lib/routes';

export default async ({ route, store }) => {
  const isLoggedIn = store.getters['session/isLoggedIn'];

  if (!isAnonymous(route.name) && isLoggedIn()) {
    if (!store.state.serviceJourneyRules.isLoaded) {
      await store.dispatch('serviceJourneyRules/load');
    }

    if (!store.state.linkedAccounts.config.hasLoaded) {
      await store.dispatch('linkedAccounts/initialiseConfig');
    }
  }

  return Promise.resolve();
};
