import {
  INIT_ANALYTICS,
  CLEAR_ACTION,
  TRACK_ACTION,
  TRACK_LINK,
} from './mutation-types';

export default {
  init({ commit }) {
    commit(INIT_ANALYTICS);
  },
  clearAction({ commit }) {
    commit(CLEAR_ACTION);
  },
  track({ commit }, action) {
    commit(TRACK_ACTION, action);
  },
  trackError(_, error) {
    error.messages.forEach((message) => {
      window.digitalData.error = {
        type: error.type,
        description: message,
        descriptionwithpagename: `${window.digitalData.page.pageInfo.pageName}:${message}`,
      };
      this.dispatch('analytics/satelliteTrack', 'track_validation_errors');
    });
  },
  trackLink({ commit }, navigationInfo) {
    commit(TRACK_LINK, navigationInfo);
    window.digitalData.event = {
      eventInfo: {
        navigation: navigationInfo,
      },
    };
    this.dispatch('analytics/satelliteTrack', 'click_link');
  },
  trackUserProperty(_, { key, value }) {
    if (window.digitalData && window.digitalData.user) {
      window.digitalData.user[key] = value;
    }
  },
  satelliteTrack(_, nameOfCall, objectToTrack = {}) {
    // Put track call in try-catch, as it likely called under error, so no internet connection.
    // eslint-disable-next-line no-underscore-dangle
    if (window._satellite) {
      /* eslint no-empty: ["error", { "allowEmptyCatch": true }] */
      try {
        // eslint-disable-next-line no-underscore-dangle
        window._satellite.track(nameOfCall, objectToTrack);
      } catch (error) { }
    }
  },
};
