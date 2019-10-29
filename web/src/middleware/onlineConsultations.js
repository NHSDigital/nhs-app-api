import { isAnonymous,
  APPOINTMENT_ADMIN_HELP,
  APPOINTMENT_GP_ADVICE } from '@/lib/routes';

export default async ({ route, store }) => {
  const isLoggedIn = store.getters['session/isLoggedIn'];
  if (!isAnonymous(route.name)
    && isLoggedIn()
    && store.state.serviceJourneyRules.isLoaded
    && (route.name === APPOINTMENT_ADMIN_HELP.name
        || route.name === APPOINTMENT_GP_ADVICE.name)) {
    await store.dispatch('onlineConsultations/setProviderNames',
      { adminProviderName: store.state.serviceJourneyRules.rules.cdssAdmin.provider,
        adviceProviderName: store.state.serviceJourneyRules.rules.cdssAdvice.provider,
      });
  }

  return Promise.resolve();
};
