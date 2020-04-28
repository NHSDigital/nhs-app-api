/* eslint-disable import/no-extraneous-dependencies import/imports-first */
import Vuex from 'vuex';
import ContentHeader from '@/components/widgets/ContentHeader';
import WebHeader from '@/components/widgets/WebHeader';
import { shallowMount, createLocalVue } from '@vue/test-utils';
import { create$T, mockCookies } from '../helpers';
import {
  APPOINTMENT_BOOKING,
  APPOINTMENT_BOOKING_GUIDANCE,
  APPOINTMENT_CONFIRMATIONS,
  APPOINTMENTS,
  GP_MEDICAL_RECORD,
  HEALTH_RECORDS,
  INDEX,
  LOGIN,
  MORE,
  MYRECORD,
  MYRECORD_GP_AT_HAND,
  PRESCRIPTION_REPEAT_COURSES,
  PRESCRIPTIONS,
  SYMPTOMS,
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

const createStore = (isNativeApp, journey = 'silverIntegration', enabled = true) => ({
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
    [`serviceJourneyRules/${journey}Enabled`]: () => (enabled),
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

  describe('breadcrumbs in native', () => {
    const $store = createStore(true);

    it.each([
      [LOGIN, false],
      [SYMPTOMS, false],
      [APPOINTMENTS, false],
      [PRESCRIPTIONS, false],
      [MYRECORD, false],
      [HEALTH_RECORDS, false],
      [MORE, false],
      [APPOINTMENT_BOOKING_GUIDANCE, true],
      [APPOINTMENT_BOOKING, true],
      [APPOINTMENT_CONFIRMATIONS, true],
      [PRESCRIPTION_REPEAT_COURSES, true],
    ])(
      'will show breadcrumb on the correct pages when in native', (name, expectedResult) => {
        const page = createDefaultPage($store, name);
        expect(page.vm.shouldShowBreadCrumb).toBe(expectedResult);
      },
    );

    it.each([
      [GP_MEDICAL_RECORD, 'silverIntegration', true, true],
      [GP_MEDICAL_RECORD, 'silverIntegration', false, false],
      [MYRECORD_GP_AT_HAND, 'silverIntegration', true, true],
      [MYRECORD_GP_AT_HAND, 'silverIntegration', false, false],
    ])(
      'will toggle breadcrumb pages with SJR rule', (page, journey, enabled, expectedResult) => {
        const $sjrStore = createStore(true, journey, enabled);

        const defaultPage = createDefaultPage($sjrStore, page);
        expect(defaultPage.vm.shouldShowBreadCrumb).toBe(expectedResult);
      },
    );
  });
});

describe('default.vue - is web', () => {
  beforeEach(() => {
    process.client = true;
    window.validateSession = () => {};
  });

  describe('breadcrumbs in web', () => {
    const $store = createStore(false);

    it.each([
      [LOGIN, false],
      [APPOINTMENT_BOOKING_GUIDANCE, true],
      [APPOINTMENT_BOOKING, true],
      [APPOINTMENT_CONFIRMATIONS, true],
      [PRESCRIPTION_REPEAT_COURSES, true],
      [SYMPTOMS, true],
      [APPOINTMENTS, true],
      [PRESCRIPTIONS, true],
      [MYRECORD, true],
      [MORE, true],
    ])(
      'will show breadcrumb on the correct pages when in web', (name, expectedResult) => {
        const page = createDefaultPage($store, name);
        expect(page.vm.shouldShowBreadCrumb).toBe(expectedResult);
      },
    );
  });
});
