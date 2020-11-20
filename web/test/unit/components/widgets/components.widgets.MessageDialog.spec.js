import MessageDialog from '@/components/widgets/MessageDialog';
import { FOCUS_ERROR_ELEMENT, EventBus } from '@/services/event-bus';
import { createStore, mount } from '../../helpers';

jest.mock('@/services/event-bus', () => ({
  ...jest.requireActual('@/services/event-bus'),
  EventBus: { $on: jest.fn(), $off: jest.fn() },
}));

let wrapper;

const mountComponent = ({
  focusable = false,
  iconText = 'Test',
  methods = undefined,
} = {}) => {
  const $store = createStore({
    state: {
      device: {
        isNativeApp: false,
      },
    },
  });

  const propsData = {
    focusable,
    iconText,
  };

  wrapper = mount(MessageDialog, { propsData, $store, methods });
  wrapper.vm.$refs.messageDialogContainer = {
    focus: jest.fn(),
    setAttribute: jest.fn(),
  };
};

describe('MessageDialog', () => {
  beforeEach(() => {
    EventBus.$on.mockClear();
    EventBus.$off.mockClear();
    window.scrollTo = jest.fn();
  });

  describe('lifecycle hooks', () => {
    it('will register FOCUS_ERROR_ELEMENT event in beforeMount', () => {
      MessageDialog.beforeMount();
      expect(EventBus.$on).toHaveBeenCalledWith(
        FOCUS_ERROR_ELEMENT, MessageDialog.scrollToTopAndFocusDialog,
      );
    });

    it('will deregister FOCUS_ERROR_ELEMENT event in beforeDestroy', () => {
      MessageDialog.beforeDestroy();
      expect(EventBus.$off).toHaveBeenCalledWith(
        FOCUS_ERROR_ELEMENT, MessageDialog.scrollToTopAndFocusDialog,
      );
    });

    it('will call focusDialog in mounted', () => {
      const focusDialog = jest.fn();

      mountComponent({ methods: { focusDialog } });

      expect(focusDialog).toHaveBeenCalled();
    });
  });

  describe('scrollToTopAndFocusDialog', () => {
    it('will scroll to top and call focusDialog', () => {
      const focusDialog = jest.fn();
      mountComponent({ methods: { focusDialog } });

      wrapper.vm.scrollToTopAndFocusDialog();

      expect(window.scrollTo).toHaveBeenCalledWith(0, 0);
      expect(focusDialog).toHaveBeenCalled();
    });
  });

  describe('focusDialog', () => {
    it('will not focus the dialog if focusable is false', () => {
      mountComponent();
      wrapper.vm.focusDialog();

      expect(wrapper.vm.$refs.messageDialogContainer.focus).not.toHaveBeenCalled();
      expect(wrapper.vm.$refs.messageDialogContainer.setAttribute).not.toHaveBeenCalled();
    });

    it('will focus the dialog if focusable is true', () => {
      mountComponent({ focusable: true });
      wrapper.vm.focusDialog();

      expect(wrapper.vm.$refs.messageDialogContainer.focus).toHaveBeenCalled();
      expect(wrapper.vm.$refs.messageDialogContainer.setAttribute).toHaveBeenCalledWith('tabindex', '-1');
    });
  });
});
