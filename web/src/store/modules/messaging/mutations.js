import mapKeys from 'lodash/fp/mapKeys';
import {
  initialState,
  ADD_ERROR,
  CLEAR,
  INIT,
  LOADED,
  SENDER_MESSAGES_LOADED,
  LOADED_MESSAGE,
  LOADED_SENDERS,
  SET_HAS_UNREAD,
} from './mutation-types';

export default {
  [ADD_ERROR](state, errorDetails) {
    state.error = errorDetails;
  },
  [CLEAR](state) {
    state.error = null;
    state.message = null;
    state.senderMessagesLoaded = false;
  },
  [INIT](state) {
    const blank = initialState();
    return mapKeys((key) => {
      state[key] = blank[key];
    })(state);
  },
  [LOADED](state, data) {
    state.senderMessages = data || [];
  },
  [SENDER_MESSAGES_LOADED](state) {
    state.senderMessagesLoaded = true;
  },
  [LOADED_MESSAGE](state, message) {
    state.message = message;
  },
  [LOADED_SENDERS](state, senders) {
    state.senders = senders || [];
  },
  [SET_HAS_UNREAD](state, data) {
    if (data) {
      state.hasUnread = data.some(message => (message.unreadCount > 0));
    }
  },
};
