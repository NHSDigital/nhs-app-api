import AuthorisationService from '@/services/authorisation-service';
import Login from '@/pages/Login';
import { BEGINLOGIN_PATH } from '@/router/paths';
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

const fidoQuery = { fidoAuthResponse: 'Boom' };

describe('login page', () => {
  let $store;
  let wrapper;
  let $cookies;

  beforeEach(() => {
    delete window.location;
  });

  const mountPage = ({
    isNativeApp = true,
    query = {},
    data,
    instructionsViewed = true,
  } = {}) => {
    $cookies = {
      get: jest.fn().mockImplementation('BetaCookie').mockReturnValue({}),
      set: jest.fn(),
    };

    $store = createStore({
      state: {
        appVersion: {
          webVersion: '1.2.3',
          nativeVersion: '3.2.1',
        },
        device: {
          isNativeApp,
        },
        getters: {
          'preRegistrationInformation/instructionsViewed': instructionsViewed,
        },
      },
      $cookies,
    });

    return mount(Login, {
      shallow: true,
      $store,
      $route: {
        query,
      },
      $cookies,
      data,
    });
  };

  beforeEach(() => {
    AuthorisationService.mockClear();
  });

  describe('template', () => {
    it('will display the header in native', () => {
      wrapper = mountPage();
      expect(wrapper.find('#native-header').exists()).toBe(true);
    });
  });

  describe('created lifecycle hook', () => {
    let dismissAllDialoguesSpy;

    beforeEach(() => {
      dismissAllDialoguesSpy = jest.spyOn(NativeApp, 'dismissAllDialogues').mockImplementation(() => true);

      wrapper = mountPage({ instructionsViewed: false });
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
      wrapper = mountPage({ instructionsViewed: false });

      expect(window.onbeforeunload).toBe(null);
    });

    it('will call native app leave warning reset', () => {
      expect(dismissAllDialoguesSpy).toHaveBeenCalled();
    });
  });

  describe('mounted', () => {
    describe('handling fido', () => {
      describe('on native', () => {
        let generateLoginUrl;

        beforeEach(() => {
          wrapper = mountPage({ query: fidoQuery });
          /* eslint-disable-next-line prefer-destructuring */
          generateLoginUrl = AuthorisationService.mock.instances[0].generateLoginUrl;
        });

        it('will generate login url using fido auth response', () => {
          expect(generateLoginUrl).toHaveBeenCalledTimes(1);
          expect(generateLoginUrl).toHaveBeenCalledWith({
            isNativeApp: true,
            cookies: $cookies,
            redirectTo: undefined,
            fidoAuthResponse: 'Boom',
          });
        });

        it('will set the window location to generated login url', () => {
          expect(window.location).toEqual('boom');
        });
      });

      describe('on web', () => {
        it('will not generate login url or change window location', () => {
          wrapper = mountPage({ query: fidoQuery, isNativeApp: false });
          expect(AuthorisationService).not.toHaveBeenCalled();
          expect(window.location).toBeUndefined();
        });
      });
    });

    describe.each([
      ['on native', true],
      ['on web', false],
    ])('not handling fido', (description, isNativeApp) => {
      describe(description, () => {
        it('will not generate login url or change window location', () => {
          wrapper = mountPage({ isNativeApp });
          expect(AuthorisationService).not.toHaveBeenCalled();
          expect(window.location).toBeUndefined();
        });
      });
    });
  });

  describe('pre registration button', () => {
    beforeEach(() => {
      wrapper = mountPage({ instructionsViewed: false });
    });

    it('will not have a form that performs a get request to the begin login path', () => {
      const form = wrapper.find(`form[action="${BEGINLOGIN_PATH}"]`);

      expect(form.exists()).toBe(false);
    });

    it('will have a button to go to the instructions page', () => {
      expect(wrapper.find('#viewInstructionsButton').exists()).toBe(true);
    });
  });
});
