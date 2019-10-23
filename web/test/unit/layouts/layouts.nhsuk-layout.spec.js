import Vuex from 'vuex';
import { shallowMount, createLocalVue } from '@vue/test-utils';
import { create$T } from '../helpers';

const $t = create$T();

jest.mock('@/components/widgets/HotJar', () => {
});
/* eslint-disable import/first */
import NhsukLayout from '@/layouts/nhsuk-layout';

const $http = jest.fn();
const $env = {};
const $style = {
  homeMain: true,
};
const localVue = createLocalVue();

const createPage = ($store, routeName = 'index') => {
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
    query: '',
    name: routeName,
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
    $env: {
      VERSION_TAG: 1,
    },
  },
  dispatch: jest.fn(),
  state: {
    device: {
      isNativeApp,
    },
    session: {
      csrfToken: 'someToken',
    },
    termsAndConditions: {
      analyticsCookieAccepted: true,
    },
  },
});

describe('nhsuk-layout - is native', () => {
  beforeEach(() => {
    process.client = true;
    window.validateSession = () => {};
  });

  it('will show breadcrumb on the correct pages when native', () => {
    const $store = createStore(true);

    const noBreadcrumbPages = [
      'Login',
      'symptoms',
      'appointments',
      'prescriptions',
      'my-record',
      'gp-medical-record',
      'more',
      'account',
    ];

    const breadcrumbPages = [
      'appointments-booking-guidance',
      'appointments-booking',
      'appointments-confirmation',
      'prescriptions-repeat-courses',
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

describe('nhsuk-layout - is web', () => {
  beforeEach(() => {
    process.client = true;
    window.validateSession = () => {};
  });

  it('will show breadcrumb on the correct pages when in web', () => {
    const $store = createStore(false);

    const noBreadcrumbPages = [
      'Login',
    ];

    const breadcrumbPages = [
      'appointments-booking-guidance',
      'appointments-booking',
      'appointments-confirmation',
      'prescriptions-repeat-courses',
      'symptoms',
      'appointments',
      'prescriptions',
      'my-record',
      'gp-medical-record',
      'more',
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
