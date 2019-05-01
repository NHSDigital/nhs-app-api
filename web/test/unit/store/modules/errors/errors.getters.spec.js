import each from 'jest-each';
import getters from '@/store/modules/errors/getters';
import { initialState } from '@/store/modules/errors/mutation-types';

const { isStandardError, showApiError } = getters;

describe('errors getters', () => {
  let state;

  const mergeState = toMerge => ({ ...state, ...toMerge });

  beforeEach(() => {
    state = initialState();
  });

  describe('isStandardError', () => {
    it('will be false when there are no api errors in the state', () => {
      state = mergeState({ apiErrors: [] });
      expect(isStandardError(state)).toBe(false);
    });

    it('will be false when there is no status in the api error', () => {
      state = mergeState({ apiErrors: [{}] });
      expect(isStandardError(state)).toBe(false);
    });

    it('will be false when the status is less than 500', () => {
      state = mergeState({ apiErrors: [{ status: 404 }] });
      expect(isStandardError(state)).toBe(false);
    });

    it('will be true when the status is greater than 500', () => {
      state = mergeState({ apiErrors: [{ status: 600 }] });
      expect(isStandardError(state)).toBe(true);
    });

    it('will be true when the status is equal to 500', () => {
      state = mergeState({ apiErrors: [{ status: 500 }] });
      expect(isStandardError(state)).toBe(true);
    });
  });

  describe('showApiError', () => {
    it('will be false when the state contains "showApiError" as false', () => {
      state = mergeState({ showApiError: false });
      expect(showApiError(state)).toBe(false);
    });

    it('will be false when the page settings contains "showApiError" as false', () => {
      state = mergeState({ showApiError: true, pageSettings: { showApiError: false } });
      expect(showApiError(state)).toBe(false);
    });

    it('will be false when there are no api errors', () => {
      state = mergeState({ apiErrors: [] });
      expect(showApiError(state)).toBe(false);
    });

    it('will be false when there are connection problems', () => {
      state = mergeState({ apiErrors: [{}], hasConnectionProblem: true });
      expect(showApiError(state)).toBe(false);
    });

    it('will be false when the status is an ignored one', () => {
      state = mergeState({ apiErrors: [{ status: 401 }], showApiError: true });
      expect(showApiError(state)).toBe(false);
    });

    it('will be true when there is a server error', () => {
      state = mergeState({ apiErrors: [{ status: 500 }], showApiError: true });
      expect(showApiError(state)).toBe(true);
    });

    it('will be false when the status code has been ignored', () => {
      state = mergeState({
        apiErrors: [{ status: 465 }],
        showApiError: true,
        pageSettings: {
          ignoredErrors: [465],
        },
      });

      expect(showApiError(state)).toBe(false);
    });

    it('will be false when the status code unknown', () => {
      state = mergeState({ apiErrors: [{ status: 256 }], showApiError: true });
      expect(showApiError(state)).toBe(false);
    });

    each([
      464,
      465,
      400,
      403,
      409,
      460,
      461,
      466,
    ]).it(' will be a standard error for status code: %s', (code) => {
      state = mergeState({ apiErrors: [{ status: code }], showApiError: true });
      expect(showApiError(state)).toBe(true);
    });
  });
});
