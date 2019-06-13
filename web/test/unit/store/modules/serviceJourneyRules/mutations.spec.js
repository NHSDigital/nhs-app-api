import mutations from '@/store/modules/serviceJourneyRules/mutations';
import { initialState, INIT, SET_RULES } from '@/store/modules/serviceJourneyRules/mutation-types';

describe('service journey rules mutations', () => {
  let state;

  beforeEach(() => {
    state = initialState();
  });

  describe('INIT', () => {
    beforeEach(() => {
      state.isLoaded = true;
      state.rules = {
        foo: 'test',
      };

      mutations[INIT](state);
    });

    it('will reset the current state to the default state', () => {
      expect(state).toEqual(initialState());
    });
  });

  describe('SET_RULES', () => {
    const rules = { foo: 'test' };

    beforeEach(() => {
      mutations[SET_RULES](state, rules);
    });

    it('will set the `rules` value to the received value', () => {
      expect(state.rules).toBe(rules);
    });

    it('will set the `isloaded` value to true', () => {
      expect(state.isLoaded).toBe(true);
    });
  });
});
