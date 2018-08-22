import {
  INIT_ANALYTICS,
  CLEAR_ACTION,
  TRACK_ACTION,
  TRACK_ERROR,
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
  trackError({ commit }, error) {
    commit(TRACK_ERROR, error);

    if (process.client) {
      error.messages.forEach((message) => {
        window.digitalData.error.type = error.type;
        window.digitalData.error.description = message;
        window.digitalData.error.descriptionwithpagename = `${window.digitalData.page.pageInfo.pageName}:${message}`;
        // eslint-disable-next-line no-underscore-dangle
        window._satellite.track('track_validation_errors');
      });
    }
  },
};
