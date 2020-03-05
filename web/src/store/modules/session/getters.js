import moment from 'moment';

export default {
  isLoggedIn(input) {
    const state = input.session || input;
    return () => !!state.csrfToken;
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
  isProxying(state, getters, rootState) {
    return !!(rootState.linkedAccounts && rootState.linkedAccounts.actingAsUser);
  },
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
};
