import BiometricBanner from '@/components/widgets/BiometricBanner';
import { initialState } from '@/store/modules/biometricBanner/mutation-types';
import { createStore, mount } from '../../helpers';

const mountBiometricBanner = $store => mount(BiometricBanner, {
  $store,
  $style: {
    info: 'info',
  },
});

describe('BiometricBanner', () => {
  let $store;
  let state;
  let wrapper;

  beforeEach(() => {
    state = {
      device: {
        isNativeApp: true,
      },
      biometricBanner: initialState(),
    };

    $store = createStore({ state });
    wrapper = mountBiometricBanner($store);
  });

  describe('is native', () => {
    beforeEach(() => {
      state.device.isNativeApp = true;
    });

    it('will not be visible if dismissed', () => {
      state.biometricBanner.dismissed = true;
      expect(wrapper.find('div').exists()).toBe(false);
    });

    describe('not dismissed', () => {
      beforeEach(() => {
        state.biometricBanner.dismissed = false;
      });

      it('will be visible', () => {
        expect(wrapper.find('div').exists()).toBe(true);
      });

      describe('dismiss button', () => {
        let dismissButton;

        beforeEach(() => {
          dismissButton = wrapper.find('#btn_biometricBannerDismiss');
        });

        it('will be visible', () => {
          expect(dismissButton.exists()).toBe(true);
        });

        it('will dispatch `biometricBanner/dismiss` when clicked', () => {
          global.digitalData = {};
          dismissButton.trigger('click');
          expect($store.dispatch).toBeCalledWith('biometricBanner/dismiss');
        });
      });
    });
  });

  describe('is not native', () => {
    beforeEach(() => {
      state.device.isNativeApp = false;
    });

    it('will not be visible if dismissed', () => {
      state.biometricBanner.dismissed = true;
      expect(wrapper.find('div').exists()).toBe(false);
    });

    it('will not be visible if not dismissed', () => {
      state.biometricBanner.dismissed = false;
      expect(wrapper.find('div').exists()).toBe(false);
    });
  });
});
