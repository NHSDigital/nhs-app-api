/* eslint-disable no-console */
import each from 'jest-each';
import QuestionDatetime from '@/components/online-consultations/QuestionDateTime';
import { mount } from '../../helpers';

let wrapper;

const mountQuestion = ({ propsData = {} } = {}) =>
  mount(QuestionDatetime, {
    propsData: {
      id: 'test-id',
      name: 'test-name',
      ...propsData,
    },
    state: {
      device: {
        isNativeApp: false,
      },
    },
  });
describe('QuestionDateTime.vue', () => {
  beforeEach(() => {
    wrapper = mountQuestion({
      propsData: {
        value: {
          day: 3,
          month: 4,
          year: 2019,
          hour: 10,
          minute: 40,
        },
      },
    });
  });
  describe('Data properties', () => {
    it('should be properly configured based on the component properties', () => {
      expect(wrapper.vm.$props.id).toEqual('test-id');
      expect(wrapper.vm.$props.name).toEqual('test-name');
    });
    it('should be properly configured based on the component data', () => {
      const dateObj = wrapper.vm.dateTimeValue;

      expect(dateObj.day).toBe(3);
      expect(dateObj.month).toBe(4);
      expect(dateObj.year).toBe(2019);
      expect(dateObj.hour).toBe(10);
      expect(dateObj.minute).toBe(40);
    });
  });

  describe('Computed properties', () => {
    it('should emit the input date when it is invalid', () => {
      const input = wrapper.find("[id='test-id-day']");
      input.element.value = '999999999';
      input.trigger('input');

      const emittedInputs = wrapper.emitted('input');
      expect(emittedInputs).toBeDefined();
      expect(emittedInputs[0].length).toEqual(1);
      expect(emittedInputs[0][0]).toEqual({
        day: 999999999,
        month: 4,
        year: 2019,
        hour: 10,
        minute: 40,
      });
    });

    it('should emit the input day when it is valid', () => {
      const input = wrapper.find("[id='test-id-day']");
      input.element.value = '10';
      input.trigger('input');

      const emittedInputs = wrapper.emitted('input');
      expect(emittedInputs).toBeDefined();
      expect(emittedInputs[0].length).toEqual(1);
      expect(emittedInputs[0][0]).toEqual({
        day: 10,
        month: 4,
        year: 2019,
        hour: 10,
        minute: 40,
      });
    });

    it('should emit the input  month when it is valid', () => {
      const input = wrapper.find("[id='test-id-month']");
      input.element.value = '10';
      input.trigger('input');
      const emittedInputs = wrapper.emitted('input');
      expect(emittedInputs).toBeDefined();
      expect(emittedInputs[0].length).toEqual(1);
      expect(emittedInputs[0][0]).toEqual({
        day: 3,
        month: 10,
        year: 2019,
        hour: 10,
        minute: 40,
      });
    });

    it('should emit the input year when it is valid', () => {
      const input = wrapper.find("[id='test-id-year']");
      input.element.value = '2010';
      input.trigger('input');

      const emittedInputs = wrapper.emitted('input');
      expect(emittedInputs).toBeDefined();
      expect(emittedInputs[0].length).toEqual(1);
      expect(emittedInputs[0][0]).toEqual({
        day: 3,
        month: 4,
        year: 2010,
        hour: 10,
        minute: 40,
      });
    });

    it('should emit the input hour when it is valid', () => {
      const input = wrapper.find("[id='test-id-hour']");
      input.element.value = '10';
      input.trigger('input');

      const emittedInputs = wrapper.emitted('input');
      expect(emittedInputs).toBeDefined();
      expect(emittedInputs[0].length).toEqual(1);
      expect(emittedInputs[0][0]).toEqual({
        day: 3,
        month: 4,
        year: 2019,
        hour: 10,
        minute: 40,
      });
    });

    it('should emit the input minute when it is valid', () => {
      const input = wrapper.find("[id='test-id-minute']");
      input.element.value = '10';
      input.trigger('input');

      const emittedInputs = wrapper.emitted('input');
      expect(emittedInputs).toBeDefined();
      expect(emittedInputs[0].length).toEqual(1);
      expect(emittedInputs[0][0]).toEqual({
        day: 3,
        month: 4,
        year: 2019,
        hour: 10,
        minute: 10,
      });
    });

    each([{
      id: 'test-id',
      errorId: 'test-id-error-message',
    }, {
      id: '',
      errorId: 'error-message',
    }]).it('should set an appropriate error id using the id prop', (data) => {
      wrapper = mountQuestion({
        propsData: {
          id: data.id,
        },
      });

      expect(wrapper.vm.errorId).toEqual(data.errorId);
    });
  });
});
