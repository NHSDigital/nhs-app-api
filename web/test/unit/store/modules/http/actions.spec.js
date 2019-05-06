import { IS_LOADING } from '@/store/modules/http/mutation-types';
import actions from '@/store/modules/http/actions';

describe('actions', () => {
  let mutation;

  beforeEach(() => {
    actions.app = {
      $cookies: {
        get: jest.fn(),
      },
    };
    actions.dispatch = jest.fn();
    mutation = { commit: jest.fn() };
  });

  describe('is loading', () => {
    it('will have an loading function', () => {
      expect(actions.isLoading).toBeInstanceOf(Function);
    });

    it('will commit the IS_LOADING mutation with a value of true', () => {
      actions.isLoading(mutation);
      expect(mutation.commit).toHaveBeenCalledWith(IS_LOADING);
    });

    it('will dispatch the session/updateLastCalledAt action', () => {
      actions.isLoading(mutation);
      expect(actions.dispatch).toHaveBeenCalledWith('session/updateLastCalledAt');
    });
  });
});
