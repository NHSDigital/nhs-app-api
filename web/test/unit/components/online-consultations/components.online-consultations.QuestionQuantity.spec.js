/* eslint-disable no-underscore-dangle */
import QuestionQuantity from '@/components/online-consultations/QuestionQuantity';
import { mount } from '../../helpers';
import each from 'jest-each';

let wrapper;

const defaultPropsData = {
  name: 'quantity-component',
  options: [{ label: 'mg', code: 'mg' }, { label: 'kg', code: 'kg' }, { label: 'lb', code: 'lb' }],
  maxValue: 20,
};

describe('QuestionQuantity', () => {
  const mountQuestion = ({ propsData } = {}) =>
    mount(QuestionQuantity, {
      propsData,
      state: {
        device: {
          isNativeApp: false,
        },
      },
    });
  beforeEach(() => {
    wrapper = mountQuestion({
      propsData: defaultPropsData,
    });
  });

  describe('Data properties', () => {
    it('should be configured properly based on component props', () => {
      expect(wrapper.vm._props.options.length).toEqual(3);
    });
  });

  describe('Computed properties', () => {
    it('should emit if data is invalid', () => {
      const input = wrapper.find('input#quantity-component-quantity');
      const select = wrapper.find('#quantity-component-unit');
      const option = select.findAll('option').at(2).element;
      input.element.value = 30;
      input.trigger('input');

      option.selected = true;
      select.find('select').trigger('change');

      const emittedInputs = wrapper.emitted('input');
      expect(emittedInputs).toBeDefined();
      expect(emittedInputs[0][0].quantity).toEqual('30');
      expect(emittedInputs[1][0].unit).toEqual(option._value);
    });

    it('should emit if data is valid', () => {
      const input = wrapper.find('input#quantity-component-quantity');
      const select = wrapper.find('#quantity-component-unit');
      const option = select.findAll('option').at(1).element;
      input.element.value = 15;
      input.trigger('input');

      option.selected = true;
      select.find('select').trigger('change');

      const emittedInputs = wrapper.emitted('input');
      expect(emittedInputs).toBeDefined();
      expect(emittedInputs[0][0].quantity).toEqual('15');
      expect(emittedInputs[1][0].unit).toEqual(option._value);
    });

    each([{
      required: false,
      expectedValidValues: ['mg', 'kg', 'lb'],
    }, {
      required: true,
      expectedValidValues: ['mg', 'kg', 'lb'],
    }]).it('will add undefined to validValues if answer is not required', ({ required, expectedValidValues }) => {
      wrapper = mountQuestion({
        propsData: {
          ...defaultPropsData,
          required,
        },
      });

      expect(wrapper.vm.validValues).toEqual(expectedValidValues);
    });
  });
});
