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
  trackError(ctx, error) {
    if (process.client) {
      error.messages.forEach((message) => {
        window.digitalData.error = {
          type: error.type,
          description: message,
          descriptionwithpagename: `${window.digitalData.page.pageInfo.pageName}:${message}`,
        };
        this.dispatch('analytics/satelliteTrack', 'track_validation_errors');
      });
    }
  },
  trackLink({ commit }, navigationInfo) {
    commit(TRACK_LINK, navigationInfo);
    if (process.client) {
      window.digitalData.event = {
        eventInfo: {
          navigation: navigationInfo,
        },
      };
      this.dispatch('analytics/satelliteTrack', 'click_link');
    }
  },
  satelliteTrack(nameOfCall) {
    // Put track call in try-catch, as it likely called under error, so no internet connection.
    if (process.client) {
      /* eslint no-empty: ["error", { "allowEmptyCatch": true }] */
      // eslint-disable-next-line no-underscore-dangle
      try { window._satellite.track(nameOfCall); } catch (exception) { }
    }
  },
};
