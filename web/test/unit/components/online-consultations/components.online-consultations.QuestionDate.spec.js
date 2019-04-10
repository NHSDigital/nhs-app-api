/* eslint-disable no-console */
import QuestionDate from '@/components/online-consultations/QuestionDate';
import each from 'jest-each';
import { mount } from '../../helpers';

let wrapper;

const mountQuestion = ({ propsData = {} } = {}) =>
  mount(QuestionDate, {
    propsData: {
      name: 'name',
      dayId: 'dayId',
      monthId: 'monthId',
      yearId: 'yearId',
      dayName: 'dayName',
      monthName: 'monthName',
      yearName: 'yearName',
      id: 'id',
      ...propsData,
    },
  });

describe('QuestionDate.vue', () => {
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

  describe('Data properties', () => {
    it('should be properly configured based on the component properties', () => {
      /* eslint-disable no-underscore-dangle */
      expect(wrapper.vm._props.dayId).toEqual('dayId');
      expect(wrapper.vm._props.monthId).toEqual('monthId');
      expect(wrapper.vm._props.yearId).toEqual('yearId');
      expect(wrapper.vm._props.dayName).toEqual('dayName');
      expect(wrapper.vm._props.monthName).toEqual('monthName');
      expect(wrapper.vm._props.yearName).toEqual('yearName');
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
      wrapper.vm.isValidInput = jest.fn().mockImplementation(() => false);
      const input = wrapper.find("[id='dayId']");
      input.element.value = '999999999';
      input.trigger('input');

      expect(wrapper.emitted('input')).toBeDefined();
      expect(wrapper.emitted().input[0].length).toEqual(1);
      expect(wrapper.emitted().input[0][0]).toEqual(
        { day: 999999999, month: 4, year: 2019 },
      );
    });
  });

  describe('Methods', () => {
    describe('isValidInput', () => {
      it('should evaluate true on correct fields', () => {
        const isValid = wrapper.vm.isValidInput({
          day: 3,
          month: 4,
          year: 2019,
        });

        expect(isValid).toEqual(true);
      });

      it('should evaluate false on incorrect date', () => {
        const isValid = wrapper.vm.isValidInput({
          day: 3000,
          month: 4,
          year: 2019,
        });

        expect(isValid).toEqual(false);
      });

      it('should evaluate false on incorrect month', () => {
        const isValid = wrapper.vm.isValidInput({
          day: 3,
          month: 4000,
          year: 2019,
        });

        expect(isValid).toEqual(false);
      });

      it('should evaluate false on incorrect year', () => {
        const isValid = wrapper.vm.isValidInput({
          day: 3,
          month: 4000,
          year: 0,
        });

        expect(isValid).toEqual(false);
      });

      it('should evaluate false on minus figure', () => {
        const isValid = wrapper.vm.isValidInput({
          day: -3,
          month: 4,
          year: 0,
        });

        expect(isValid).toEqual(false);
      });


      each([{}, {
        day: '',
        month: '',
        year: '',
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
        year: '',
      }, {
        day: undefined,
        month: '1',
        year: undefined,
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
