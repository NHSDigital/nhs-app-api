import GenericCheckbox from '@/components/widgets/GenericCheckbox';
import CheckboxGroup from '@/components/CheckboxGroup';
import { mount } from '../helpers';

const checkboxOptions = [
  { label: 'ONE', selected: false, code: 1 },
  { label: 'TWO', selected: false, code: 2 },
  { label: 'THREE', selected: false, code: 3 },
];

const mountComponent = ({ propsData = {}, methods = {} } = {}) =>
  mount(CheckboxGroup, {
    state: {
      device: {
        isNativeApp: false,
      },
    },
    propsData: {
      name: 'check-box-group',
      checkboxes: checkboxOptions,
      errorText: 'Error text',
      error: false,
      ...propsData,
    },
    methods,
  });

describe('Check box group', () => {
  let wrapper;

  beforeEach(() => {
    wrapper = mountComponent();
  });

  it('will display all checkboxes', () => {
    expect(wrapper.findAll(GenericCheckbox).length).toBe(3);
  });

  describe('checkbox selected', () => {
    beforeEach(() => {
      wrapper = mountComponent();
    });

    it('should emit when selected value changed is called', () => {
      const checkbox = wrapper.find('input[type="checkbox"]');
      checkbox.trigger('click');
      expect(wrapper.emitted('select')).toBeDefined();
    });

    it('should call on selected value changed on select event', () => {
      const selectedValueChanged = jest.fn();

      wrapper = mountComponent({ methods: { selectedValueChanged } });

      const checkbox = wrapper.find('input[type="checkbox"]');
      checkbox.trigger('click');
      expect(selectedValueChanged).toHaveBeenCalledTimes(1);
    });
  });
});
