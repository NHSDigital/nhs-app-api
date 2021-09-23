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

  const mountPage = ({ $store, $route }) => {
    const testPage = { template: '<div></div>' };

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

    describe('hasConnectionProblem', () => {
      let hasConnectionErrorBool;

      describe('when online and no Api error', () => {
        beforeEach(() => {
          getters['errors/showApiError'] = false;
          jest.spyOn(navigator, 'onLine', 'get').mockReturnValueOnce(true);
          hasConnectionErrorBool = wrapper.vm.hasConnectionProblem();
        });

        it('will return a value of false', () => {
          expect(hasConnectionErrorBool).toEqual(false);
        });
      });

      describe('when offline and no Api error', () => {
        beforeEach(() => {
          getters['errors/showApiError'] = false;
          jest.spyOn(navigator, 'onLine', 'get').mockReturnValueOnce(false);
          hasConnectionErrorBool = wrapper.vm.hasConnectionProblem();
        });

        it('will return a value of true', () => {
          expect(hasConnectionErrorBool).toEqual(true);
        });
      });
    });
  });
});
