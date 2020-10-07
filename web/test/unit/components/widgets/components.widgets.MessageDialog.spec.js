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
};

describe('MessageDialog', () => {
  const matchedElement = {
    blur: jest.fn(),
    setAttribute: jest.fn(),
    focus: jest.fn(),
  };

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
    beforeEach(() => {
      document.getElementById = jest.fn(() => matchedElement);
    });

    it('will not focus the dialog if focusable is false', () => {
      mountComponent();

      wrapper.vm.focusDialog();

      expect(matchedElement.focus).not.toHaveBeenCalled();
      expect(matchedElement.setAttribute).not.toHaveBeenCalled();
    });

    it('will focus the dialog if focusable is true', () => {
      mountComponent({ focusable: true });

      wrapper.vm.focusDialog();

      expect(matchedElement.focus).toHaveBeenCalled();
      expect(matchedElement.setAttribute).toHaveBeenCalled();
    });
  });
});
