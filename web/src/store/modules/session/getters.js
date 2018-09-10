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
      const result = now < expiryTime;
      return result;
    };
  },
};
