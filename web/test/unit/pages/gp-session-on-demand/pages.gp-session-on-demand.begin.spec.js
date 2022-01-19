import BeginPage from '@/pages/gp-session-on-demand/begin';
import AuthorisationService from '@/services/authorisation-service';
import NativeApp from '@/services/native-app';
import { UPDATE_TITLE, EventBus } from '@/services/event-bus';
import { mount, createStore } from '../../helpers';

jest.mock('@/services/authorisation-service');
jest.mock('@/services/native-app');
jest.mock('@/services/event-bus', () => ({
  ...jest.requireActual('@/services/event-bus'),
  EventBus: { $emit: jest.fn() },
}));

let store;
let router;
let wrapper;

const hostname = 'localhost';
const loginIdentityToken = 'jwtToken';
const generatedGpSessionUrl = `http://cid/on-demand-gp-session-return&asserted_login_identity=${loginIdentityToken}`;

const mountPage = () => {
  store = createStore({
    state: {
      appVersion: { nativeVersion: true },
      device: { isNativeApp: true },
    },
    $cookies: { set: jest.fn() },
    $http: {
      postV1PatientAssertedLoginIdentity: jest.fn()
        .mockImplementation(() => Promise.resolve({ token: loginIdentityToken })),
    },
  });

  router = {
    currentRoute: {
      query: {
        targetPage: '/appointments',
      },
    },
  };

  return mount(BeginPage, {
    $store: store,
    $router: router,
  });
};

describe('on-demand-gp-return', () => {
  beforeEach(() => {
    EventBus.$emit.mockClear();

    AuthorisationService.mockImplementation(() => ({
      generateGpSessionUrl: jest.fn(() => ({ gpSessionConnectUrl: generatedGpSessionUrl })),
      setAuthCookieForNativeOnDemandGpSession: jest.fn(),
    }));


    delete window.location;
    window.location = { hostname };
  });


  describe('does not support native on demand GP session', () => {
    beforeEach(() => {
      NativeApp.supportsCreateOnDemandGpSession.mockReturnValue(false);

      mountPage();
    });

    it('will load spinner', () => {
      wrapper = mountPage();
      expect(wrapper.find('.loading-spinner-background').isVisible()).toBe(true);
    });

    it('user is navigated to page based on gp session url and identity token', () => {
      expect(store.app.$http.postV1PatientAssertedLoginIdentity).toHaveBeenCalledWith({
        assertedLoginIdentityRequest: {
          IntendedRelyingPartyUrl: 'localhost',
        },
      });
      expect(window.location).toBe(generatedGpSessionUrl);
    });

    it('will dispatch to update title', () => {
      expect(EventBus.$emit)
        .toHaveBeenCalledWith(
          UPDATE_TITLE, 'navigation.pages.titles.pageLoading',
        );
    });
  });

  describe('does support native on demand GP session', () => {
    beforeEach(() => {
      NativeApp.supportsCreateOnDemandGpSession.mockReturnValue(true);
      NativeApp.createOnDemandGpSession = jest.fn();

      mountPage();
    });

    it('will call the assertedLoginIdentity endpoint', () => {
      expect(store.app.$http.postV1PatientAssertedLoginIdentity).toHaveBeenCalledWith({
        assertedLoginIdentityRequest: {
          IntendedRelyingPartyUrl: 'localhost',
        },
      });
    });

    it('will invoke native on demand GP session with the redirect url and token', () => {
      expect(NativeApp.createOnDemandGpSession)
        .toHaveBeenCalledWith(JSON.stringify({
          redirectTo: '/appointments',
          assertedLoginIdentity: 'jwtToken',
        }));
    });

    it('will not update the page title', () => {
      expect(EventBus.$emit).not.toHaveBeenCalledWith(UPDATE_TITLE, expect.anything);
    });
  });
});
