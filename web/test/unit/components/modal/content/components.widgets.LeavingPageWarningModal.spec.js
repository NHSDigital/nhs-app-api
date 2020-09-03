import i18n from '@/plugins/i18n';
import LeavingPageWarningModal from '@/components/modal/content/LeavingPageWarningModal';
import { INDEX_PATH } from '@/router/paths';
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
      mountOpts: {
        i18n,
      },
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
        .toEqual('Leave this page?');

      expect(wrapper.find("p[data-sid='pageLeaveWarningText']").text())
        .toEqual('If you have entered any information, it will not be saved.');

      expect(wrapper.find("button[id='modalStayOnPage']").text())
        .toEqual('Stay on this page');

      expect(wrapper.find("a[id='modalLeavePage']").text())
        .toEqual('Leave this page');
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
