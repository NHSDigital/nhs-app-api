import mutations from '@/store/modules/messaging/mutations';
import {
  initialState,
  INIT,
  LOADED,
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

    it('will set the unread messages state to the received value', () => {
      expect(state.unreadMessages).toEqual(data);
    });
  });
});
