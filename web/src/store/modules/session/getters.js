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
    const profile = { name: '', dateOfBirth: '', nhsNumber: '' };
    if (getters.isProxying) {
      profile.name = rootState.linkedAccounts.actingAsUser.name;
      profile.dateOfBirth = rootState.linkedAccounts.actingAsUser.dateOfBirth;
      profile.nhsNumber = rootState.linkedAccounts.actingAsUser.nhsNumber;
    } else {
      profile.name = state.user;
      profile.dateOfBirth = state.dateOfBirth;
      profile.nhsNumber = state.nhsNumber;
    }

    return profile;
  },
};
