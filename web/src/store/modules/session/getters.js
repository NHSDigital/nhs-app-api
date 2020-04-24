import moment from 'moment';
import proofLevel from '@/lib/proofLevel';

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
  isProofLevel9(state) {
    return state.proofLevel === proofLevel.P9;
  },
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
  shouldUplift(state) {
    return requiredProofLevel => requiredProofLevel === proofLevel.P9
      && state.proofLevel !== proofLevel.P9;
  },
};
