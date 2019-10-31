import mutations from '@/store/modules/notifications/mutations';
import {
  initialState,
  SET_REGISTRATION,
  SET_WAITING,
} from '@/store/modules/notifications/mutation-types';

describe('notifications mutations', () => {
  let state;

  beforeEach(() => {
    state = initialState();
  });

  describe('SET_REGISTRATION', () => {
    beforeEach(() => {
      mutations[SET_REGISTRATION](state, true);
    });

    it('will set the notifications registration state to the received value', () => {
      expect(state.registered).toEqual(true);
    });
  });

  describe('SET_WAITING', () => {
    beforeEach(() => {
      mutations[SET_WAITING](state, true);
    });

    it('will set the notifications waiting state to the received value', () => {
      expect(state.isWaiting).toEqual(true);
    });
  });
});
