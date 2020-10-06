import NhsukLayout from '@/components/layout/NhsUkLayout';
import NativeCallbacks from '@/services/native-app';
import { ADVICE } from '@/router/routes/advice';
import { INDEX } from '@/router/routes/general';
import { LOGIN } from '@/router/routes/login';
import { MORE } from '@/router/routes/more';
import {
  APPOINTMENTS,
  BOOKING,
  CONFIRMATION,
} from '@/router/routes/appointments';
import {
  PRESCRIPTIONS,
  REPEAT_COURSES,
} from '@/router/routes/prescriptions';
import {
  DOCUMENT_DETAIL,
  GP_MEDICAL_RECORD,
  HEALTH_RECORDS,
  GP_MEDICAL_RECORD_GP_AT_HAND,
} from '@/router/routes/medical-record';

import { createStore, mockCookies, shallowMount } from '../../helpers';

jest.mock('@/components/widgets/HotJar', () => {});
jest.mock('@/services/native-app');

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
      ...route,
    },
    methods: {
      configureWebContext: helpUrl => helpUrl,
    },
  });
};

const createLayoutStore = (isNativeApp, enabled = true, versionEnabled = true) => createStore({
  $cookies: mockCookies(),
  getters: {
    'errors/showApiError': false,
    'serviceJourneyRules/myRecordHubEnabled': enabled,
    'appVersion/isNativeVersionAfter': jest.fn().mockReturnValue(versionEnabled),
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
    loginSettings: {
      biometricType: undefined,
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
    global.validateSession = () => {};
    NativeCallbacks.fetchBiometricSpec.mockClear();
  });

  describe('is native', () => {
    const isNative = true;
    describe.each([
      [BOOKING.name, BOOKING],
      [CONFIRMATION.name, CONFIRMATION],
      [REPEAT_COURSES.name, REPEAT_COURSES],
    ])('for `%s`', (_, page) => {
      beforeEach(() => {
        $store = createLayoutStore(isNative);
        wrapper = createPage($store, page);
        $store.$cookies.get = jest.fn();
      });

      it('will show breadcrumb', () => {
        expect(wrapper.vm.shouldShowBreadCrumb).toBe(true);
      });
    });

    describe.each([
      [LOGIN.name, LOGIN],
      [ADVICE.name, ADVICE],
      [APPOINTMENTS.name, APPOINTMENTS],
      [PRESCRIPTIONS.name, PRESCRIPTIONS],
      [HEALTH_RECORDS.name, HEALTH_RECORDS],
      [MORE.name, MORE],
    ])('for `%s`', (_, page) => {
      beforeEach(() => {
        $store = createLayoutStore(isNative);
        wrapper = createPage($store, page);
        $store.$cookies.get = jest.fn();
      });

      it('will not show breadcrumb', () => {
        expect(wrapper.vm.shouldShowBreadCrumb).toBe(false);
      });
    });

    describe.each([
      [GP_MEDICAL_RECORD.name, GP_MEDICAL_RECORD],
      [GP_MEDICAL_RECORD_GP_AT_HAND.name, GP_MEDICAL_RECORD_GP_AT_HAND],
    ])('silver integration enabled for `%s`', (_, page) => {
      beforeEach(() => {
        $store = createLayoutStore(isNative, true);
        wrapper = createPage($store, page);
        $store.$cookies.get = jest.fn();
      });

      it('will show breadcrumb', () => {
        expect(wrapper.vm.shouldShowBreadCrumb).toBe(true);
      });
    });

    describe.each([
      [GP_MEDICAL_RECORD.name, GP_MEDICAL_RECORD],
    ])('silver integration is not enabled for `%s`', (_, page) => {
      beforeEach(() => {
        $store = createLayoutStore(isNative, false);
        wrapper = createPage($store, page);
        $store.$cookies.get = jest.fn();
      });

      it('will not show breadcrumb', () => {
        expect(wrapper.vm.shouldShowBreadCrumb).toBe(false);
      });
    });

    describe('is before version for web biometrics', () => {
      beforeEach(() => {
        $store = createLayoutStore({ isNative, versionEnabled: false });
        $store.$cookies.get = jest.fn();
      });

      it('will not call to fetch biometric spec', () => {
        expect(NativeCallbacks.fetchBiometricSpec).toBeCalledTimes(0);
      });
    });
  });

  describe('is web', () => {
    beforeEach(() => {
      $store = createLayoutStore(false);
    });

    describe.each([
      [BOOKING.name, BOOKING],
      [CONFIRMATION.name, CONFIRMATION],
      [REPEAT_COURSES.name, REPEAT_COURSES],
      [ADVICE.name, ADVICE],
      [APPOINTMENTS.name, APPOINTMENTS],
      [PRESCRIPTIONS.name, PRESCRIPTIONS],
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
    ])('for `%s`', (_, page) => {
      beforeEach(() => {
        wrapper = createPage($store, page);
      });

      it('will not show the contentHeader', () => {
        expect(wrapper.vm.shouldShowContentHeader).toBe(false);
      });
    });

    describe.each([
      [BOOKING.name, BOOKING],
      [CONFIRMATION.name, CONFIRMATION],
      [REPEAT_COURSES.name, REPEAT_COURSES],
      [ADVICE.name, ADVICE],
      [APPOINTMENTS.name, APPOINTMENTS],
      [PRESCRIPTIONS.name, PRESCRIPTIONS],
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
