import { isAnonymous } from '@/router';
import find from 'lodash/fp/find';

const getSession = async (store) => {
  if (!store.state.session.hasLoaded) {
    await store.dispatch('session/getSession');
  }
};

const getServiceJourneyRules = async (store) => {
  if (!store.state.serviceJourneyRules.isLoaded) {
    let sjrCall = 'serviceJourneyRules/load';
    if (store.state.linkedAccounts && store.state.linkedAccounts.actingAsUser) {
      sjrCall = 'serviceJourneyRules/loadLinkedAccount';
    }
    await store.dispatch(sjrCall);
  }
};

const getLinkedAccounts = async (store, to) => {
  if (store.state.session.hasGpSession && !store.state.linkedAccounts.config.hasLoaded) {
    await store.dispatch('linkedAccounts/initialiseConfig');
    if (store.state.linkedAccounts.config.hasLinkedAccounts) {
      const patientIdInUrl = to.params.patientId;
      const mainUserPatientId = store.getters['linkedAccounts/mainPatientId'];
      if (!patientIdInUrl || patientIdInUrl === mainUserPatientId) {
        await store.dispatch('linkedAccounts/switchToMainUserProfile');
      } else {
        const linkedAccount =
          find(item => item.id === patientIdInUrl)(store.state.linkedAccounts.items);
        if (linkedAccount) {
          await store.dispatch('linkedAccounts/select', linkedAccount);
          await store.dispatch(
            'linkedAccounts/loadAccountAccessSummary',
            linkedAccount.id,
          );
          await store.dispatch('linkedAccounts/switchProfile', linkedAccount);
        }
      }
    }
  }
};

export default async (context) => {
  const { to, store, next } = context;
  const isLoggedIn = store.getters['session/isLoggedIn'];

  if (!isAnonymous(to) && isLoggedIn()) {
    await getSession(store);
    await getServiceJourneyRules(store);
    await getLinkedAccounts(store, to);
  }

  return next();
};
