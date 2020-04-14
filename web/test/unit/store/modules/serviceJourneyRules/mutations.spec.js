import { mutationNames } from '@/store/modules/serviceJourneyRules/constants';
import mutations from '@/store/modules/serviceJourneyRules/mutations';
import { initialState } from '@/store/modules/serviceJourneyRules/mutation-types';

const {
  INIT,
  SET_RULES,
} = mutationNames;

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
    const rules = {
      journeys: {
        foo: 'test',
      },
    };

    beforeEach(() => {
      mutations[SET_RULES](state, rules);
    });

    it('will set the `rules` value to the received value', () => {
      expect(state.rules).toBe(rules.journeys);
    });

    it('will set the `isloaded` value to true', () => {
      expect(state.isLoaded).toBe(true);
    });
  });
});
