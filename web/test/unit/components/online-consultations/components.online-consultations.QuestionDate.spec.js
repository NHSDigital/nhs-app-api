/* eslint-disable no-console */
import QuestionDate from '@/components/online-consultations/QuestionDate';
import { mount } from '../../helpers';

let wrapper;

const mountQuestion = ({ propsData = {} } = {}) =>
  mount(QuestionDate, {
    propsData: {
      id: 'test-id',
      name: 'test-name',
      ...propsData,
    },
  });

describe('QuestionDate.vue', () => {
  describe('default props', () => {
    it('should be properly configured based on the component properties', () => {
      wrapper = mountQuestion();

      expect(wrapper.vm.id).toEqual('test-id');
      expect(wrapper.vm.name).toEqual('test-name');
      expect(wrapper.vm.value).toEqual({});
    });
  });

  describe('Data properties', () => {
    beforeEach(() => {
      wrapper = mountQuestion({
        propsData: {
          value: {
            day: 3,
            month: 4,
            year: 2019,
          },
        },
      });
    });

    it('should be properly configured based on the component data', () => {
      const dateObj = wrapper.vm.dateValue;

      expect(dateObj.day).toBe(3);
      expect(dateObj.month).toBe(4);
      expect(dateObj.year).toBe(2019);
    });
  });

  describe('Computed properties', () => {
    it('should emit the input date when it is invalid', () => {
      // Arrange
      const checkAndEmitIsValueValid = jest.fn().mockImplementation(() => false);
      wrapper = mountQuestion();
      wrapper.vm.checkAndEmitIsValueValid = checkAndEmitIsValueValid;
      const emittedValue = { day: 999999999 };
      const input = wrapper.find("[id='test-id-day']");

      // Act
      input.element.value = '999999999';
      input.trigger('input');

      // Assert
      const emittedInputs = wrapper.emitted('input');
      expect(checkAndEmitIsValueValid).toHaveBeenCalledWith(emittedValue);
      expect(emittedInputs).toBeDefined();
      expect(emittedInputs[0].length).toBe(1);
      expect(emittedInputs[0][0]).toEqual(emittedValue);
    });
  });
});
