import { APPOINTMENTS_NAME, TERMSANDCONDITIONS_NAME } from '@/router/names';
import { LOGOUT_PATH, EMPTY_PATH, executeHomeNavigationRule } from '@/router/paths';

describe('executeHomeNavigationRule', () => {
  it('terms and condition header link should resolve to logout', () => {
    expect(executeHomeNavigationRule(TERMSANDCONDITIONS_NAME)).toBe(LOGOUT_PATH);
  });

  it('anything route\'s  header link should resolve to home', () => {
    expect(executeHomeNavigationRule(APPOINTMENTS_NAME)).toBe(EMPTY_PATH);
  });
});
