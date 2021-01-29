import NativeApp from '@/services/native-app';
import PreRegistrationInformation from '@/pages/pre-registration-information/index';
import { BEGINLOGIN_PATH } from '@/router/paths';
import { redirectTo } from '@/lib/utils';
import { createStore, mount } from '../helpers';

jest.mock('@/lib/utils');
jest.mock('@/services/native-app');

describe('pre registration information', () => {
  const query = {
    foo: 'example',
  };
  let $store;
  let wrapper;

  const mountWrapper = () => {
    $store = createStore({
      state: {
        device: {
          isNativeApp: true,
        },
      },
    });

    return mount(PreRegistrationInformation, {
      $store,
      $route: {
        query,
      },
      stubs: {
        'nhs-uk-app-layout': '<div><slot/></div>',
      },
    });
  };

  beforeEach(() => {
    wrapper = mountWrapper();
  });

  afterEach(() => {
    redirectTo.mockClear();
    NativeApp.showHeaderSlim.mockClear();
    NativeApp.hideWhiteScreen.mockClear();
  });

  describe('mounted', () => {
    it('will show slim header', () => {
      expect(NativeApp.showHeaderSlim).toBeCalled();
    });

    it('will hide white screen', () => {
      expect(NativeApp.hideWhiteScreen).toBeCalled();
    });
  });

  describe('login button', () => {
    let loginButton;

    beforeEach(() => {
      loginButton = wrapper.find('#login-button');
    });

    it('exists', () => {
      expect(loginButton.exists()).toBe(true);
    });

    it('will not be disabled prior to clicking', () => {
      expect(wrapper.vm.isButtonDisabled).toBe(false);
    });

    describe('click', () => {
      beforeEach(() => {
        loginButton.trigger('click');
      });

      it('will dispatch `analytics/satelliteTrack`', () => {
        expect($store.dispatch).toBeCalledWith('analytics/satelliteTrack', 'login');
      });

      it('will dispatch `preRegistrationInformation/continue`', () => {
        expect($store.dispatch).toBeCalledWith('preRegistrationInformation/continue');
      });

      it('will be disabled as button has been clicked', () => {
        expect(wrapper.vm.isButtonDisabled).toBe(true);
      });

      it('will redirect to `BEGINLOGIN_PATH`', () => {
        expect(redirectTo).toBeCalledWith(wrapper.vm, BEGINLOGIN_PATH, query);
      });
    });
  });
});
