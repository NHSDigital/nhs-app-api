/* eslint-disable import/no-extraneous-dependencies import/imports-first */
import Vuex from 'vuex';
import ContentHeader from '@/components/widgets/ContentHeader';
import WebHeader from '@/components/widgets/WebHeader';
import { shallowMount, createLocalVue } from '@vue/test-utils';
import { create$T, mockCookies } from '../helpers';
import {
  INDEX,
  LOGIN,
  SYMPTOMS,
  APPOINTMENTS,
  APPOINTMENT_BOOKING,
  APPOINTMENT_BOOKING_GUIDANCE,
  APPOINTMENT_CONFIRMATIONS,
  PRESCRIPTIONS,
  PRESCRIPTION_REPEAT_COURSES,
  MYRECORD,
  GP_MEDICAL_RECORD,
  MORE,
} from '@/lib/routes';

const $t = create$T();

jest.mock('@/components/widgets/HotJar', () => {
});
/* eslint-disable import/first */
import DefaultPage from '@/layouts/default';

const $http = jest.fn();
const $env = {
  ANALYTICS_SCRIPT_URL: 'test script',
};
const $style = {
  homeMain: true,
};
const localVue = createLocalVue();


const createDefaultPage = ($store, route = INDEX) => {
  localVue.use(Vuex);
  localVue.mixin({
    methods: {
      configureWebContext(helpUrl) {
        return helpUrl;
      },
    },
  });
  DefaultPage.components.HotJar = {
    computed: {},
    staticRenderFns: [],
    name: 'HotJar',
  };
  const $route = {
    name: route.name,
    query: '',
    crumb: route.crumb,
  };
  const loggedIn = true;
  return shallowMount(DefaultPage, {
    localVue,
    mocks: {
      $http,
      $store,
      $env,
      $route,
      $t,
      $style,
      showTemplate: () => true,
      loggedIn,
    },
    stubs: {
      nuxt: '<div></div>',
    },
  });
};

const createDefaultPageForLoginScreen = ($store) => {
  localVue.use(Vuex);
  const $route = {
    query: '',
    name: 'Login',
  };
  const loggedIn = true;
  return shallowMount(DefaultPage, {
    localVue,
    mocks: {
      $http,
      $store,
      $env,
      $route,
      $style,
      $t,
      showTemplate: () => true,
      loggedIn,
    },
    stubs: {
      nuxt: '<div></div>',
    },
  });
};

const createStore = isNativeApp => ({
  app: {
    $cookies: mockCookies(),
    $env: {
      VERSION_TAG: 1,
    },
  },
  dispatch: jest.fn(),
  subscribe: jest.fn(),
  getters: {
    'errors/showApiError': false,
  },
  state: {
    appVersion: {
      webVersion: '1.2.3',
      nativeVersion: '3.2.1',
    },
    device: {
      isNativeApp,
      source: 'ios',
    },
    header: {
      headerText: 'someheader',
    },
    pageTitle: {
      pageTitle: 'some title',
    },
    session: {
      csrfToken: 'someToken',
    },
    termsAndConditions: {
      analyticsCookieAccepted: true,
    },
  },
});

describe('default.vue - is native', () => {
  beforeEach(() => {
    process.client = true;
    window.validateSession = () => {
    };
  });

  it('will dispatch native login message when native', () => {
    const $store = createStore(true);
    jest.spyOn($store, 'dispatch');

    createDefaultPage($store);
    expect($store.dispatch)
      .toHaveBeenCalledWith('auth/nativeLogin');
  });

  it('will show breadcrumb on the correct pages when native', () => {
    const $store = createStore(true);

    const noBreadcrumbPages = [
      LOGIN,
      SYMPTOMS,
      APPOINTMENTS,
      PRESCRIPTIONS,
      MYRECORD,
      GP_MEDICAL_RECORD,
      MORE,
    ];

    const breadcrumbPages = [
      APPOINTMENT_BOOKING_GUIDANCE,
      APPOINTMENT_BOOKING,
      APPOINTMENT_CONFIRMATIONS,
      PRESCRIPTION_REPEAT_COURSES,
    ];

    noBreadcrumbPages.forEach((name) => {
      const defaultPage = createDefaultPage($store, name);
      expect(defaultPage.vm.shouldShowBreadCrumb).toBe(false);
    });

    breadcrumbPages.forEach((name) => {
      const defaultPage = createDefaultPage($store, name);
      expect(defaultPage.vm.shouldShowBreadCrumb).toBe(true);
    });
  });

  it('will not dispatch native login message when native', () => {
    const $store = createStore(false);

    jest.spyOn($store, 'dispatch');

    createDefaultPage($store);
    expect($store.dispatch)
      .not
      .toHaveBeenLastCalledWith('auth/nativeLogin');
  });

  it('will show content header with breadcrumb when not in login page', () => {
    const $store = createStore(true);
    const defaultPage = createDefaultPage($store);

    jest.spyOn($store, 'dispatch');

    expect(defaultPage.find(ContentHeader).exists()).toBe(true);
    expect(defaultPage.vm.shouldShowBreadCrumb).toBe(true);
  });

  it('will show content header without breadcrumb when in login page', () => {
    const $store = createStore(true);
    const defaultPage = createDefaultPageForLoginScreen($store);

    jest.spyOn($store, 'dispatch');

    expect(defaultPage.find(ContentHeader).exists()).toBe(true);
    expect(defaultPage.vm.shouldShowBreadCrumb).toBe(false);
  });

  it('will show full desktop header when not native and not in login page', () => {
    const $store = createStore(false);
    const defaultPage = createDefaultPage($store);

    jest.spyOn($store, 'dispatch');

    expect(defaultPage.vm.shouldShowFullDesktopHeader).toBe(true);
    expect(defaultPage.find(WebHeader).exists()).toBe(true);
  });

  it('will send correct help URL to setHelpUrl mixin function', () => {
    const $store = createStore(true);
    const defaultPage = createDefaultPage($store);
    const expectedHelpUrl = 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/help/';

    jest.spyOn($store, 'dispatch');

    expect(defaultPage.vm.currentHelpUrl).toBe(expectedHelpUrl);
  });

  it('will load analytics when on a logged in page', () => {
    const $store = createStore(true);
    $store.app.$cookies.get = jest.fn(() => undefined);
    const defaultPage = createDefaultPage($store);
    const head = defaultPage.vm.$options.head.call(defaultPage.vm);

    expect(head.script[0].src).toBe('test script');
  });

  it('will not load analytics when on a logged off page', () => {
    const $store = createStore(true);
    $store.app.$cookies.get = jest.fn(() => undefined);
    const defaultPage = createDefaultPageForLoginScreen($store);
    const head = defaultPage.vm.$options.head.call(defaultPage.vm);

    expect(head.script).toBeUndefined();
  });
});

describe('default.vue - is web', () => {
  beforeEach(() => {
    process.client = true;
    window.validateSession = () => {};
  });

  it('will show breadcrumb on the correct pages when in web', () => {
    const $store = createStore(false);

    const noBreadcrumbPages = [
      LOGIN,
    ];

    const breadcrumbPages = [
      APPOINTMENT_BOOKING_GUIDANCE,
      APPOINTMENT_BOOKING,
      APPOINTMENT_CONFIRMATIONS,
      PRESCRIPTION_REPEAT_COURSES,
      SYMPTOMS,
      APPOINTMENTS,
      PRESCRIPTIONS,
      MYRECORD,
      GP_MEDICAL_RECORD,
      MORE,
    ];

    noBreadcrumbPages.forEach((name) => {
      const defaultPage = createDefaultPage($store, name);
      expect(defaultPage.vm.shouldShowBreadCrumb).toBe(false);
    });

    breadcrumbPages.forEach((name) => {
      const defaultPage = createDefaultPage($store, name);
      expect(defaultPage.vm.shouldShowBreadCrumb).toBe(true);
    });
  });
});
