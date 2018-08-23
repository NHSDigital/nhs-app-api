import {
  INIT_ANALYTICS,
  CLEAR_ACTION,
  TRACK_ACTION,
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
        // eslint-disable-next-line no-underscore-dangle
        window._satellite.track('track_validation_errors');
      });
    }
  },
};
