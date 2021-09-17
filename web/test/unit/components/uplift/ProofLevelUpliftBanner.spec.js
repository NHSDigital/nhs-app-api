import AuthorisationService from '@/services/authorisation-service';
import ProofLevelUpliftBanner from '@/components/uplift/ProofLevelUpliftBanner';
import NativeApp from '@/services/native-app';
import { mount } from '../../helpers';

jest.mock('@/services/authorisation-service');
jest.mock('@/services/native-app');

describe('Proof level uplift banner', () => {
  const hostname = 'www.example.com';
  let $http;
  let response;
  let wrapper;
  let $store;

  const mountProofLevelUpliftBanner = ({ store }) => mount(ProofLevelUpliftBanner, {
    $store: store,
  });

  beforeEach(() => {
    response = {};
    $http = {
      postV1PatientAssertedLoginIdentity: jest.fn(() => Promise.resolve(response)),
    };
    AuthorisationService.mockImplementation(() => ({
      generateUpliftUrl: jest.fn(() => ({ upliftUrl: 'www.foo.com' })),
    }));
    $store = {
      dispatch: jest.fn(),
      $env: {
        CID_REDIRECT_URI: 'mock cid redirect uri',
        CID_CLIENT_ID: 'mock cid client ID',
        CID_AUTH_ENDPOINT: 'mock cid auth endpoint',
      },
      $http,
      state: {
        device: {
          isNativeApp: false,
        },
      },
    };
    wrapper = mountProofLevelUpliftBanner({ store: $store });
  });

  describe('uplift button', () => {
    const { location } = window;
    let upliftButton;

    beforeEach(() => {
      delete window.location;
      window.location = {
        hostname,
      };
      upliftButton = wrapper.find('button');
    });

    it('will exist', () => {
      expect(upliftButton.exists()).toBe(true);
    });

    describe('click', () => {
      beforeEach(() => {
        upliftButton.trigger('click');
        response.token = 'token';
      });

      it('will call `postV1PatientAssertedLoginIdentity`', () => {
        expect($http.postV1PatientAssertedLoginIdentity).toBeCalledWith({
          assertedLoginIdentityRequest: {
            IntendedRelyingPartyUrl: hostname,
            action: 'UpliftStarted',
          },
        });
      });

      it('will redirect to external uplift journey', () => {
        expect($store.dispatch).toBeCalledWith('http/isLoadingExternalSite');
        expect(window.location).toBe(`${wrapper.vm.upliftUrl}&asserted_login_identity=${response.token}`);
      });
    });

    describe('NativeApp methods are supported and correct uplift json is returned to native app', () => {
      beforeEach(() => {
        NativeApp.supportsNativeNhsLoginUplift.mockReturnValue(true);
        NativeApp.startNhsLoginUplift = jest.fn();
        upliftButton.trigger('click');
        response.token = 'token';
      });

      it('will return correct uplift json', () => {
        expect(NativeApp.startNhsLoginUplift).toBeCalledWith('token');
      });
    });

    afterEach(() => {
      window.location = location;
      NativeApp.startNhsLoginUplift.mockClear();
    });
  });
});
