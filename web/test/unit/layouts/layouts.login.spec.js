import i18n from '@/plugins/i18n';
import LoginLayout from '@/layouts/login';
import VueMeta from 'vue-meta';
import { createStore, localVue, shallowMount } from '../helpers';

localVue.use(VueMeta);

describe('login layout', () => {
  describe('metaInfo', () => {
    let head;

    beforeEach(() => {
      const wrapper = shallowMount(
        LoginLayout,
        {
          $store: createStore({
            state: {
              device: { isNativeApp: true, source: 'web' },
              appVersion: { nativeVersion: '3.2.1', webVersion: '1.2.3' },
              errors: { hasConnectionProblem: false },
              session: { showExpiryMessage: false },
            },
          }),
          $route: {
            name: 'login',
            meta: {
              helpUrl: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/help/',
            },
            query: {},
          },
          methods: {
            configureWebContext(url) {
              return url;
            },
          },
          mountOpts: { i18n },
        },
      );
      head = wrapper.vm.$meta().refresh().metaInfo;
    });

    it('will set language from locale', () => {
      expect(head.htmlAttrs.lang).toBe('en-GB');
    });

    it('will set title to be the pageTitle', () => {
      expect(head.title).toBe('Login screen');
    });

    it('will have no scripts defined', () => {
      expect(head.script.length).toBe(0);
    });
  });
});
