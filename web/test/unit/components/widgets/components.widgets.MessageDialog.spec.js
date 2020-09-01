import MessageDialog from '@/components/widgets/MessageDialog';
import { createStore, mount } from '../../helpers';


const createComponent = ({ focusable = false, iconText = 'Test' } = {}) => {
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

  mount(MessageDialog, { propsData, $store });
};

describe('MessageDialog', () => {
  const object = {
    blur: jest.fn(),
    setAttribute: jest.fn(),
    focus: jest.fn(),
  };

  it('will not call setScreenReaderFocus if focusable is false', () => {
    const getElementById = jest.fn(() => object);
    document.getElementById = getElementById;
    createComponent();
    expect(object.blur).not.toHaveBeenCalled();
    expect(object.focus).not.toHaveBeenCalled();
    expect(object.setAttribute).not.toHaveBeenCalled();
  });

  it('will call setScreenReaderFocus if focusable is true', () => {
    const getElementById = jest.fn(() => object);
    document.getElementById = getElementById;
    createComponent({ focusable: true });
    expect(object.blur).toHaveBeenCalled();
    expect(object.focus).toHaveBeenCalled();
    expect(object.setAttribute).toHaveBeenCalled();
  });
});
