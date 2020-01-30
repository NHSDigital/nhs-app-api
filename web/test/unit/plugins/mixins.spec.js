import '@/plugins/mixins';
// eslint-disable-next-line import/no-extraneous-dependencies
import Vue from 'vue';
import Vuex from 'vuex';
import NativeCallbacks from '@/services/native-app';
import { createLocalVue, mount } from '@vue/test-utils';
import { INDEX } from '@/lib/routes';
import { createUri } from '@/lib/noJs';
import { initialState } from '@/store/modules/errors/mutation-types';
import { redirectTo } from '@/lib/utils';
import { createStore } from '../helpers';

jest.mock('@/lib/utils');

const localVue = createLocalVue();
localVue.use(Vuex);

describe('mixins', () => {
  let state;
  let getters;
  let wrapper;

  const mountPage = ({ $store }) => {
    const testPage = { template: '<div></div>' };

    return mount(testPage, {
      localVue,
      mocks: {
        $route: { path: '/foo' },
        $store,
      },
    });
  };

  beforeEach(() => {
    state = { errors: initialState() };
    getters = { 'session/isProxying': false };
    wrapper = mountPage({ $store: createStore({ state, getters }) });
  });

  it('will have correct number of globally registered mounted mixins', () => {
    const expectedGlobalMountedMixins = [
      'ResetPageFocus',
    ];
    // assert
    const mountedFunctions = Vue.options.mounted;
    expect(mountedFunctions.length).toBe(expectedGlobalMountedMixins.length);
  });

  describe('methods', () => {
    describe('configureWebContext', () => {
      const currentUrl = '/test-url';
      let configureWebContext;

      beforeEach(() => {
        configureWebContext = jest.spyOn(NativeCallbacks, 'configureWebContext');
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
            wrapper.vm.configureWebContext(currentUrl);
          });

          it('will call native callback `configureWebContext` with empty retry URL', () => {
            expect(configureWebContext).toBeCalledWith(currentUrl, '');
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
            wrapper.vm.configureWebContext(currentUrl);
          });

          it('will call native callback `configureWebContext` with empty redirect URL', () => {
            expect(configureWebContext).toBeCalledWith(currentUrl, redirectUrl);
          });
        });

        describe('when proxying', () => {
          beforeEach(() => {
            getters['session/isProxying'] = true;
            wrapper.vm.configureWebContext(currentUrl);
          });

          it('will call native callback `configureWebContext` with correct redirect URL', () => {
            const redirectUrl = createUri({
              path: INDEX.path,
              noJs: {
                flashMessage: {
                  show: true,
                  key: 'linkedProfiles.lossProxyError',
                  type: 'error',
                },
              },
            });
            expect(configureWebContext).toBeCalledWith(currentUrl, redirectUrl);
          });
        });
      });

      describe('not native app', () => {
        beforeEach(() => {
          state.device = { isNativeApp: false };
          wrapper.vm.configureWebContext(currentUrl);
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
        expect(redirectTo).toBeCalledWith(wrapper.vm, '/foo');
      });
    });
  });
});
