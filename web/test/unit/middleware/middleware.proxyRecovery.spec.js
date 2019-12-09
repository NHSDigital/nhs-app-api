import proxyRecovery from '@/middleware/proxyRecovery';
import { createStore } from '../helpers';

describe('middleware/proxyRecovery', () => {
  const errorText = 'proxy error text';
  let store;
  let app;

  const callProxyRecovery = routeName => proxyRecovery({ route: { name: routeName }, store, app });

  beforeEach(() => {
    store = createStore();
    app = {
      i18n: {
        tc: jest.fn().mockReturnValue(errorText),
      },
    };
  });

  it('will add proxy error is recovering from proxy loss', () => {
    // arrange
    store.getters['linkedAccounts/isRecoveringFromProxyLoss'] = true;

    // act
    callProxyRecovery('index');

    // assert
    expect(app.i18n.tc).toHaveBeenCalledWith('linkedProfiles.lossProxyError');
    expect(store.dispatch).toHaveBeenCalledWith('flashMessage/addError', errorText);
    expect(store.dispatch).toHaveBeenCalledWith('flashMessage/show');
    expect(store.dispatch).toHaveBeenCalledWith('linkedAccounts/proxyRecoveryComplete');
  });

  it('will not add proxy error when not recovering from proxy loss', () => {
    // arrange
    store.getters['linkedAccounts/isRecoveringFromProxyLoss'] = false;

    // act
    callProxyRecovery('test-route-name');

    // assert
    expect(store.dispatch).not.toHaveBeenCalledWith('flashMessage/addError', errorText);
    expect(store.dispatch).not.toHaveBeenCalledWith('flashMessage/show');
    expect(store.dispatch).not.toHaveBeenCalledWith('linkedAccounts/proxyRecoveryComplete');
  });
});
