import mutations from '@/store/modules/messaging/mutations';
import {
  initialState,
  INIT,
  LOADED,
  SET_SENDER,
} from '@/store/modules/messaging/mutation-types';

describe('messaging mutations', () => {
  let state;

  beforeEach(() => {
    state = initialState();
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
    const data = 'test state';

    beforeEach(() => {
      mutations[LOADED](state, data);
    });

    it('will set the sender messages state to the received value', () => {
      expect(state.senderMessages).toEqual(data);
    });
  });

  describe('SET_SENDER', () => {
    const data = 'test state';

    beforeEach(() => {
      mutations[SET_SENDER](state, data);
    });

    it('will set the selected sender state to the received value', () => {
      expect(state.selectedSender).toEqual(data);
    });
  });
});
