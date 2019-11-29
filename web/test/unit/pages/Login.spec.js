import AuthorisationService from '@/services/authorisation-service';
import Login from '@/pages/Login';
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

const mountWithQueryData = ({ isNativeApp = true, query, source = 'android', data }) => {
  const $env = {
    BIOMETRICS_ENABLED: true,
    THROTTLING_ENABLED: true,
    ORGAN_DONATION_THROTTLING_URL: 'www.foo.com',
  };


  const $cookies = {
    get: jest.fn().mockImplementation('BetaCookie').mockReturnValue({}),
    set: jest.fn(),
  };

  return mount(Login, {
    $env,
    $store: createStore({
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
    }),
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

const createFidoQueries = () =>
  Object.assign({}, defaultQuery, {
    fidoAuthResponse: 'Boom',
  });

describe('login page', () => {
  let wrapper;

  beforeEach(() => {
    wrapper = mountWithQueryData({ query: defaultQuery });
    AuthorisationService.mockClear();
  });

  it('has a login button', () => {
    expect(wrapper.find('#login-button').exists()).toBe(true);
  });

  it('will call generateLoginValues only once when login button clicked', () => {
    expect(AuthorisationService).not.toHaveBeenCalled();
    wrapper.find('#login-button').trigger('submit');
    wrapper.find('#login-button').trigger('submit');
    wrapper.find('#login-button').trigger('submit');
    expect(AuthorisationService.mock.instances[0].generateLoginUrl).toHaveBeenCalledTimes(1);
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

  it('will not display record organ donation link', () => {
    expect(wrapper.find('#btn_organDonation').exists()).toBe(false);
  });

  describe('throttling', () => {
    beforeEach(() => {
      wrapper = mountWithQueryData({
        query: defaultQuery,
        data: () => ({ practiceParticipating: false }),
      });
    });

    describe('record organ donation link', () => {
      let link;

      beforeEach(() => {
        link = wrapper.find('#btn_organDonation');
      });

      it('will exist', () => {
        expect(link.exists()).toBe(true);
      });

      it('will translate "shared.organDonation.recordDecision"', () => {
        expect(link.text()).toBe('translate_shared.organDonation.recordDecision');
      });
    });
  });
});
