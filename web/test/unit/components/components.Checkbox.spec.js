import Checkbox from '@/components/Checkbox';
import ErrorMessage from '@/components/widgets/ErrorMessage';
import { mount } from '../helpers';

const mountCheckbox = propsData => mount(Checkbox, {
  propsData,
  state: {
    device: {
      isNativeApp: false,
    },
  },
  $style: {
    'validation-inline': 'validation-inline',
  },
});

const createPropsData = ({ errorMessage = '', value = false, showError = false } = {}) => ({
  errorMessage,
  value,
  showError,
});

describe('Checkbox', () => {
  let wrapper;

  beforeEach(() => {
    const propsData = createPropsData();
    wrapper = mountCheckbox(propsData);
  });

  describe('error message', () => {
    const errorMessage = 'Error Message';

    describe('when showing error', () => {
      beforeEach(() => {
        const propsData = createPropsData({ errorMessage, showError: true });
        wrapper = mountCheckbox(propsData);
      });

      it('will display horizontal error bar', () => {
        expect(wrapper.find('.validation-inline').exists()).toBe(true);
      });

      it('will display error message', () => {
        expect(wrapper.find(ErrorMessage).exists()).toBe(true);
      });
    });

    describe('when not showing error', () => {
      beforeEach(() => {
        const propsData = createPropsData({ errorMessage, showError: false });
        wrapper = mountCheckbox(propsData);
      });

      it('will not display horizontal error bar', () => {
        expect(wrapper.find('.validation-inline').exists()).toBe(false);
      });

      it('will not display error message', () => {
        expect(wrapper.find(ErrorMessage).exists()).toBe(false);
      });
    });
  });
});
