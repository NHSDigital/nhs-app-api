import {
  IS_LOADING,
  IS_LOADING_EXTERNAL_SITE,
  LOADING_COMPLETE,
} from '@/store/modules/http/mutation-types';
import actions from '@/store/modules/http/actions';

describe('actions', () => {
  let mutation;
  const url = 'http://foo/';

  beforeEach(() => {
    actions.app = {
      $cookies: {
        get: jest.fn(),
      },
    };
    actions.dispatch = jest.fn();
    mutation = { commit: jest.fn() };
  });

  describe('isLoading', () => {
    it('will have an loading function', () => {
      expect(actions.isLoading).toBeInstanceOf(Function);
    });

    it('will commit the IS_LOADING mutation with the correct url', () => {
      actions.isLoading(mutation, url);
      expect(mutation.commit).toHaveBeenCalledWith(IS_LOADING, url);
    });

    it('will log an error if the url is not specified', () => {
      actions.isLoading(mutation);
      expect(actions.dispatch).toHaveBeenCalledWith('log/onError', 'url not specified in isLoading');
    });

    it('will dispatch the session/updateLastCalledAt action', () => {
      actions.isLoading(mutation, url);
      expect(actions.dispatch).toHaveBeenCalledWith('session/updateLastCalledAt');
    });
  });

  describe('isLoadingExternalSite', () => {
    it('will have an loading external site function', () => {
      expect(actions.isLoadingExternalSite).toBeInstanceOf(Function);
    });

    it('will commit the IS_LOADING_EXTERNAL_SITE mutation', () => {
      actions.isLoadingExternalSite(mutation);
      expect(mutation.commit).toHaveBeenCalledWith(IS_LOADING_EXTERNAL_SITE);
    });
  });

  describe('loading completed', () => {
    it('will commit the LOADING_COMPLETED mutation with the correct url', () => {
      actions.loadingCompleted(mutation, url);
      expect(mutation.commit).toHaveBeenCalledWith(LOADING_COMPLETE, url);
    });

    it('will log an error if no url is specified', () => {
      actions.loadingCompleted(mutation);
      expect(actions.dispatch).toHaveBeenCalledWith('log/onError', 'url not specified in loadingCompleted');
    });
  });
});
