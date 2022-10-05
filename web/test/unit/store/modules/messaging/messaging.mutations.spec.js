import mutations from '@/store/modules/messaging/mutations';
import {
  initialState,
  ADD_ERROR,
  CLEAR,
  INIT,
  LOADED,
  LOADED_MESSAGE,
  LOADED_SENDERS,
  DECREMENT_TOTAL_UNREAD_MESSAGE_COUNT,
} from '@/store/modules/messaging/mutation-types';

describe('messaging mutations', () => {
  let state;

  beforeEach(() => {
    state = initialState();
  });

  describe('ADD_ERROR', () => {
    const errorDetails = 'errorDetails';

    beforeEach(() => {
      state.error = undefined;
      mutations[ADD_ERROR](state, errorDetails);
    });

    it('will set the error state to the received value', () => {
      expect(state.error).toBe(errorDetails);
    });
  });

  describe('CLEAR', () => {
    beforeEach(() => {
      state.error = 'errorDetails';
      state.message = 'message';
      mutations[CLEAR](state);
    });

    it('will set the error state to null', () => {
      expect(state.error).toBeNull();
    });

    it('will set the message state to null', () => {
      expect(state.message).toBeNull();
    });
  });

  describe('INIT', () => {
    beforeEach(() => {
      state.hasLoaded = true;
      state.unreadMessages = ['test'];
      mutations[INIT](state);
    });

    it('will reset the current state to the default state', () => {
      expect(state).toEqual(initialState());
    });
  });

  describe('LOADED', () => {
    describe('with data', () => {
      const data = 'test state';

      beforeEach(() => {
        mutations[LOADED](state, data);
      });

      it('will set the sender messages state to the received value', () => {
        expect(state.senderMessages).toBe(data);
      });
    });

    describe('no data', () => {
      beforeEach(() => {
        state.senderMessages = ['senderMessage'];
        mutations[LOADED](state, null);
      });

      it('will set the sender messages state to empty collection', () => {
        expect(state.senderMessages).toEqual([]);
      });
    });
  });

  describe('LOADED_MESSAGE', () => {
    const data = 'test state';

    beforeEach(() => {
      mutations[LOADED_MESSAGE](state, data);
    });

    it('will set the message state to the received value', () => {
      expect(state.message).toEqual(data);
    });
  });

  describe('DECREMENT_UNREAD_MESSAGE_COUNT', () => {
    it('will decrement the total unread message count in the state', () => {
      mutations[DECREMENT_TOTAL_UNREAD_MESSAGE_COUNT](state);
      expect(state.totalUnreadMessageCount).toBe(-1);
    });
  });

  describe('LOADED_SENDERS', () => {
    const data = 'test state';

    beforeEach(() => {
      mutations[LOADED_SENDERS](state, data);
    });

    it('will set the senders state to the received value', () => {
      expect(state.senders).toEqual(data);
    });

    describe('no senders', () => {
      beforeEach(() => {
        state.senders = ['senderOne'];
        mutations[LOADED_SENDERS](state, undefined);
      });

      it('will set the senders state to empty array', () => {
        expect(state.senders).toEqual([]);
      });
    });
  });
});
