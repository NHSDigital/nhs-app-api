import each from 'jest-each';
import first from 'lodash/fp/first';
import mutations from '@/store/modules/errors/mutations';
import { initialState, ADD_API_ERROR } from '@/store/modules/errors/mutation-types';

describe('errors mutations', () => {
  let state;

  beforeEach(() => {
    state = initialState();
  });

  describe('ADD_API_ERROR', () => {
    describe('error response', () => {
      each([
        400,
        403,
        409,
        460,
        461,
        464,
        465,
        466,
        500,
      ]).it('will add an api error for status code %s', (status) => {
        mutations[ADD_API_ERROR](state, { response: { status } });
        expect(first(state.apiErrors)).toEqual({
          status,
          serviceDeskReference: '',
        });
      });

      it('will add an api error for status code greater than 500', () => {
        mutations[ADD_API_ERROR](state, { response: { status: 503 } });
        expect(first(state.apiErrors)).toEqual({
          status: 503,
          serviceDeskReference: '',
        });
      });

      it('will not add an api error for not handled status code', () => {
        mutations[ADD_API_ERROR](state, { response: { status: 404 } });
        expect(state.apiErrors).toHaveLength(0);
      });

      describe('data', () => {
        it('will set `serviceDeskReference` when provided', () => {
          const serviceDeskReference = 'test reference';
          mutations[ADD_API_ERROR](state, {
            response: {
              status: 500,
              data: { serviceDeskReference },
            },
          });
          expect(first(state.apiErrors)).toEqual({
            status: 500,
            serviceDeskReference,
          });
        });

        it('will set `error` when provided', () => {
          const error = 'test error code';
          mutations[ADD_API_ERROR](state, {
            response: {
              status: 500,
              data: { errorCode: error },
            },
          });
          expect(first(state.apiErrors)).toEqual({
            status: 500,
            serviceDeskReference: '',
            error,
          });
        });
      });
    });

    describe('error no response', () => {
      beforeEach(() => {
        mutations[ADD_API_ERROR](state, { response: null });
      });

      it('will add an api error with status code 500', () => {
        expect(first(state.apiErrors)).toEqual({
          status: 500,
          serviceDeskReference: '',
        });
      });

      it('will set `hasConnectionProblem` state to true', () => {
        expect(state.hasConnectionProblem).toBe(true);
      });
    });

    describe('error message', () => {
      const message = 'test message';

      beforeEach(() => {
        mutations[ADD_API_ERROR](state, { message });
      });

      it('will add message to the api error', () => {
        expect(first(state.apiErrors)).toEqual({
          status: 500,
          serviceDeskReference: '',
          message,
        });
      });
    });
  });
});
