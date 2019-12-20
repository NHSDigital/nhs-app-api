import Vuex from 'vuex';
import { shallowMount, createLocalVue } from '@vue/test-utils';
import { create$T, mockCookies, createRouter } from '../helpers';
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
  DOCUMENT_DETAIL,
  GP_MEDICAL_RECORD,
  MORE,
  MESSAGING_MESSAGES,
} from '@/lib/routes';

const $t = create$T();
const $router = createRouter();

jest.mock('@/components/widgets/HotJar', () => {
});
jest.mock('lodash/fp/get');

/* eslint-disable import/first */
import NhsukLayout from '@/layouts/nhsuk-layout';

const $http = jest.fn();
const $env = {
  ANALYTICS_SCRIPT_URL: 'test script',
};
const $style = {
  homeMain: true,
};
const localVue = createLocalVue();

const createPage = ($store, route = INDEX) => {
  localVue.use(Vuex);
  localVue.mixin({
    methods: {
      configureWebContext(helpUrl) {
        return helpUrl;
      },
    },
  });
  NhsukLayout.components.HotJar = {
    computed: {},
    staticRenderFns: [],
    name: 'HotJar',
  };

  const $route = {
    name: route.name,
    query: '',
    crumb: route.crumb,
    shouldShowContentHeader: route.shouldShowContentHeader ?
      route.shouldShowContentHeader : undefined,
  };

  const loggedIn = true;

  return shallowMount(NhsukLayout, {
    localVue,
    mocks: {
      $http,
      $store,
      $env,
      $route,
      $t,
      $router,
      $style,
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
  getters: {},
  dispatch: jest.fn(),
  state: {
    appVersion: {
      webVersion: '1.2.3',
      nativeVersion: '3.2.1',
    },
    device: {
      isNativeApp,
      source: 'ios',
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
    http: {
      loadingUrls: [],
    },
  },
});

describe('nhsuk-layout - is native', () => {
  beforeEach(() => {
    process.client = true;
    window.validateSession = () => {};
    jest.clearAllMocks();
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
      const page = createPage($store, name);
      expect(page.vm.shouldShowBreadCrumb).toBe(false);
    });

    breadcrumbPages.forEach((name) => {
      const page = createPage($store, name);
      expect(page.vm.shouldShowBreadCrumb).toBe(true);
    });
  });

  it('will load analytics when on a logged in page', () => {
    const $store = createStore(true);
    $store.app.$cookies.get = jest.fn(() => undefined);
    const defaultPage = createPage($store);
    const head = defaultPage.vm.$options.head.call(defaultPage.vm);

    expect(head.script[0].src).toBe('test script');
  });

  it('will not load analytics when on a logged off page', () => {
    const $store = createStore(true);
    $store.app.$cookies.get = jest.fn(() => undefined);
    const defaultPage = createPage($store, LOGIN);
    const head = defaultPage.vm.$options.head.call(defaultPage.vm);

    expect(head.script).toBeUndefined();
  });
});

describe('nhsuk-layout - is web', () => {
  beforeEach(() => {
    process.client = true;
    window.validateSession = () => {};
  });

  it('will show the contentHeader on the correct pages in web', () => {
    const $store = createStore(false);

    const noContentHeaderPages = [
      LOGIN,
      DOCUMENT_DETAIL,
      MESSAGING_MESSAGES,
    ];

    const contentHeaderPages = [
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

    noContentHeaderPages.forEach((name) => {
      const page = createPage($store, name);
      expect(page.vm.shouldShowContentHeader).toBe(false);
    });

    contentHeaderPages.forEach((name) => {
      const page = createPage($store, name);
      expect(page.vm.shouldShowContentHeader).toBe(true);
    });
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
      const page = createPage($store, name);
      expect(page.vm.shouldShowBreadCrumb).toBe(false);
    });

    breadcrumbPages.forEach((name) => {
      const page = createPage($store, name);
      expect(page.vm.shouldShowBreadCrumb).toBe(true);
    });
  });
});
