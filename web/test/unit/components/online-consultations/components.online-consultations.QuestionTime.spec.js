/* eslint-disable no-underscore-dangle */
import QuestionTime from '@/components/online-consultations/QuestionTime';
import { mount } from '../../helpers';

let wrapper;

describe('QuestionTime.vue', () => {
  beforeEach(() => {
    const mountConfirmation = ({ propsData } = {}) =>
      mount(QuestionTime, {
        propsData,
        state: {
          device: {
            isNativeApp: false,
          },
        },
      });
    wrapper = mountConfirmation({
      propsData: {
        hourId: 'hourId',
        minuteId: 'minuteId',
      },
    });
    wrapper.vm.time = {
      hour: 11,
      minute: 12,
    };
  });

  describe('Data properties', () => {
    it('should be configured properly based on component props', () => {
      expect(wrapper.vm._props.hourId).toEqual('hourId');
      expect(wrapper.vm._props.minuteId).toEqual('minuteId');
    });

    it('should be configured properly based on component data', () => {
      const timeObj = wrapper.vm.time;

      expect(timeObj.hour).toEqual(11);
      expect(timeObj.minute).toEqual(12);
    });
  });

  describe('Computed properties', () => {
    it('should emit if data is invalid', () => {
      const input = wrapper.find('input#hourId');
      input.element.value = 25;
      input.trigger('input');

      expect(wrapper.emitted('input')).toBeDefined();
      expect(wrapper.emitted().input[0].length).toEqual(1);
    });
  });

  describe('Methods', () => {
    describe('isValidInput', () => {
      it('should evaluate true on correct fields', () => {
        const isValid = wrapper.vm.isValidInput({
          hour: 12,
          minute: 11,
        });

        expect(isValid).toEqual(true);
      });

      it('should evaluate false on incorrect hour', () => {
        const isValid = wrapper.vm.isValidInput({
          hour: 24,
          minute: 40,
        });

        expect(isValid).toEqual(false);
      });

      it('should evaluate false on incorrect minute', () => {
        const isValid = wrapper.vm.isValidInput({
          hour: 13,
          minute: 60,
        });

        expect(isValid).toEqual(false);
      });

      it('should evaluate false for negative values', () => {
        const isValid = wrapper.vm.isValidInput({
          hour: -1,
          minute: 59,
        });

        expect(isValid).toEqual(false);
      });
    });
  });
});
