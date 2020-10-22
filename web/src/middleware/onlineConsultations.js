import { isAnonymous } from '@/router';
import { APPOINTMENT_ADMIN_HELP_NAME, GP_ADVICE_NAME } from '@/router/names';

export default async ({ to, store, next }) => {
  const isLoggedIn = store.getters['session/isLoggedIn'];

  if (
    !isAnonymous(to)
    && isLoggedIn()
    && store.state.serviceJourneyRules.isLoaded
    && (to.name === APPOINTMENT_ADMIN_HELP_NAME || to.name === GP_ADVICE_NAME)) {
    await store.dispatch('onlineConsultations/setProviderNames', {
      adminProviderName: store.state.serviceJourneyRules.rules.cdssAdmin.provider,
      adviceProviderName: store.state.serviceJourneyRules.rules.cdssAdvice.provider,
    });
  }

  return next();
};
