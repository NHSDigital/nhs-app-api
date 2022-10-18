import mapKeys from 'lodash/fp/mapKeys';
import {
  initialState,
  ADD_ERROR,
  ADD_ERROR_REPLY,
  CLEAR,
  INIT,
  LOADED,
  SENDER_MESSAGES_LOADED,
  LOADED_MESSAGE,
  LOADED_SENDERS,
  SET_HAS_UNREAD,
  SET_UNREADMESSAGE_SENDER_COUNT,
  DECREMENT_TOTAL_UNREAD_MESSAGE_COUNT,
  SET_PREVIOUS_CHOICE,
  CLEAR_ERROR_REPLY,
} from './mutation-types';

export default {
  [ADD_ERROR](state, errorDetails) {
    state.error = errorDetails;
  },
  [ADD_ERROR_REPLY](state, error) {
    state.errorReplyCount += 1;
    state.errorReply = error;
  },
  [CLEAR](state) {
    state.error = null;
    state.errorReply = null;
    state.errorReplyCount = 0;
    state.message = null;
    state.senderMessagesLoaded = false;
    state.previousChoice = null;
  },
  [CLEAR_ERROR_REPLY](state) {
    state.errorReply = null;
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
  [SET_UNREADMESSAGE_SENDER_COUNT](state, data) {
    if (data) {
      const unreadSendersCount = data.filter(sender => sender.unreadCount > 0).length;
      const unreadMessagesCount = data.reduce((total, sender) => (sender.unreadCount > 0 ? total + sender.unreadCount : total), 0);
      state.totalUnreadMessageCount = unreadMessagesCount;
      state.totalUnreadSendersCount = unreadSendersCount;
    }
  },
  [DECREMENT_TOTAL_UNREAD_MESSAGE_COUNT](state) {
    state.totalUnreadMessageCount -= 1;
  },
  [SET_PREVIOUS_CHOICE](state, choice) {
    state.previousChoice = choice || '';
  },
};
