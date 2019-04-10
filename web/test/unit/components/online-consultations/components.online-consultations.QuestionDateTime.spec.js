/* eslint-disable no-console */
import each from 'jest-each';
import QuestionDatetime from '@/components/online-consultations/QuestionDateTime';
import { mount } from '../../helpers';

let wrapper;

const mountQuestion = ({ propsData = {} } = {}) =>
  mount(QuestionDatetime, {
    propsData: {
      name: 'name',
      dayId: 'dayId',
      monthId: 'monthId',
      yearId: 'yearId',
      hourId: 'hourId',
      minuteId: 'minuteId',
      dayName: 'dayName',
      monthName: 'monthName',
      yearName: 'yearName',
      id: 'id',
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
      /* eslint-disable no-underscore-dangle */
      expect(wrapper.vm._props.dayId).toEqual('dayId');
      expect(wrapper.vm._props.monthId).toEqual('monthId');
      expect(wrapper.vm._props.yearId).toEqual('yearId');
      expect(wrapper.vm._props.hourId).toEqual('hourId');
      expect(wrapper.vm._props.minuteId).toEqual('minuteId');
      expect(wrapper.vm._props.dayName).toEqual('dayName');
      expect(wrapper.vm._props.monthName).toEqual('monthName');
      expect(wrapper.vm._props.yearName).toEqual('yearName');
      expect(wrapper.vm._props.id).toEqual('id');
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
      wrapper.vm.isValidInput = jest.fn()
        .mockImplementation(() => false);
      const input = wrapper.find("[id='dayId']");
      input.element.value = '999999999';
      input.trigger('input');

      expect(wrapper.emitted('input')).toBeDefined();
      expect(wrapper.emitted().input[0].length).toEqual(1);
      expect(wrapper.emitted().input[0][0]).toEqual({
        day: 999999999,
        month: 4,
        year: 2019,
        hour: 10,
        minute: 40,
      });
    });

    it('should emit the input day when it is valid', () => {
      wrapper.vm.isValidInput = jest.fn()
        .mockImplementation(() => false);
      const input = wrapper.find("[id='dayId']");
      input.element.value = '10';
      input.trigger('input');

      expect(wrapper.emitted('input'))
        .toBeDefined();
      expect(wrapper.emitted().input[0].length)
        .toEqual(1);
      expect(wrapper.emitted().input[0][0])
        .toEqual(
          {
            day: 10,
            month: 4,
            year: 2019,
            hour: 10,
            minute: 40,
          },
        );
    });

    it('should emit the input  month when it is valid', () => {
      wrapper.vm.isValidInput = jest.fn()
        .mockImplementation(() => false);
      const input = wrapper.find("[id='monthId']");
      input.element.value = '10';
      input.trigger('input');

      expect(wrapper.emitted('input'))
        .toBeDefined();
      expect(wrapper.emitted().input[0].length)
        .toEqual(1);
      expect(wrapper.emitted().input[0][0])
        .toEqual(
          {
            day: 3,
            month: 10,
            year: 2019,
            hour: 10,
            minute: 40,
          },
        );
    });

    it('should emit the input year when it is valid', () => {
      wrapper.vm.isValidInput = jest.fn()
        .mockImplementation(() => false);
      const input = wrapper.find("[id='yearId']");
      input.element.value = '2010';
      input.trigger('input');

      expect(wrapper.emitted('input'))
        .toBeDefined();
      expect(wrapper.emitted().input[0].length)
        .toEqual(1);
      expect(wrapper.emitted().input[0][0])
        .toEqual(
          {
            day: 3,
            month: 4,
            year: 2010,
            hour: 10,
            minute: 40,
          },
        );
    });

    it('should emit the input hour when it is valid', () => {
      wrapper.vm.isValidInput = jest.fn()
        .mockImplementation(() => false);
      const input = wrapper.find("[id='hourId']");
      input.element.value = '10';
      input.trigger('input');

      expect(wrapper.emitted('input'))
        .toBeDefined();
      expect(wrapper.emitted().input[0].length)
        .toEqual(1);
      expect(wrapper.emitted().input[0][0])
        .toEqual(
          {
            day: 3,
            month: 4,
            year: 2019,
            hour: 10,
            minute: 40,
          },
        );
    });

    it('should emit the input minute when it is valid', () => {
      wrapper.vm.isValidInput = jest.fn()
        .mockImplementation(() => false);
      const input = wrapper.find("[id='minuteId']");
      input.element.value = '10';
      input.trigger('input');

      expect(wrapper.emitted('input'))
        .toBeDefined();
      expect(wrapper.emitted().input[0].length)
        .toEqual(1);
      expect(wrapper.emitted().input[0][0])
        .toEqual(
          {
            day: 3,
            month: 4,
            year: 2019,
            hour: 10,
            minute: 10,
          },
        );
    });

    each([{
      id: undefined,
      errorId: 'datetime-answer-error-message',
    }, {
      id: 'myid',
      errorId: 'myid-error-message',
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

  describe('Methods', () => {
    describe('isValidInput', () => {
      it('should evaluate true on correct fields', () => {
        const isValid = wrapper.vm.isValidInput({
          day: 3,
          month: 4,
          year: 2019,
          hour: 10,
          minute: 30,
        });

        expect(isValid).toEqual(true);
      });

      it('should evaluate false on incorrect date', () => {
        const isValid = wrapper.vm.isValidInput({
          day: 3000,
          month: 4,
          year: 2019,
          hour: 10,
          minute: 30,
        });

        expect(isValid).toEqual(false);
      });

      it('should evaluate false on incorrect month', () => {
        const isValid = wrapper.vm.isValidInput({
          day: 3,
          month: 4000,
          year: 2019,
          hour: 10,
          minute: 30,
        });

        expect(isValid).toEqual(false);
      });

      it('should evaluate false on incorrect year', () => {
        const isValid = wrapper.vm.isValidInput({
          day: 3,
          month: 4000,
          year: 0,
          hour: 10,
          minute: 30,
        });

        expect(isValid).toEqual(false);
      });

      it('should evaluate false on incorrect hour', () => {
        const isValid = wrapper.vm.isValidInput({
          day: 3,
          month: 4000,
          year: 0,
          hour: 10000,
          minute: 30,
        });

        expect(isValid).toEqual(false);
      });

      it('should evaluate false on incorrect minute', () => {
        const isValid = wrapper.vm.isValidInput({
          day: 3,
          month: 4000,
          year: 0,
          hour: 10,
          minute: 30000,
        });

        expect(isValid).toEqual(false);
      });

      it('should evaluate false on minus figure', () => {
        const isValid = wrapper.vm.isValidInput({
          day: -3,
          month: 4,
          year: 0,
          hour: 10,
          minute: 30,
        });

        expect(isValid).toEqual(false);
      });

      each([{}, {
        day: '',
        month: '',
        year: '',
        hour: '',
        minute: '',
      }]).it('should return true if not required and all fields are empty or undefined', (datetime) => {
        wrapper = mountQuestion({
          propsData: {
            required: false,
          },
        });
        expect(wrapper.vm.isValidInput(datetime)).toEqual(true);
      });

      each([{
        day: '1',
        month: undefined,
        year: '3',
        hour: '',
        minute: undefined,
      }, {
        day: undefined,
        month: '1',
        year: undefined,
        hour: '3',
        minute: '',
      }]).it('should return false if not required and not all fields are empty or undefined', (datetime) => {
        wrapper = mountQuestion({
          propsData: {
            required: false,
          },
        });
        expect(wrapper.vm.isValidInput(datetime)).toEqual(false);
      });
    });
  });
});
