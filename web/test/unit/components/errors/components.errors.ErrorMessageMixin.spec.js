// ApiError is used as it includes the ErrorMessageMixin and a mixin cannot be mounted directly
import ApiError from '@/components/errors/ApiError';
import { initialState as initialDevice } from '@/store/modules/device/mutation-types';
import { initialState as initialErrors } from '@/store/modules/errors/mutation-types';
import has from 'lodash/fp/has';
import get from 'lodash/fp/get';
import locale from '@/locale';
import { createStore, mount } from '../../helpers';

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
    routePath,
  } = {}) => {
    state = createState();
    state.errors.routePath = routePath;
    state.errors.apiErrors[0] = apiError;

    $store = createStore({
      getters: {
        'errors/showApiError': hasApiError,
      },
      state,
    });

    return mount(ApiError, { $store, $te, $t });
  };

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
      describe('not showing errors', () => {
        it('will be an empty string when not showing errors', () => {
          wrapper = mountErrorMessageMixin();
          expect(wrapper.vm.getMessage('appointments')).toEqual('');
        });
      });
    });

    describe('get text', () => {
      it('will return an empty string if the key does not exist in the locale file', () => {
        wrapper = mountErrorMessageMixin();
        expect(wrapper.vm.getText('mickey.mouse')).toEqual('');
      });

      it('will return the value if the key exists in the locale file', () => {
        wrapper = mountErrorMessageMixin();
        expect(wrapper.vm.getText('errors.pageHeader')).toEqual('Server error');
      });
    });
  });
});
