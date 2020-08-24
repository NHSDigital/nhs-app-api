import { SET_ACCEPTANCE } from '@/store/modules/termsAndConditions/mutation-types';
import mutations from '@/store/modules/termsAndConditions/mutations';
import { mockCookies } from '../../../helpers';

describe('termsAndConditions/mutations', () => {
  beforeEach(() => {
    mutations.$env = {
      SECURE_COOKIES: true,
    };
    mutations.$cookies = mockCookies();
  });

  it('will set `areAccepted` to true when SET_ACCEPTANCE is committed with true', () => {
    const state = { areAccepted: false, analyticsCookieAccepted: '' };
    mutations[SET_ACCEPTANCE](state, { areAccepted: true, analyticsCookieAccepted: true });
    expect(state.areAccepted).toEqual(true);
    expect(mutations.$cookies.set).toHaveBeenCalledWith(
      'nhso.terms',
      { analyticsCookieAccepted: true, areAccepted: true },
      null,
      '/',
      null,
      true,
      'Lax',
    );
    expect(state.analyticsCookieAccepted).toEqual(true);
  });

  it('will set `areAccepted` to false when SET_ACCEPTANCE is committed with false', () => {
    const state = { areAccepted: true, analyticsCookieAccepted: '' };
    mutations[SET_ACCEPTANCE](state, { areAccepted: false, analyticsCookieAccepted: false });
    expect(state.areAccepted).toEqual(false);
    expect(mutations.$cookies.set).toHaveBeenCalledWith(
      'nhso.terms',
      { analyticsCookieAccepted: false, areAccepted: false },
      null,
      '/',
      null,
      true,
      'Lax',
    );
    expect(state.analyticsCookieAccepted).toEqual(false);
  });
});
