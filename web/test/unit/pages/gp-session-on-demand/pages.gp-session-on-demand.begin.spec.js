import BeginPage from '@/pages/gp-session-on-demand/begin';
import AuthorisationService from '@/services/authorisation-service';
import { mount, createStore } from '../../helpers';

jest.mock('@/services/authorisation-service');

describe('on-demand-gp-return', () => {
  let store;
  let router;
  const hostname = 'localhost';
  const generatedGpSessionUrl = 'http://cid/on-demand-gp-session-return/';
  const loginIdentityToken = 'jwtToken';
  const mountPage = () => {
    store = createStore({
      dispatch: jest.fn(() => Promise.resolve()),
      $env: {},
      state: {
        appVersion: {
          nativeVersion: true,
        },
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

  beforeEach(() => {
    delete window.location;
    window.location = {
      hostname,
    };

    AuthorisationService.mockImplementation(() => ({
      generateGpSessionUrl: jest.fn(() => ({ gpSessionConnectUrl: generatedGpSessionUrl })),
    }));
  });

  it('will load spinner', async () => {
    await mountPage();
    expect(store.dispatch).toHaveBeenCalledWith('http/isLoadingExternalSite');
  });

  it('user is navigated to page based on gp session url and identity token', async () => {
    await mountPage();
    expect(store.app.$http.postV1PatientAssertedLoginIdentity).toHaveBeenCalledWith({
      assertedLoginIdentityRequest: {
        IntendedRelyingPartyUrl: 'localhost',
      },
    });
    expect(window.location).toBe(`${generatedGpSessionUrl}&asserted_login_identity=${loginIdentityToken}`);
  });
});
