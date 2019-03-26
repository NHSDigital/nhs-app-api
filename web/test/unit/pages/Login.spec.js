import LoginButton from '@/components/LoginButton';
import Login from '@/pages/Login';
import AuthorisationService from '@/services/authorisation-service';
import { $t, createStore, mount } from '../helpers';

jest.mock('@/services/authorisation-service');
const loginResponse = {
  loginUrl: 'boom',
  request: {
    authoriseUrl: 'bang',
  },
};
AuthorisationService.prototype.generateLoginUrl = jest.fn().mockImplementation()
  .mockReturnValue(loginResponse);

const $env = {
  BIOMETRICS_ENABLED: true,
  THROTTLING_ENABLED: true,
};
const defaultQuery = {
  source: 'android',
};

let $cookies;
let $store;
let cookieValue;
let state;
let stubs;
let wrapper;

const mountWithQueryData = ({ isNativeApp = true, query, source = 'android' }) => {
  state = {
    appVersion: {
      webVersion: '1.2.3',
      nativeVersion: '3.2.1',
    },
    device: {
      isNativeApp,
      source,
    },
  };

  cookieValue = {};
  $cookies = {
    get: jest.fn().mockImplementation('BetaCookie').mockReturnValue(cookieValue),
    set: jest.fn(),
  };

  $store = createStore({
    $env,
    state,
    $cookies,
    context: {
      redirect: jest.fn(),
    },
  });

  stubs = ['no-ssr'];

  return mount(Login, {
    $env,
    $store,
    $t,
    $route: {
      query,
    },
    $cookies,
    stubs,
  });
};

const createFidoQueries = () =>
  Object.assign({}, defaultQuery, {
    fidoAuthResponse: 'Boom',
  });

describe('login page', () => {
  beforeEach(() => {
    wrapper = mountWithQueryData({ query: defaultQuery });
    AuthorisationService.mockClear();
  });

  it('has a login button', () => {
    expect(wrapper.find(LoginButton).exists()).toBe(true);
  });

  it('will call generateLoginValues only once when login button clicked', () => {
    expect(AuthorisationService).not.toHaveBeenCalled();
    wrapper.find(LoginButton).trigger('submit');
    wrapper.find(LoginButton).trigger('submit');
    wrapper.find(LoginButton).trigger('submit');
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
});
