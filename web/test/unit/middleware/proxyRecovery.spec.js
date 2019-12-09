import proxyRecovery from '@/middleware/proxyRecovery';
import {
  MYRECORD,
  INDEX,
} from '@/lib/routes';

describe('middleware/proxyRecovery', () => {
  let getters;
  let store;
  let dispatch;
  let app;

  const callProxyRecovery = (route) => {
    proxyRecovery({ route, store, app });
  };

  beforeEach(() => {
    dispatch = jest.fn();
    getters = [];
    store = {
      getters,
      dispatch,
    };
    const tc = jest.fn().mockImplementation(val => val);
    app = {
      i18n: {
        tc,
      },
    };
  });

  describe('when recovering from a loss of proxy', () => {
    beforeEach(() => {
      getters['linkedAccounts/isRecoveringFromProxyLoss'] = true;
    });

    it('will will dispatch a flash message and show', () => {
      callProxyRecovery(MYRECORD);
      expect(dispatch).toBeCalledWith('flashMessage/addError', 'linkedProfiles.lossProxyError');
      expect(dispatch).toBeCalledWith('flashMessage/show');
    });

    it('will will dispatch a flash message and show and reset on INDEX page', () => {
      callProxyRecovery(INDEX);
      expect(dispatch).toBeCalledWith('flashMessage/addError', 'linkedProfiles.lossProxyError');
      expect(dispatch).toBeCalledWith('flashMessage/show');
      expect(dispatch).toBeCalledWith('linkedAccounts/proxyRecoveryComplete');
    });
  });
});
