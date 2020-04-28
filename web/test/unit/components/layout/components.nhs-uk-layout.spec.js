import {
  create$T,
  createRouter,
  createStore,
  mockCookies,
  shallowMount,
} from '../../helpers';

import {
  APPOINTMENT_BOOKING,
  APPOINTMENT_BOOKING_GUIDANCE,
  APPOINTMENT_CONFIRMATIONS,
  APPOINTMENTS,
  DOCUMENT_DETAIL,
  GP_MEDICAL_RECORD,
  HEALTH_RECORDS,
  INDEX,
  LOGIN,
  HEALTH_INFORMATION_UPDATES_MESSAGES,
  MORE,
  MYRECORD,
  MYRECORD_GP_AT_HAND,
  PRESCRIPTION_REPEAT_COURSES,
  PRESCRIPTIONS,
  SYMPTOMS,
} from '@/lib/routes';

const $t = create$T();
const $router = createRouter();

jest.mock('@/components/widgets/HotJar', () => {
});

/* eslint-disable import/first */
import NhsukLayout from '@/components/layout/NhsUkLayout';

const $http = jest.fn();
const $env = {
  ANALYTICS_SCRIPT_URL: 'test script',
};
const $style = {
  homeMain: true,
};

const createPage = ($store, route = INDEX) => {
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
    $http,
    $store,
    $env,
    $route,
    $t,
    $router,
    $style,
    loggedIn,
    methods: {
      configureWebContext(helpUrl) {
        return helpUrl;
      },
    },
    stubs: {
      nuxt: '<div></div>',
    },
  });
};

const createLayoutStore = (isNativeApp, journey = 'silverIntegration', enabled = true) => createStore({
  $cookies: mockCookies(),
  $env: {
    VERSION_TAG: 1,
  },
  getters: {
    'errors/showApiError': false,
    [`serviceJourneyRules/${journey}Enabled`]: () => (enabled),
  },
  state: {
    appVersion: {
      webVersion: '1.2.3',
      nativeVersion: '3.2.1',
    },
    onlineConsultations: {},
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

  it('will load analytics when on a logged in page', () => {
    const $store = createLayoutStore(true);
    $store.app.$cookies.get = jest.fn(() => undefined);
    const defaultPage = createPage($store);
    const head = defaultPage.vm.$options.head.call(defaultPage.vm);

    expect(head.script[0].src).toBe('test script');
  });

  it('will not load analytics when on a logged off page', () => {
    const $store = createLayoutStore(true);
    $store.app.$cookies.get = jest.fn(() => undefined);
    const defaultPage = createPage($store, LOGIN);
    const head = defaultPage.vm.$options.head.call(defaultPage.vm);

    expect(head.script).toBeUndefined();
  });

  describe('breadcrumbs in native', () => {
    const $store = createLayoutStore(true);

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
        const page = createPage($store, name);
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
        const $sjrStore = createLayoutStore(true, journey, enabled);

        const defaultPage = createPage($sjrStore, page);
        expect(defaultPage.vm.shouldShowBreadCrumb).toBe(expectedResult);
      },
    );
  });
});

describe('nhsuk-layout - is web', () => {
  beforeEach(() => {
    process.client = true;
    window.validateSession = () => {};
    jest.clearAllMocks();
  });

  describe('contentHeader in web', () => {
    const $store = createLayoutStore(false);

    it.each([
      [LOGIN, false],
      [DOCUMENT_DETAIL, false],
      [HEALTH_INFORMATION_UPDATES_MESSAGES, false],
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
      'will show the contentHeader on the correct pages in web', (name, expectedResult) => {
        const page = createPage($store, name);
        expect(page.vm.shouldShowContentHeader).toBe(expectedResult);
      },
    );
  });

  describe('breadcrumbs in web', () => {
    const $store = createLayoutStore(false);

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
        const page = createPage($store, name);
        expect(page.vm.shouldShowBreadCrumb).toBe(expectedResult);
      },
    );
  });
});
