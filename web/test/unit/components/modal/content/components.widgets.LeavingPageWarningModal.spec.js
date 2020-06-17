import { INDEX_PATH } from '@/router/paths';
import LeavingPageWarningModal from '@/components/modal/content/LeavingPageWarningModal';
import { mount } from '../../../helpers';

jest.mock('@/lib/utils', () =>

  ({
    redirectTo: jest.fn(),
  }));

describe('LeavingPageWarningModal.vue', () => {
  const createModal = (
    {
      $store = {},
      propsData,
    } = {},
  ) =>
    mount(LeavingPageWarningModal, {
      $store,
      propsData,
    });

  describe('render modal', () => {
    it('will render leaving page warning modal content', () => {
      const wrapper = createModal({
        $store: {
          app: { $env: {} },
          $env: {},
          state: {
            pageLeaveWarning: { attemptedRedirectRoute: INDEX_PATH },
          },
        },
      });

      expect(wrapper.find("h2[data-sid='pageLeaveWarningHeader']").text())
        .toEqual('translate_web.pageLeavingWarning.header');

      expect(wrapper.find("p[data-sid='pageLeaveWarningText']").text())
        .toEqual('translate_web.pageLeavingWarning.warning');

      expect(wrapper.find("button[id='modalStayOnPage']").text())
        .toEqual('translate_web.pageLeavingWarning.stayButtonText');

      expect(wrapper.find("a[id='modalLeavePage']").text())
        .toEqual('translate_web.pageLeavingWarning.leaveButtonText');
    });
  });

  describe('button action', () => {
    describe('user selects stay on page', () => {
      it('will stay on the page', () => {
        const store =
        {
          app: { $env: {} },
          $env: {},
          state: {
            pageLeaveWarning: { attemptedRedirectRoute: INDEX_PATH },
          },
          dispatch: jest.fn(),
        };

        const dispatch = jest.spyOn(store, 'dispatch');

        const page = createModal({
          $store: store,
        });

        page.vm.stayOnPage();

        expect(dispatch).toBeCalledWith('pageLeaveWarning/stayOnPage');
      });
    });
    describe('leave page', () => {
      it('will leave the page', () => {
        const store =
        {
          app: { $env: {} },
          $env: {},
          state: {
            pageLeaveWarning: { attemptedRedirectRoute: INDEX_PATH },
          },
          dispatch: jest.fn(),
        };

        const dispatch = jest.spyOn(store, 'dispatch');

        const page = createModal({
          $store: store,
        });

        page.vm.leavePage();

        expect(dispatch).toBeCalledWith('pageLeaveWarning/leavePage');
      });
    });
  });
});
