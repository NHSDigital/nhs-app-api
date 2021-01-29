import AuthorisationService from '@/services/authorisation-service';
import Login from '@/pages/Login';
import NativeApp from '@/services/native-app';
import { PRE_REGISTRATION_INFORMATION_PATH } from '@/router/paths';
import { redirectTo } from '@/lib/utils';
import { createStore, mount } from '../helpers';

jest.mock('@/services/authorisation-service');
jest.mock('@/lib/utils');

const loginResponse = {
  loginUrl: 'boom',
  request: {
    authoriseUrl: 'bang',
  },
};

AuthorisationService.prototype.generateLoginUrl = jest.fn().mockReturnValue(loginResponse);

const fidoQuery = { fidoAuthResponse: 'Boom' };

describe('login page', () => {
  let $store;
  let wrapper;

  const mountPage = ({
    isNativeApp = true,
    query = {},
    data,
  } = {}) => {
    $store = createStore({
      state: {
        appVersion: {
          webVersion: '1.2.3',
          nativeVersion: '3.2.1',
        },
        device: {
          isNativeApp,
        },
      },
    });

    return mount(Login, {
      $store,
      $route: {
        query,
      },
      data,
      stubs: {
        'login-layout': '<div><slot/></div>',
      },
    });
  };

  beforeEach(() => {
    delete window.location;
    AuthorisationService.mockClear();
    redirectTo.mockClear();
  });

  describe('template', () => {
    it('will display the header in native', () => {
      wrapper = mountPage();
      expect(wrapper.find('#native-header').exists()).toBe(true);
    });
  });

  describe('created lifecycle hook', () => {
    let dismissPageLeaveWarningDialogue;

    beforeEach(() => {
      dismissPageLeaveWarningDialogue = jest.spyOn(NativeApp, 'dismissPageLeaveWarningDialogue').mockImplementation(() => true);

      wrapper = mountPage();
    });

    it('will call page leave warning reset', () => {
      expect($store.dispatch).toBeCalledWith('pageLeaveWarning/reset');
    });

    it('sets window onbeforeunload event to null', () => {
      window.onbeforeunload = () => {};

      AuthorisationService.mockClear();
      wrapper = mountPage();

      expect(window.onbeforeunload).toBe(null);
    });

    it('will call native app leave warning reset', () => {
      expect(dismissPageLeaveWarningDialogue).toBeCalled();
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
          expect(generateLoginUrl).toBeCalledTimes(1);
          expect(generateLoginUrl).toBeCalledWith({
            isNativeApp: true,
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
          expect(AuthorisationService).not.toBeCalled();
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
          expect(AuthorisationService).not.toBeCalled();
          expect(window.location).toBeUndefined();
        });
      });
    });
  });

  describe('continue button', () => {
    const query = { REDIRECT_PARAMETER: 'foo' };
    let button;

    beforeEach(() => {
      wrapper = mountPage({ query });
      button = wrapper.find('#viewInstructionsButton');
    });

    it('will exist', () => {
      expect(button.exists()).toBe(true);
    });

    it('will be enabled', () => {
      expect(wrapper.vm.isButtonDisabled).toBe(false);
    });

    describe('on click', () => {
      beforeEach(() => {
        button.trigger('click');
      });

      it('will disable button', () => {
        expect(wrapper.vm.isButtonDisabled).toBe(true);
      });

      it('will dispatch `analytics/satelliteTrack`', () => {
        expect($store.dispatch).toBeCalledWith('analytics/satelliteTrack', 'login');
      });

      it('will redirect to PRE_REGISTRATION_INFORMATION_PATH and passing the query', () => {
        expect(redirectTo).toBeCalledWith(wrapper.vm, PRE_REGISTRATION_INFORMATION_PATH, query);
      });
    });
  });
});
