import DefaultMixin from '@/plugins/mixinDefinitions/DefaultMixin';
// eslint-disable-next-line import/no-extraneous-dependencies
import Vuex from 'vuex';
import NativeApp from '@/services/native-app';
import { createLocalVue, mount } from '@vue/test-utils';
import { INDEX_PATH } from '@/router/paths';
import { createUri } from '@/lib/noJs';
import { initialState } from '@/store/modules/errors/mutation-types';
import * as utils from '@/lib/utils';
import { createStore } from '../helpers';

jest.mock('@/lib/utils');

const localVue = createLocalVue();
localVue.use(Vuex);
localVue.mixin(DefaultMixin);

describe('mixins', () => {
  let state;
  let getters;
  let wrapper;
  let store;

  const mountPage = ({ $store, $route }) => {
    const testPage = { template: '<div></div>' };
    store = $store;

    return mount(testPage, {
      localVue,
      mocks: {
        $route,
        $store,
      },
    });
  };

  beforeEach(() => {
    state = { errors: initialState() };
    getters = { 'session/isProxying': false };

    const $env = { BASE_NHS_APP_HELP_URL: 'http://stubs.local.bitraft.io' };
    const $route = { path: '/foo', query: { param: 'value' }, meta: { helpPath: '/help' } };
    wrapper = mountPage({ $store: createStore({ state, getters, $env }), $route });
  });

  describe('computed', () => {
    describe('showTemplate', () => {
      it('will return true when there is network access and no api error', () => {
        getters['errors/showApiError'] = false;
        store.state.errors.hasConnectionProblem = false;

        expect(wrapper.vm.showTemplate).toEqual(true);
      });

      it('will return false when there is network access and api error', () => {
        getters['errors/showApiError'] = true;
        store.state.errors.hasConnectionProblem = false;

        expect(wrapper.vm.showTemplate).toEqual(false);
      });

      it('will return false when there is no network access and no api error', () => {
        getters['errors/showApiError'] = false;
        store.state.errors.hasConnectionProblem = true;

        expect(wrapper.vm.showTemplate).toEqual(false);
      });

      it('will return false when there is no network access and api error', () => {
        getters['errors/showApiError'] = true;
        store.state.errors.hasConnectionProblem = true;

        expect(wrapper.vm.showTemplate).toEqual(false);
      });
    });

    describe('currentHelpUrl', () => {
      it('should call the generate help link function with the correct parameters', () => {
        const result = wrapper.vm.currentHelpUrl; // eslint-disable-line no-unused-vars

        expect(utils.generateContextualHelpLink)
          .toBeCalledWith(
            expect.objectContaining({ $env: wrapper.vm.$store.$env }),
            expect.objectContaining({ meta: wrapper.vm.$route.meta }),
          );
      });
    });
  });

  describe('methods', () => {
    describe('configureWebContext', () => {
      let configureWebContext;

      beforeEach(() => {
        configureWebContext = jest.spyOn(NativeApp, 'configureWebContext');
        utils.generateContextualHelpLink = jest.fn().mockReturnValue('http://stubs.local.bitraft.io/help');
      });

      afterEach(() => {
        configureWebContext.mockClear();
      });

      describe('native app', () => {
        beforeEach(() => {
          state.device = { isNativeApp: true };
        });

        describe('no error redirect URL', () => {
          beforeEach(() => {
            state.errors.pageSettings = {
              redirectUrl: {
                default: undefined,
              },
            };
            wrapper.vm.configureWebContext();
          });

          it('will call native callback `configureWebContext` with empty retry URL', () => {
            expect(configureWebContext)
              .toBeCalledWith('http://stubs.local.bitraft.io/help', '');
          });
        });

        describe('has error redirect URL', () => {
          const redirectUrl = '/redirect-url';

          beforeEach(() => {
            state.errors.pageSettings = {
              redirectUrl: {
                default: redirectUrl,
              },
            };
            wrapper.vm.configureWebContext();
          });

          it('will call native callback `configureWebContext` with empty redirect URL', () => {
            expect(configureWebContext).toBeCalledWith('http://stubs.local.bitraft.io/help',
              redirectUrl);
          });
        });

        describe('when proxying', () => {
          beforeEach(() => {
            getters['session/isProxying'] = true;
            wrapper.vm.configureWebContext();
          });

          it('will call native callback `configureWebContext` with correct redirect URL', () => {
            const redirectUrl = createUri({
              path: INDEX_PATH,
              noJs: {
                flashMessage: {
                  show: true,
                  key: 'profiles.itMayNotBePossibleToProxyRightNow',
                  type: 'error',
                },
              },
            });
            expect(configureWebContext).toBeCalledWith('http://stubs.local.bitraft.io/help', redirectUrl);
          });
        });
      });

      describe('not native app', () => {
        beforeEach(() => {
          state.device = { isNativeApp: false };
          wrapper.vm.configureWebContext();
        });

        it('will not call native callback `configureWebContext`', () => {
          expect(configureWebContext).not.toBeCalled();
        });
      });
    });

    describe('reload', () => {
      beforeEach(() => {
        wrapper.vm.reload();
      });

      it('will redirect to current route', () => {
        expect(utils.redirectTo).toBeCalledWith(wrapper.vm, '/foo', { param: 'value' });
      });
    });

    describe('watch - $router.query.ts', () => {
      it('will dispatch `errors/setConnectionProblem` with the value false when navigator online', () => {
        jest.spyOn(navigator, 'onLine', 'get').mockReturnValue(true);
        wrapper.setData({ $route: { query: { ts: 'foo' } } });

        expect(store.dispatch).toBeCalledWith('errors/setConnectionProblem', false);
      });

      it('will dispatch `errors/setConnectionProblem` with the value true when navigator not online', () => {
        jest.spyOn(navigator, 'onLine', 'get').mockReturnValue(false);
        wrapper.setData({ $route: { query: { ts: 'foo' } } });

        expect(store.dispatch).toBeCalledWith('errors/setConnectionProblem', true);
      });
    });
  });
});
