import mapKeys from 'lodash/fp/mapKeys';
import {
  initialState,
  INIT,
  LOADED,
  SET_SENDER,
  SET_HAS_UNREAD,
} from './mutation-types';

export default {
  [INIT](state) {
    const blank = initialState();
    return mapKeys((key) => {
      state[key] = blank[key];
    })(state);
  },
  [LOADED](state, data) {
    state.senderMessages = data || [];
  },
  [SET_SENDER](state, sender) {
    state.selectedSender = sender;
  },
  [SET_HAS_UNREAD](state, data) {
    if (data) {
      state.hasUnread = data.some(message => (message.unreadCount > 0));
    }
  },
};
