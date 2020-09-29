/* eslint-disable no-underscore-dangle */
import ContentHeader from '@/components/widgets/ContentHeader';
import i18n from '@/plugins/i18n';
import TsAndCsLayout from '@/layouts/termsAndConditions';
import OnUpdateTitleMixin from '@/plugins/mixinDefinitions/OnUpdateTitleMixin';
import ResetSpinnerMixin from '@/plugins/mixinDefinitions/ResetSpinnerMixin';
import { UPDATE_HEADER, EventBus } from '@/services/event-bus';
import { createStore, shallowMount } from '../helpers';

jest.mock('@/services/event-bus', () => ({
  ...jest.requireActual('@/services/event-bus'),
  EventBus: { $emit: jest.fn(), $on: jest.fn() },
}));

const routeMeta = {
  headerKey: 'terms-conditions-header',
  helpUrl: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/help/',
};

const createDefaultPage = ($store, stubs) => (shallowMount(TsAndCsLayout, {
  $store,
  $route: {
    name: 'terms-and-conditions',
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

describe('termsAndConditions.vue ', () => {
  beforeEach(() => {
    window.validateSession = () => {};
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
  });

  describe('created()', () => {
    it('will dispatch appVersion/updateWebVersion on created', () => {
      const $store = createLayoutStore();
      jest.spyOn($store, 'dispatch');

      createDefaultPage($store);
      expect($store.dispatch)
        .toHaveBeenLastCalledWith('appVersion/updateWebVersion', 1);
    });

    it('will dispatch session/updateLastCalledAt if process is browser', () => {
      process.browser = true;
      const $store = createLayoutStore();
      jest.spyOn($store, 'dispatch');

      createDefaultPage($store);
      expect($store.dispatch)
        .toHaveBeenCalledWith('session/updateLastCalledAt');
    });

    it('will not dispatch session/updateLastCalledAt if process is not browser', () => {
      process.browser = false;
      const $store = createLayoutStore();
      jest.spyOn($store, 'dispatch');

      createDefaultPage($store);
      expect($store.dispatch)
        .not.toHaveBeenCalledWith('session/updateLastCalledAt');
    });
  });
  describe('metaInfo', () => {
    let head;

    describe('web, analytics not accepted, durationSeconds not in session cookie', () => {
      beforeEach(() => {
        const layout = createDefaultPage(createLayoutStore());
        layout.setData({ title: 'Accept conditions of use' });
        head = layout.vm.$options.metaInfo.call(layout.vm);
      });

      it('will set language from locale', () => {
        expect(head.htmlAttrs.lang).toBe('en-GB');
      });

      it('will set title to be the ts & cs pageTitle with the app title appended', () => {
        expect(head.title).toBe('Accept conditions of use - NHS App');
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
        expect(TsAndCsLayout.mixins).toEqual([ResetSpinnerMixin, OnUpdateTitleMixin]);
      });
    });
  });
});

