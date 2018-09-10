import { SET_ACCEPTANCE } from '@/store/modules/termsAndConditions/mutation-types';
import mutations from '@/store/modules/termsAndConditions/mutations';

describe('termsAndConditions/mutations', () => {
  it('will set `areAccepted` to true when SET_ACCEPTANCE is committed with true', () => {
    const state = { areAccepted: false };
    mutations[SET_ACCEPTANCE](state, true);
    expect(state.areAccepted).toEqual(true);
  });

  it('will set `areAccepted` to false when SET_ACCEPTANCE is committed with false', () => {
    const state = { areAccepted: true };
    mutations[SET_ACCEPTANCE](state, false);
    expect(state.areAccepted).toEqual(false);
  });
});
