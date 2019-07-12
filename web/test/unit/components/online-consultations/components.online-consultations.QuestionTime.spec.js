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
        id: 'test-id',
        name: 'test-name',
      },
    });
    wrapper.vm.time = {
      hour: 11,
      minute: 12,
    };
  });

  describe('Data properties', () => {
    it('should be configured properly based on component data', () => {
      const timeObj = wrapper.vm.time;

      expect(timeObj.hour).toEqual(11);
      expect(timeObj.minute).toEqual(12);
    });
  });

  describe('Computed properties', () => {
    it('should emit if data is invalid', () => {
      const input = wrapper.find('input#test-id-hour');
      input.element.value = 25;
      input.trigger('input');

      expect(wrapper.emitted('input')).toBeDefined();
      expect(wrapper.emitted().input[0].length).toEqual(1);
    });
  });
});
