import '@/plugins/mixins';
// eslint-disable-next-line import/no-extraneous-dependencies
import Vue from 'vue';
import Vuex from 'vuex';
import NativeCallbacks from '@/services/native-app';
import { createLocalVue, mount } from '@vue/test-utils';
import { initialState } from '@/store/modules/errors/mutation-types';
import { createStore } from '../helpers';

const localVue = createLocalVue();
localVue.use(Vuex);

describe('mixins', () => {
  let state;
  let wrapper;

  const mountPage = ({ $store }) => {
    const testPage = { template: '<div></div>' };

    return mount(testPage, {
      localVue,
      mocks: {
        $store,
      },
    });
  };

  beforeEach(() => {
    state = { errors: initialState() };
    wrapper = mountPage({ $store: createStore({ state }) });
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
  });
});
