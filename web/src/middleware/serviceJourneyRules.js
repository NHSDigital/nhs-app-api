import { isAnonymous } from '@/lib/routes';

export default async ({ route, store }) => {
  const isLoggedIn = store.getters['session/isLoggedIn'];

  if (!isAnonymous(route.name) && isLoggedIn()) {
    if (!store.state.serviceJourneyRules.isLoaded) {
      if (store.state.linkedAccounts && store.state.linkedAccounts.actingAsUser) {
        await store.dispatch('serviceJourneyRules/loadLinkedAccount');
      } else {
        await store.dispatch('serviceJourneyRules/load');
      }
    }

    if (!store.state.linkedAccounts.config.hasLoaded) {
      await store.dispatch('linkedAccounts/initialiseConfig');

      if (store.state.linkedAccounts.config.hasLinkedAccounts) {
        // want to run this on full page refresh to make sure TPP main user suid is set
        await store.dispatch('linkedAccounts/switchToMainUserProfile');
      }
    }
  }

  return Promise.resolve();
};
