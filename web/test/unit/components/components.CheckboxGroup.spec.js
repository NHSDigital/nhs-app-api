import GenericCheckbox from '@/components/widgets/GenericCheckbox';
import CheckboxGroup from '@/components/CheckboxGroup';
import { mount } from '../helpers';

const checkboxOptions = [
  { label: 'One', selected: false, code: 1 },
  { label: 'Two', selected: false, code: 2 },
  { label: 'Three', selected: false, code: 3 },
];
const htmlCheckboxOptions = [
  { label: '<span id="one">One</span>', selected: false, code: 1 },
  { label: '<span id="two">Two</span>', selected: false, code: 2 },
  { label: '<span id="three">Three</span>', selected: false, code: 3 },
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

  it('will display all checkboxes', () => {
    wrapper = mountComponent();
    expect(wrapper.findAll(GenericCheckbox).length).toBe(3);
  });

  describe('renderAsHtml', () => {
    it('will render input labels as html if renderAsHtml is true', () => {
      wrapper = mountComponent({
        propsData: {
          renderAsHtml: true,
          checkboxes: htmlCheckboxOptions,
        },
      });

      const labelOne = wrapper.find('label span#one');
      const labelTwo = wrapper.find('label span#two');
      const labelThree = wrapper.find('label span#three');

      expect(labelOne).toBeDefined();
      expect(labelTwo).toBeDefined();
      expect(labelThree).toBeDefined();
    });

    it('will not render input labels as html if renderAsHtml is false', () => {
      wrapper = mountComponent({ propsData: { renderAsHtml: false } });

      const labelOne = wrapper.find('label#checkbox-1-label');
      const labelTwo = wrapper.find('label#checkbox-2-label');
      const labelThree = wrapper.find('label#checkbox-3-label');

      expect(labelOne).toBeDefined();
      expect(labelOne.element.innerHTML).toEqual('One');
      expect(labelTwo).toBeDefined();
      expect(labelTwo.element.innerHTML).toEqual('Two');
      expect(labelThree).toBeDefined();
      expect(labelThree.element.innerHTML).toEqual('Three');
    });
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
