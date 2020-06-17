/* eslint-disable no-underscore-dangle */
import NhsukLayout from '@/layouts/nhsuk-layout';
import { UPDATE_HEADER, EventBus } from '@/services/event-bus';
import OnUpdateTitleMixin from '@/plugins/mixinDefinitions/OnUpdateTitleMixin';
import { create$T } from '../helpers';

jest.mock('@/services/event-bus', () => ({
  ...jest.requireActual('@/services/event-bus'),
  EventBus: { $on: jest.fn(), $off: jest.fn(), $emit: jest.fn() },
}));

const createContext = ({
  analyticsCookieAccepted = false,
  analyticsScriptUrl = 'NOT_SET',
  isAnonymousRoute = true,
  durationSeconds = undefined,
  nativeVersion = '',
  source = 'web',
} = {}) => ({
  $t: create$T(),
  $route: { meta: { isAnonymous: isAnonymousRoute } },
  $store: {
    $cookies: { get: cookie => (cookie === 'nhso.session' ? { durationSeconds } : {}) },
    $env: { ANALYTICS_SCRIPT_URL: analyticsScriptUrl },
    state: {
      device: { source },
      appVersion: { nativeVersion, webVersion: '1.2.3' },
      termsAndConditions: { analyticsCookieAccepted },
    },
  },
  title: 'Test Title',
});

describe('nhsuk layout', () => {
  describe('metaInfo', () => {
    let head;

    describe('web, analytics not accepted, durationSeconds not in session cookie', () => {
      beforeEach(() => {
        head = NhsukLayout.metaInfo.call(createContext());
      });

      it('will set language from locale', () => {
        expect(head.htmlAttrs.lang).toBe('translate_language');
      });

      it('will set title to be the component title', () => {
        expect(head.title).toBe('Test Title');
      });

      it('will include the appTitle in the titleTempalte', () => {
        expect(head.titleTemplate).toBe('%s - translate_appTitle');
      });

      it('will have no scripts defined', () => {
        expect(head.script).toBeUndefined();
      });

      it('will have a meta entry for the web version', () => {
        expect(head.meta[0]).toEqual({ name: 'web version', content: '1.2.3' });
      });

      it('will have a meta entry for the platform ', () => {
        expect(head.meta[1]).toEqual({ name: 'platform', content: 'web' });
      });

      it('will disable sanitizers for noscript', () => {
        expect(head.__dangerouslyDisableSanitizers).toEqual(['noscript']);
      });
    });

    describe('analytics accepted, url set, and is not anonymous route', () => {
      beforeEach(() => {
        head = NhsukLayout.metaInfo.call(createContext({
          analyticsCookieAccepted: true,
          analyticsScriptUrl: 'analytics-script-url',
          isAnonymousRoute: false,
        }));
      });

      it('will add the analytics script', () => {
        expect(head.script[0]).toEqual({ src: 'analytics-script-url' });
      });
    });

    describe.each([
      ['analytics not accepted, url set, and is not anonymous route', {
        analyticsScriptUrl: 'analytics-script-url',
        isAnonymousRoute: false,
      }],
      ['analytics accepted, url not set, and is not anonymous route', {
        analyticsCookieAccepted: true,
        isAnonymousRoute: false,
      }],
      ['analytics accepted, url set, and is anonymous route', {
        analyticsCookieAccepted: true,
        analyticsScriptUrl: 'analytics-script-url',
      }],
    ])('%s', (_, options) => {
      beforeEach(() => {
        head = NhsukLayout.metaInfo.call(createContext(options));
      });

      it('will have no script tags', () => {
        expect(head.script).toBeUndefined();
      });
    });

    describe('durationSeconds set on session cookie', () => {
      beforeEach(() => {
        head = NhsukLayout.metaInfo.call(createContext({ durationSeconds: '12300' }));
      });

      it('will have a noscript to redirect to /account/signout after durationSeconds', () => {
        expect(head.noscript[0]).toEqual({
          innerHTML: '<meta http-equiv="refresh" content="12300;URL=\'/account/signout\'">',
          body: false,
        });
      });
    });

    describe('on native', () => {
      beforeEach(() => {
        head = NhsukLayout.metaInfo.call(createContext({ nativeVersion: '1.3.4', source: 'testiOS' }));
      });

      it('will add nativeVersion to platform meta', () => {
        expect(head.meta[1]).toEqual({ name: 'platform', content: 'testiOS (1.3.4)' });
      });
    });
  });

  describe('on update title', () => {
    beforeEach(() => {
      EventBus.$on.mockClear();
      EventBus.$off.mockClear();
    });

    it('will emit UPDATE_HEADER with to.meta in beforeRouteUpdate', () => {
      const meta = { titleKey: 'aubergine' };
      const onUpdateTitle = jest.fn();

      NhsukLayout.beforeRouteUpdate.call({ onUpdateTitle }, { meta }, undefined, jest.fn());

      expect(EventBus.$emit).toHaveBeenCalledWith(UPDATE_HEADER, meta);
    });

    it('will call onUpdateTitle passing to.meta in beforeRouteUpdate', () => {
      const meta = { titleKey: 'aubergine' };
      const onUpdateTitle = jest.fn();

      NhsukLayout.beforeRouteUpdate.call({ onUpdateTitle }, { meta }, undefined, jest.fn());

      expect(onUpdateTitle).toHaveBeenCalledWith(meta);
    });
  });

  describe('mixins', () => {
    it('will include the OnUpdateTitleMixin', () => {
      expect(NhsukLayout.mixins).toEqual([OnUpdateTitleMixin]);
    });
  });
});
