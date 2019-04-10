/* eslint-disable no-underscore-dangle */
import QuestionQuantity from '@/components/online-consultations/QuestionQuantity';
import { mount } from '../../helpers';

let wrapper;

describe('QuestionQuantity', () => {
  beforeEach(() => {
    const mountConfirmation = ({ propsData } = {}) =>
      mount(QuestionQuantity, {
        propsData,
        state: {
          device: {
            isNativeApp: false,
          },
        },
      });
    wrapper = mountConfirmation({
      propsData: {
        quantityId: 'quantity-input',
        selectId: 'unit-select',
        errorId: 'error-message',
        quantityLabelId: 'quantity-label',
        selectLabelId: 'unit-select-label',
        unitOptions: [{ value: 'mg' }, { value: 'kg' }, { value: 'lb' }],
        maxValue: 20,
      },
    });
  });

  describe('Data properties', () => {
    it('should be configured properly based on component props', () => {
      expect(wrapper.vm._props.quantityLabelId).toEqual('quantity-label');
      expect(wrapper.vm._props.quantityId).toEqual('quantity-input');
      expect(wrapper.vm._props.selectId).toEqual('unit-select');
      expect(wrapper.vm._props.selectLabelId).toEqual('unit-select-label');
      expect(wrapper.vm._props.unitOptions.length).toEqual(3);
    });
  });

  describe('Computed properties', () => {
    it('should emit if data is invalid', () => {
      const input = wrapper.find('input#quantity-input');
      const select = wrapper.find('#unit-select');
      const option = select.findAll('option').at(2).element;
      input.element.value = 30;
      input.trigger('input');

      option.selected = true;
      select.find('select').trigger('change');

      expect(wrapper.emitted('input')).toBeDefined();
      expect(wrapper.emitted().input[1][0].quantity).toEqual(30);
      expect(wrapper.emitted().input[1][0].unit).toEqual(option._value);
    });

    it('should emit if data is valid', () => {
      const input = wrapper.find('input#quantity-input');
      const select = wrapper.find('#unit-select');
      const option = select.findAll('option').at(1).element;
      input.element.value = 15;
      input.trigger('input');

      option.selected = true;
      select.find('select').trigger('change');

      expect(wrapper.emitted('input')).toBeDefined();
      expect(wrapper.emitted().input[1][0].quantity).toEqual(15);
      expect(wrapper.emitted().input[1][0].unit).toEqual(option._value);
    });
  });

  describe('Methods', () => {
    describe('isValidInput', () => {
      it('should evaluate true on correct fields', () => {
        const isValid = wrapper.vm.isValidInput({
          quantity: 20,
          unit: 'mg',
        });

        expect(isValid).toEqual(true);
      });

      it('should evaluate false if value exceeds max', () => {
        const isValid = wrapper.vm.isValidInput({
          value: 25,
          unit: 'kg',
        });

        expect(isValid).toEqual(false);
      });

      it('should evaluate false if there is no unit', () => {
        const isValid = wrapper.vm.isValidInput({
          value: 19,
          unit: '',
        });

        expect(isValid).toEqual(false);
      });
    });
  });
});
