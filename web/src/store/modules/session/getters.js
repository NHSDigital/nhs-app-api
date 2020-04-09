import moment from 'moment';
import { P9_PROOF_LEVEL } from './mutation-types';

export default {
  currentProfile(state, getters, rootState) {
    if (getters.isProxying) {
      return {
        ageMonths: rootState.linkedAccounts.actingAsUser.ageMonths,
        ageYears: rootState.linkedAccounts.actingAsUser.ageYears,
        givenName: rootState.linkedAccounts.actingAsUser.givenName,
        fullName: rootState.linkedAccounts.actingAsUser.fullName,
        displayPersonalizedContent:
          rootState.linkedAccounts.actingAsUser.displayPersonalizedContent,
      };
    }

    return {
      name: state.user,
      dateOfBirth: state.dateOfBirth,
      nhsNumber: state.nhsNumber,
    };
  },
  isExpiring(state) {
    return (expiringWarningSeconds) => {
      const { durationSeconds, lastCalledAt } = state;
      if (!durationSeconds || !lastCalledAt || !expiringWarningSeconds) return false;
      const now = new Date();
      const expiryTime = moment(lastCalledAt).add(durationSeconds, 'seconds').toDate();
      const expiringTime = moment(lastCalledAt).add(durationSeconds - expiringWarningSeconds, 'seconds').toDate();
      return now < expiryTime && now >= expiringTime;
    };
  },
  isLoggedIn(input) {
    const state = input.session || input;
    return () => !!state.csrfToken;
  },
  isNotP9User: state => state.proofLevel !== P9_PROOF_LEVEL,
  isProxying(state, getters, rootState) {
    return !!(rootState.linkedAccounts && rootState.linkedAccounts.actingAsUser);
  },
  isValid(state) {
    return () => {
      const { durationSeconds, lastCalledAt } = state;
      if (!durationSeconds || !lastCalledAt) return false;
      const now = new Date();
      const expiryTime = moment(lastCalledAt).add(durationSeconds, 'seconds').toDate();
      return now < expiryTime;
    };
  },
};
