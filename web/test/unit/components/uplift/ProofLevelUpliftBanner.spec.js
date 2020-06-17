import AuthorisationService from '@/services/authorisation-service';
import ProofLevelUpliftBanner from '@/components/uplift/ProofLevelUpliftBanner';
import { mount } from '../../helpers';

jest.mock('@/services/authorisation-service');

describe('Proof level uplift banner', () => {
  const hostname = 'www.example.com';
  let $http;
  let response;
  let wrapper;

  const mountProofLevelUpliftBanner = () => mount(ProofLevelUpliftBanner, {
    $store: {
      $env: {
        NATIVE_CID_REDIRECT_URI: 'mock native cid redirect uri',
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
    },
  });

  beforeEach(() => {
    response = {};
    $http = {
      postV1PatientAssertedLoginIdentity: jest.fn(() => Promise.resolve(response)),
    };
    AuthorisationService.mockImplementation(() => ({
      generateUpliftUrl: jest.fn(() => ({ upliftUrl: 'www.foo.com' })),
    }));
    wrapper = mountProofLevelUpliftBanner();
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
        expect(window.location).toBe(`${wrapper.vm.upliftUrl}&asserted_login_identity=${response.token}`);
      });
    });

    afterEach(() => {
      window.location = location;
    });
  });
});
