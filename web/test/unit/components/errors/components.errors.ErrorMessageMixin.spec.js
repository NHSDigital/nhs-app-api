// ApiError is used as it includes the ErrorMessageMixin and a mixin cannot be mounted directly
import ApiError from '@/components/errors/ApiError';
import { initialState as initialDevice } from '@/store/modules/device/mutation-types';
import { initialState as initialErrors } from '@/store/modules/errors/mutation-types';
import { createStore, mount } from '../../helpers';
import has from 'lodash/fp/has';
import get from 'lodash/fp/get';
import locale from '@/locale';

const engLocale = locale.en;
const $te = key => has(key, engLocale);
const $t = key => get(key, engLocale);

const createState = () => ({
  device: initialDevice(),
  errors: initialErrors(),
});

describe('error message mixin', () => {
  let $store;
  let state;
  let wrapper;

  const mountErrorMessageMixin = ({
    apiError,
    hasApiError = false,
    hasConnectionError = false,
    routePath,
    showError = true,
  } = {}) => {
    state = createState();
    if (hasConnectionError) {
      state.errors.hasConnectionProblem = hasConnectionError;
    }

    if (routePath) {
      state.errors.routePath = routePath;
    }

    if (apiError) {
      state.errors.apiErrors[0] = apiError;
    }

    $store = createStore({
      getters: {
        'errors/showApiError': hasApiError,
      },
      state,
    });

    const mounted = mount(ApiError, { $store, $te, $t });
    mounted.vm.showError = jest.fn().mockReturnValue(showError);
    return mounted;
  };

  beforeEach(() => {
    wrapper = mountErrorMessageMixin();
  });

  describe('computed', () => {
    describe('component', () => {
      it('will be the route path without the leading slash', () => {
        wrapper = mountErrorMessageMixin({ routePath: '/mypage' });
        expect(wrapper.vm.component).toEqual('mypage');
      });

      it('will substitude "/" for "."', () => {
        wrapper = mountErrorMessageMixin({ routePath: '/my/page' });
        expect(wrapper.vm.component).toEqual('my.page');
      });

      it('will substitude "-" for "_"', () => {
        wrapper = mountErrorMessageMixin({ routePath: '/my-page' });
        expect(wrapper.vm.component).toEqual('my_page');
      });
    });

    describe('domain', () => {
      it('will be "errors" when there are API errors', () => {
        const apiError = {
          status: 401,
          error: 'forbidden',
        };
        wrapper = mountErrorMessageMixin({ apiError, hasApiError: true });
        expect(wrapper.vm.domain).toEqual('errors');
      });

      it('will be "noConnection" when there are no API errors', () => {
        wrapper = mountErrorMessageMixin({ hasApiError: false });
        expect(wrapper.vm.domain).toEqual('noConnection');
      });
    });
  });

  describe('methods', () => {
    describe('get component error code key', () => {
      it('will be empty string when there is no API error', () => {
        wrapper = mountErrorMessageMixin({ hasApiError: false });
        expect(wrapper.vm.getComponentErrorCodeKey('any')).toEqual('');
      });

      it('will be value of `[component].errors.[statusCode].[errorCode].[type] if it exists', () => {
        const apiError = {
          status: 401,
          error: 'forbidden',
        };

        wrapper = mountErrorMessageMixin({ apiError, hasApiError: true, routePath: '/mypath' });
        wrapper.vm.getText = jest.fn().mockReturnValue('something');
        wrapper.vm.getComponentErrorCodeKey('typeName');
        expect(wrapper.vm.getText).toHaveBeenCalledWith('mypath.errors.401.forbidden.typeName');
      });
    });

    describe('get message', () => {
      describe('has connection error', () => {
        it('will return the component error code key when one is returned', () => {
          const expected = 'an error';

          wrapper = mountErrorMessageMixin({ hasConnectionError: true });
          wrapper.vm.getText = jest.fn().mockReturnValue(expected);

          expect(wrapper.vm.getMessage('appointments')).toEqual(expected);
          expect(wrapper.vm.getText).toHaveBeenCalledWith('noConnection.appointments');
        });
      });
      describe('showing errors', () => {
        it('will return the component error code key when one is returned', () => {
          const expected = 'an error';

          wrapper = mountErrorMessageMixin({ hasConnectionError: true });
          wrapper.vm.getText = jest.fn().mockReturnValue(expected);

          expect(wrapper.vm.getMessage('appointments')).toEqual(expected);
          expect(wrapper.vm.getText).toHaveBeenCalledWith('noConnection.appointments');
        });
      });

      describe('not showing errors', () => {
        beforeEach(() => {
          wrapper.vm.showError = jest.fn().mockReturnValue(false);
        });

        it('will be an empty string when not showing errors', () => {
          expect(wrapper.vm.getMessage('appointments')).toEqual('');
        });
      });
    });

    describe('get text', () => {
      it('will return an empty string if the key does not exist in the locale file', () => {
        expect(wrapper.vm.getText('mickey.mouse')).toEqual('');
      });

      it('will return the value if the key exists in the locale file', () => {
        expect(wrapper.vm.getText('errors.pageHeader')).toEqual('Server error');
      });
    });
  });
});
