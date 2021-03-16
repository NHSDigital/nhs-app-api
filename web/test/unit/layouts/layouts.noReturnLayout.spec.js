/* eslint-disable no-underscore-dangle */
import ContentHeader from '@/components/widgets/ContentHeader';
import WebHeader from '@/components/widgets/WebHeader';
import i18n from '@/plugins/i18n';
import NoReturnLayout from '@/layouts/no-return-flow-layout';
import OnUpdateTitleMixin from '@/plugins/mixinDefinitions/OnUpdateTitleMixin';
import ResetSpinnerMixin from '@/plugins/mixinDefinitions/ResetSpinnerMixin';
import { UPDATE_HEADER, EventBus } from '@/services/event-bus';
import NativeApp from '@/services/native-app';
import { createStore, shallowMount } from '../helpers';

jest.mock('@/services/event-bus', () => ({
  ...jest.requireActual('@/services/event-bus'),
  EventBus: { $emit: jest.fn(), $on: jest.fn() },
}));

jest.mock('@/services/native-app');

const routeMeta = {
  headerKey: 'no-return-layout-header',
  titleKey: 'no-return-layout-title',
  helpUrl: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/help/',
};

const createDefaultPage = ($store, stubs) => (shallowMount(NoReturnLayout, {
  $store,
  $route: {
    name: 'no-return-flow-layout',
    meta: routeMeta,
  },
  showTemplate: () => true,
  loggedIn: true,
  methods: {
    configureWebContext(url) {
      return url;
    },
  },
  stubs,
  mountOpts: { i18n },
}));

const createLayoutStore = ({
  analyticsCookieAccepted = false,
  analyticsScriptUrl = 'NOT_SET',
  durationSeconds = undefined,
} = {}) => createStore({
  $cookies: { get: cookie => (cookie === 'nhso.session' ? { durationSeconds } : {}) },
  $env: { ANALYTICS_SCRIPT_URL: analyticsScriptUrl, VERSION_TAG: 1 },
  state: {
    device: { isNativeApp: true, source: 'web' },
    appVersion: { nativeVersion: '3.2.1', webVersion: '1.2.3' },
    termsAndConditions: { analyticsCookieAccepted },
  },
});

describe('no-return-flow-layout.vue ', () => {
  beforeEach(() => {
    EventBus.$emit.mockClear();
  });

  it('will show content header', () => {
    const $store = createLayoutStore();
    const defaultPage = createDefaultPage($store);

    expect(defaultPage.find(ContentHeader).exists()).toBe(true);
  });

  describe('mounted()', () => {
    it('will send correct help URL to setHelpUrl mixin function', () => {
      const $store = createLayoutStore();
      const defaultPage = createDefaultPage($store);
      const expectedHelpUrl = 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/help/';

      expect(defaultPage.vm.currentHelpUrl)
        .toBe(expectedHelpUrl);
    });

    it('will emit UPDATE_HEADER passing the current route meta as event', () => {
      createDefaultPage(createLayoutStore(), { 'content-header': '<div/>' });
      expect(EventBus.$emit).toHaveBeenCalledWith(UPDATE_HEADER, routeMeta);
    });

    describe('showWebHeader is true', () => {
      let page;

      beforeEach(() => {
        NativeApp.shouldShowPreLoginHeader = jest.fn().mockImplementation(() => true);
        page = createDefaultPage(createLayoutStore());
      });

      it('will show the web header', () => {
        expect(page.find(WebHeader).exists()).toBe(true);
      });
    });

    describe('showWebHeader is false', () => {
      let page;

      beforeEach(() => {
        NativeApp.shouldShowPreLoginHeader = jest.fn().mockImplementation(() => false);
        page = createDefaultPage(createLayoutStore());
      });

      it('will not show the web header', () => {
        expect(page.find(WebHeader).exists()).toBe(false);
      });
    });
  });


  describe('created()', () => {
    it('will dispatch appVersion/updateWebVersion on created', () => {
      const $store = createLayoutStore();
      jest.spyOn($store, 'dispatch');

      createDefaultPage($store);
      expect($store.dispatch)
        .toHaveBeenLastCalledWith('appVersion/updateWebVersion', 1);
    });
  });
  describe('metaInfo', () => {
    let head;

    describe('web, analytics not accepted, durationSeconds not in session cookie', () => {
      beforeEach(() => {
        const layout = createDefaultPage(createLayoutStore());
        layout.setData({ title: 'No return layout title' });
        head = layout.vm.$options.metaInfo.call(layout.vm);
      });

      it('will set language from locale', () => {
        expect(head.htmlAttrs.lang).toBe('en-GB');
      });

      it('will set title to the pageTitle with the app title appended', () => {
        expect(head.title).toBe('No return layout title - NHS App');
      });

      it('will have no scripts defined', () => {
        expect(head.script).toBeUndefined();
      });

      it('will disable sanitizers for noscript', () => {
        expect(head.__dangerouslyDisableSanitizers).toEqual(['noscript']);
      });
    });

    describe('analytics accepted, url set', () => {
      beforeEach(() => {
        const layout = createDefaultPage(createLayoutStore({
          analyticsCookieAccepted: true,
          analyticsScriptUrl: 'analytics-script-url',
        }));
        head = layout.vm.$options.metaInfo.call(layout.vm);
      });

      it('will add the analytics script', () => {
        expect(head.script[0]).toEqual({ src: 'analytics-script-url' });
      });
    });

    describe.each([
      ['analytics not accepted, url set', { analyticsScriptUrl: 'analytics-script-url' }],
      ['analytics accepted, url not set', { analyticsCookieAccepted: true }],
    ])('%s', (_, options) => {
      beforeEach(() => {
        const layout = createDefaultPage(createLayoutStore(options));
        head = layout.vm.$options.metaInfo.call(layout.vm);
      });

      it('will have no script tags', () => {
        expect(head.script).toBeUndefined();
      });
    });

    describe('durationSeconds set on session cookie', () => {
      beforeEach(() => {
        const layout = createDefaultPage(createLayoutStore({ durationSeconds: '12300' }));
        head = layout.vm.$options.metaInfo.call(layout.vm);
      });

      it('will have a noscript to redirect to /account/signout after durationSeconds', () => {
        expect(head.noscript[0]).toEqual({
          innerHTML: '<meta http-equiv="refresh" content="12300;URL=\'/account/signout\'">',
          body: false,
        });
      });
    });

    describe('mixins', () => {
      it('will include the OnUpdateTitleMixin', () => {
        expect(NoReturnLayout.mixins).toEqual([ResetSpinnerMixin, OnUpdateTitleMixin]);
      });
    });
  });
});

