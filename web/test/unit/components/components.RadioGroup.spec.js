import ErrorMessage from '@/components/widgets/ErrorMessage';
import GenericRadioButton from '@/components/widgets/GenericRadioButton';
import RadioGroup from '@/components/RadioGroup';
import { mount } from '../helpers';

const mountGroup = propsData => mount(RadioGroup, {
  propsData,
  state: {
    device: {
      isNativeApp: false,
    },
  },
});

const createPropsData = ({ header = '', errorMessage = '', showError = false } = {}) => ({
  errorMessage,
  header,
  radios: [
    { value: 'first', label: 'first label' },
    { value: 'second', label: 'second label' },
  ],
  showError,
});

describe('Radio group', () => {
  let wrapper;

  beforeEach(() => {
    const propsData = createPropsData();
    wrapper = mountGroup(propsData);
  });

  it('will use "div" to group radio buttons', () => {
    expect(wrapper.find('div.radio-group').exists()).toBe(true);
  });

  it('will display all radio buttons', () => {
    expect(wrapper.findAll(GenericRadioButton).length).toBe(2);
  });

  describe('error message', () => {
    const errorMessage = 'Error Message';

    describe('when showing error', () => {
      beforeEach(() => {
        const propsData = createPropsData({ errorMessage, showError: true });
        wrapper = mountGroup(propsData);
      });

      it('will display error message', () => {
        expect(wrapper.find(ErrorMessage).exists()).toBe(true);
      });
    });

    describe('when not showing error', () => {
      beforeEach(() => {
        const propsData = createPropsData({ errorMessage, showError: false });
        wrapper = mountGroup(propsData);
      });

      it('will not display error message', () => {
        expect(wrapper.find(ErrorMessage).exists()).toBe(false);
      });
    });
  });

  describe('with header', () => {
    let header;

    beforeEach(() => {
      header = 'Header';
      const propsData = createPropsData({ header });
      wrapper = mountGroup(propsData);
    });

    it('will use "fieldset" to group radio buttons', () => {
      expect(wrapper.find('fieldset.radio-group').exists()).toBe(true);
    });

    it('will use legend to display header', () => {
      expect(wrapper.find('legend').text()).toBe(header);
    });
  });
});
