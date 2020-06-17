import SessionExpiryModal from '@/components/modal/content/SessionExpiryModal';
import { mount } from '../../../helpers';

describe('SessionExpiryModal.vue', () => {
  const createModal = (
    {
      $store = {},
      propsData,
    } = {},
  ) =>
    mount(SessionExpiryModal, {
      $store,
      propsData,
    });

  describe('render modal', () => {
    it('will render session expiry modal content', () => {
      const wrapper = createModal({
        $store: {
          app: { $env: {} },
          $env: {
            SESSION_EXPIRING_WARNING_SECONDS: 300,
          },
        },
      });

      expect(wrapper.find("p[data-sid='warningDurationInformation']").text())
        .toEqual('translate_web.sessionExpiry.warningDurationInformation');

      expect(wrapper.find("button[id='modalExtendSession']").text())
        .toEqual('translate_web.sessionExpiry.warningGetMoreTime');

      expect(wrapper.find("a[id='modalExtendLogout']").text())
        .toEqual('translate_web.sessionExpiry.warningLogOut');
    });
  });

  describe('button action', () => {
    it('should dismiss modal and dispatch extends session event', () => {
      const store =
        {
          app: { $env: {} },
          $env: {
            SESSION_EXPIRING_WARNING_SECONDS: 300,
          },
          dispatch: jest.fn(),
        };

      const dispatch = jest.spyOn(store, 'dispatch');

      const page = createModal({
        $store: store,
      });

      page.vm.extendSession();

      expect(dispatch).toBeCalledWith('modal/hide');
      expect(dispatch).toHaveBeenLastCalledWith('session/extend');
    });

    it('should dismiss modal and dispatch extends session event', () => {
      const store =
        {
          app: { $env: {} },
          $env: {
            SESSION_EXPIRING_WARNING_SECONDS: 300,
          },
          dispatch: jest.fn(),
        };

      const dispatch = jest.spyOn(store, 'dispatch');

      const page = createModal({
        $store: store,
      });

      page.vm.logout();

      expect(dispatch).toBeCalledWith('modal/hide');
      expect(dispatch).toHaveBeenLastCalledWith('auth/logout');
    });
  });

  describe('sessionExpiryInMinutes', () => {
    let store;

    beforeEach(() => {
      store =
        {
          app: { $env: {} },
          $env: {
            SESSION_EXPIRING_WARNING_SECONDS: 0,
          },
        };
    });

    it('should returned 5 minutes for when environment variable is set to 300 seconds ',
      () => {
        const page = createModal({ $store: store });
        store.$env.SESSION_EXPIRING_WARNING_SECONDS = 300;
        expect(page.vm.sessionExpiryInMinutes).toEqual(5);
      });

    it('should returned 5 minutes for when environment variable is set to a 300 second string ',
      () => {
        const page = createModal({ $store: store });
        store.$env.SESSION_EXPIRING_WARNING_SECONDS = '300';
        expect(page.vm.sessionExpiryInMinutes).toEqual(5);
      });

    it('should returned 1 minutes for when environment variable is set to 60 seconds ',
      () => {
        const page = createModal({ $store: store });
        store.$env.SESSION_EXPIRING_WARNING_SECONDS = 60;
        expect(page.vm.sessionExpiryInMinutes).toEqual(1);
      });

    it('should returned 1 minutes for when environment variable is set to 30 seconds ',
      () => {
        const page = createModal({ $store: store });
        store.$env.SESSION_EXPIRING_WARNING_SECONDS = 30;
        expect(page.vm.sessionExpiryInMinutes).toEqual(1);
      });

    it('should returned 1 minutes for when environment variable is set to 0 seconds ',
      () => {
        const page = createModal({ $store: store });
        store.$env.SESSION_EXPIRING_WARNING_SECONDS = 0;
        expect(page.vm.sessionExpiryInMinutes).toEqual(1);
      });
  });
});
