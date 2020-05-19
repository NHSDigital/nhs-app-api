import NhsukLayout from '@/components/layout/NhsUkLayout';
import {
  APPOINTMENTS,
  APPOINTMENT_BOOKING,
  APPOINTMENT_BOOKING_GUIDANCE,
  APPOINTMENT_CONFIRMATIONS,
  DOCUMENT_DETAIL,
  GP_MEDICAL_RECORD,
  HEALTH_INFORMATION_UPDATES_MESSAGES,
  HEALTH_RECORDS,
  INDEX,
  LOGIN,
  MORE,
  MYRECORD,
  MYRECORD_GP_AT_HAND,
  PRESCRIPTIONS,
  PRESCRIPTION_REPEAT_COURSES,
  SYMPTOMS,
} from '@/lib/routes';
import { createStore, mockCookies, shallowMount } from '../../helpers';

jest.mock('@/components/widgets/HotJar', () => {});

const createPage = ($store, route = INDEX) => {
  NhsukLayout.components.HotJar = {
    computed: {},
    staticRenderFns: [],
    name: 'HotJar',
  };

  return shallowMount(NhsukLayout, {
    $env: {
      ANALYTICS_SCRIPT_URL: 'test script',
    },
    $store,
    $route: {
      name: route.name,
      query: '',
      crumb: route.crumb,
      shouldShowContentHeader: route.shouldShowContentHeader ?
        route.shouldShowContentHeader : undefined,
    },
    methods: {
      configureWebContext: helpUrl => helpUrl,
    },
    stubs: {
      nuxt: '<div></div>',
    },
  });
};

const createLayoutStore = (isNativeApp, enabled = true) => createStore({
  $cookies: mockCookies(),
  getters: {
    'errors/showApiError': false,
    'serviceJourneyRules/myRecordHubEnabled': enabled,
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

describe('NhsUkLayout', () => {
  let $store;
  let wrapper;

  beforeEach(() => {
    process.client = true;
    global.validateSession = () => {};
  });

  describe('is native', () => {
    const isNative = true;

    beforeEach(() => {
      $store = createLayoutStore(isNative);
      $store.app.$cookies.get = jest.fn();
    });

    it('will load analytics when on a logged in page', () => {
      const defaultPage = createPage($store);
      const head = defaultPage.vm.$options.head.call(defaultPage.vm);

      expect(head.script[0].src).toBe('test script');
    });

    it('will not load analytics when on a logged off page', () => {
      const defaultPage = createPage($store, LOGIN);
      const head = defaultPage.vm.$options.head.call(defaultPage.vm);

      expect(head.script).toBeUndefined();
    });

    describe.each([
      [APPOINTMENT_BOOKING_GUIDANCE.name, APPOINTMENT_BOOKING_GUIDANCE],
      [APPOINTMENT_BOOKING.name, APPOINTMENT_BOOKING],
      [APPOINTMENT_CONFIRMATIONS.name, APPOINTMENT_CONFIRMATIONS],
      [PRESCRIPTION_REPEAT_COURSES.name, PRESCRIPTION_REPEAT_COURSES],
    ])('for `%s`', (_, page) => {
      beforeEach(() => {
        wrapper = createPage($store, page);
      });

      it('will show breadcrumb', () => {
        expect(wrapper.vm.shouldShowBreadCrumb).toBe(true);
      });
    });

    describe.each([
      [LOGIN.name, LOGIN],
      [SYMPTOMS.name, SYMPTOMS],
      [APPOINTMENTS.name, APPOINTMENTS],
      [PRESCRIPTIONS.name, PRESCRIPTIONS],
      [MYRECORD.name, MYRECORD],
      [HEALTH_RECORDS.name, HEALTH_RECORDS],
      [MORE.name, MORE],
    ])('for `%s`', (_, page) => {
      beforeEach(() => {
        wrapper = createPage($store, page);
      });

      it('will not show breadcrumb', () => {
        expect(wrapper.vm.shouldShowBreadCrumb).toBe(false);
      });
    });

    describe.each([
      [GP_MEDICAL_RECORD.name, GP_MEDICAL_RECORD],
      [MYRECORD_GP_AT_HAND.name, MYRECORD_GP_AT_HAND],
    ])('silver integration enabled for `%s`', (_, page) => {
      beforeEach(() => {
        $store = createLayoutStore(isNative, true);
        wrapper = createPage($store, page);
      });

      it('will show breadcrumb', () => {
        expect(wrapper.vm.shouldShowBreadCrumb).toBe(true);
      });
    });

    describe.each([
      [GP_MEDICAL_RECORD.name, GP_MEDICAL_RECORD],
      [MYRECORD_GP_AT_HAND.name, MYRECORD_GP_AT_HAND],
    ])('silver integration is not enabled for `%s`', (_, page) => {
      beforeEach(() => {
        $store = createLayoutStore(isNative, false);
        wrapper = createPage($store, page);
      });

      it('will not show breadcrumb', () => {
        expect(wrapper.vm.shouldShowBreadCrumb).toBe(false);
      });
    });
  });

  describe('is web', () => {
    beforeEach(() => {
      $store = createLayoutStore(false);
    });

    describe.each([
      [APPOINTMENT_BOOKING_GUIDANCE.name, APPOINTMENT_BOOKING_GUIDANCE],
      [APPOINTMENT_BOOKING.name, APPOINTMENT_BOOKING],
      [APPOINTMENT_CONFIRMATIONS.name, APPOINTMENT_CONFIRMATIONS],
      [PRESCRIPTION_REPEAT_COURSES.name, PRESCRIPTION_REPEAT_COURSES],
      [SYMPTOMS.name, SYMPTOMS],
      [APPOINTMENTS.name, APPOINTMENTS],
      [PRESCRIPTIONS.name, PRESCRIPTIONS],
      [MYRECORD.name, MYRECORD],
      [MORE.name, MORE],
    ])('for `%s`', (_, page) => {
      beforeEach(() => {
        wrapper = createPage($store, page);
      });

      it('will show the contentHeader', () => {
        expect(wrapper.vm.shouldShowContentHeader).toBe(true);
      });
    });

    describe.each([
      [LOGIN.name, LOGIN],
      [DOCUMENT_DETAIL.name, DOCUMENT_DETAIL],
      [HEALTH_INFORMATION_UPDATES_MESSAGES.name, HEALTH_INFORMATION_UPDATES_MESSAGES],
    ])('for `%s`', (_, page) => {
      beforeEach(() => {
        wrapper = createPage($store, page);
      });

      it('will not show the contentHeader', () => {
        expect(wrapper.vm.shouldShowContentHeader).toBe(false);
      });
    });

    describe.each([
      [APPOINTMENT_BOOKING_GUIDANCE.name, APPOINTMENT_BOOKING_GUIDANCE],
      [APPOINTMENT_BOOKING.name, APPOINTMENT_BOOKING],
      [APPOINTMENT_CONFIRMATIONS.name, APPOINTMENT_CONFIRMATIONS],
      [PRESCRIPTION_REPEAT_COURSES.name, PRESCRIPTION_REPEAT_COURSES],
      [SYMPTOMS.name, SYMPTOMS],
      [APPOINTMENTS.name, APPOINTMENTS],
      [PRESCRIPTIONS.name, PRESCRIPTIONS],
      [MYRECORD.name, MYRECORD],
      [MORE.name, MORE],
    ])('for `%s`', (_, page) => {
      beforeEach(() => {
        wrapper = createPage($store, page);
      });

      it('will show breadcrumb', () => {
        expect(wrapper.vm.shouldShowBreadCrumb).toBe(true);
      });
    });

    describe.each([
      [LOGIN.name, LOGIN],
    ])('for `%s`', (_, page) => {
      beforeEach(() => {
        wrapper = createPage($store, page);
      });

      it('will not show breadcrumb', () => {
        expect(wrapper.vm.shouldShowBreadCrumb).toBe(false);
      });
    });
  });
});
