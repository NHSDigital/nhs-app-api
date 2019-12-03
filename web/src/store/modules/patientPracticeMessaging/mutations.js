import mapKeys from 'lodash/fp/mapKeys';
import {
  INIT,
  LOADED,
  CLEAR,
  SET_SELECTED_MESSAGE_ID,
  LOADED_MESSAGE,
  SET_DETAILS,
  SET_SUMMARIES,
  SET_SELECTED_MESSAGE_RECIPIENT,
  initialState,
} from './mutation-types';

const clearMessage = (state) => {
  state.selectedMessageDetails = undefined;
  state.selectedMessageId = undefined;
  state.selectedMessageRecipient = undefined;
};

export default {
  [INIT](state) {
    const blank = initialState();
    return mapKeys((key) => {
      state[key] = blank[key];
    })(state);
  },
  [SET_SUMMARIES](state, data) {
    state.messageSummaries = data || [];
  },
  [LOADED](state, loaded) {
    state.loaded = !!loaded;
  },
  [LOADED_MESSAGE](state, loaded) {
    state.loadedDetails = !!loaded;
  },
  [SET_DETAILS](state, data) {
    state.selectedMessageDetails = data || undefined;
  },
  [CLEAR](state) {
    clearMessage(state);
  },
  [SET_SELECTED_MESSAGE_ID](state, id) {
    state.selectedMessageId = id;
  },
  [SET_SELECTED_MESSAGE_RECIPIENT](state, recipient) {
    state.selectedMessageRecipient = recipient;
  },
};
