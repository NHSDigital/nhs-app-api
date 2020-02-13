import AuthorisationService from '@/services/authorisation-service';
import Login from '@/pages/Login';
import { BEGINLOGIN } from '@/lib/routes';
import { createStore, mount } from '../helpers';

jest.mock('@/services/authorisation-service');
const loginResponse = {
  loginUrl: 'boom',
  request: {
    authoriseUrl: 'bang',
  },
};
AuthorisationService.prototype.generateLoginUrl = jest.fn().mockImplementation()
  .mockReturnValue(loginResponse);

const defaultQuery = {
  source: 'android',
};

const createFidoQueries = () =>
  Object.assign({}, defaultQuery, {
    fidoAuthResponse: 'Boom',
  });

describe('login page', () => {
  let $store;
  let wrapper;

  const mountWithQueryData = ({ isNativeApp = true, query, source = 'android', data }) => {
    const $env = {
      BIOMETRICS_ENABLED: true,
    };

    const $cookies = {
      get: jest.fn().mockImplementation('BetaCookie').mockReturnValue({}),
      set: jest.fn(),
    };

    $store = createStore({
      $env,
      state: {
        appVersion: {
          webVersion: '1.2.3',
          nativeVersion: '3.2.1',
        },
        device: {
          isNativeApp,
          source,
        },
      },
      $cookies,
      context: {
        redirect: jest.fn(),
      },
    });
    return mount(Login, {
      $env,
      $store,
      $route: {
        query,
      },
      $cookies,
      data,
      stubs: {
        'no-ssr': '<div><slot/></div>',
      },
    });
  };

  beforeEach(() => {
    wrapper = mountWithQueryData({ query: defaultQuery });
    AuthorisationService.mockClear();
  });

  it('will have a form that performs a get request to the begin login path', () => {
    const form = wrapper.find(`form[action="${BEGINLOGIN.path}"]`);

    expect(form.exists()).toBe(true);
    expect(form.attributes().method).toEqual('get');
  });

  it('will redirect automatically if FidoAuthResponse exists and is native', () => {
    const fidoQuery = createFidoQueries();
    wrapper = mountWithQueryData({ query: fidoQuery });
    expect(AuthorisationService.mock.instances[0].generateLoginUrl).toHaveBeenCalledTimes(1);
  });

  it('will not automatically redirect if it is native but no fidoResponse', () => {
    wrapper = mountWithQueryData({ query: defaultQuery });
    expect(AuthorisationService).not.toHaveBeenCalled();
  });

  it('will not automatically redirect if it is web', () => {
    const fidoQuery = createFidoQueries();

    wrapper = mountWithQueryData({ query: fidoQuery, isNativeApp: false, source: 'web' });
    expect(AuthorisationService).not.toHaveBeenCalled();
  });

  describe('login button', () => {
    let loginButton;

    beforeEach(() => {
      loginButton = wrapper.find('#login-button');
    });

    it('exists', () => {
      expect(loginButton.exists()).toBe(true);
    });

    it('will not be disabled', () => {
      expect(wrapper.vm.isButtonDisabled).toBe(false);
    });

    describe('click', () => {
      beforeEach(() => {
        loginButton.trigger('click');
      });

      it('will call analytics/satelliteTrack', () => {
        expect($store.dispatch).toHaveBeenCalledWith('analytics/satelliteTrack', 'login');
      });

      it('will be disabled', () => {
        expect(wrapper.vm.isButtonDisabled).toBe(true);
      });
    });
  });
});
