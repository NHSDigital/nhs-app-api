import AuthorisationService from '@/services/authorisation-service';
import LoginButton from '@/components/widgets/LoginButton';
import { createStore, mount } from '../../helpers';

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

describe('login page', () => {
  let $store;
  let wrapper;

  const mountWithQueryData = ({ isNativeApp = true, query, source = 'android', data }) => {
    const $env = {
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
        preRegistrationInformation: {
          seen: true,
        },
      },
      $cookies,
      context: {
        redirect: jest.fn(),
      },
    });
    return mount(LoginButton, {
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

  describe('login button', () => {
    let loginButton;

    beforeEach(() => {
      loginButton = wrapper.find('#login-button');
    });

    it('exists', () => {
      expect(loginButton.exists()).toBe(true);
    });

    it('will not be disabled prior to clicking', () => {
      expect(wrapper.vm.isButtonDisabled).toBe(false);
    });

    describe('click', () => {
      beforeEach(() => {
        loginButton.trigger('click');
      });

      it('will call analytics/satelliteTrack', () => {
        expect($store.dispatch).toHaveBeenCalledWith('analytics/satelliteTrack', 'login');
      });

      it('will be disabled as button has been clicked', () => {
        expect(wrapper.vm.isButtonDisabled).toBe(true);
      });
    });
  });
});
