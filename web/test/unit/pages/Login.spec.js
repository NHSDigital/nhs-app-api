import AuthorisationService from '@/services/authorisation-service';
import Login from '@/pages/Login';
import { BEGINLOGIN } from '@/lib/routes';
import NativeApp from '@/services/native-app';
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

  const mountWithQueryData = ({
    isNativeApp = true,
    query,
    source = 'android',
    data,
    instructionsViewed = true,
  }) => {
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
        getters: {
          'device/isNativeApp': isNativeApp,
          'preRegistrationInformation/instructionsViewed': instructionsViewed,
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

  describe('created lifecycle hook', () => {
    let dismissAllDialoguesSpy;

    beforeEach(() => {
      dismissAllDialoguesSpy = jest.spyOn(NativeApp, 'dismissAllDialogues').mockImplementation(() => true);
      AuthorisationService.mockClear();

      wrapper = mountWithQueryData({ query: defaultQuery, instructionsViewed: false });
    });

    it('will call pre registration sync', () => {
      expect($store.dispatch).toHaveBeenCalledWith('preRegistrationInformation/sync');
    });

    it('will call page leave warning reset', () => {
      expect($store.dispatch).toHaveBeenCalledWith('pageLeaveWarning/reset');
    });

    it('sets window onbeforeunload event to null', () => {
      window.onbeforeunload = () => {};

      AuthorisationService.mockClear();
      wrapper = mountWithQueryData({ query: defaultQuery, instructionsViewed: false });

      expect(window.onbeforeunload).toBe(null);
    });

    it('will call native app leave warning reset', () => {
      expect(dismissAllDialoguesSpy).toHaveBeenCalled();
    });
  });

  describe('viewed instructions', () => {
    beforeEach(() => {
      AuthorisationService.mockClear();
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

    it('will not display the header in web', () => {
      const fidoQuery = createFidoQueries();
      wrapper = mountWithQueryData({ query: fidoQuery, isNativeApp: false, source: 'web' });
      expect(wrapper.find('#native-header').exists()).toBe(false);
    });

    it('will display the header in native', () => {
      const fidoQuery = createFidoQueries();
      wrapper = mountWithQueryData({ query: fidoQuery, isNativeApp: true, source: 'android' });
      expect(wrapper.find('#native-header').exists()).toBe(true);
    });
  });

  describe('pre registration button', () => {
    beforeEach(() => {
      wrapper = mountWithQueryData({ query: defaultQuery, instructionsViewed: false });
      AuthorisationService.mockClear();
    });

    it('will not have a form that performs a get request to the begin login path', () => {
      const form = wrapper.find(`form[action="${BEGINLOGIN.path}"]`);

      expect(form.exists()).toBe(false);
    });

    it('will have a button to go to the instructions page', () => {
      expect(wrapper.find('#viewInstructionsButton').exists()).toBe(true);
    });
  });
});
