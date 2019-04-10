import GenericDateInput from '@/components/widgets/GenericDateInput';
import { mount } from '../../helpers';

const state = {
  device: {
    isNativeApp: false,
  },
};

let wrapper;

describe('GenericDateInput.vue', () => {
  beforeEach(() => {
    const mountConfirmation = ({ propsData } = {}) =>
      mount(GenericDateInput, {
        propsData,
        state,
      });
    wrapper = mountConfirmation({
      propsData: {
        name: 'name',
        questionId: 'qid',
        text: 'This is a <strong>sample question</strong>?',
        dayId: 'dayId',
        monthId: 'monthId',
        yearId: 'yearId',
        dayName: 'dayName',
        monthName: 'monthName',
        yearName: 'yearName',
        value:
          {
            day: 15,
            month: 12,
            year: 2019,
          },
      },
    });
  });

  describe('input', () => {
    it('should have all parts of date', () => {
      expect(wrapper.find("[id='dayId']")
        .exists())
        .toEqual(true);

      expect(wrapper.find("[id='monthId']")
        .exists())
        .toEqual(true);

      expect(wrapper.find("[id='yearId']")
        .exists())
        .toEqual(true);
    });
    it('should have dated limit of 31', () => {
      const inputAttributes = wrapper.find("[id='dayId']").attributes();

      expect(inputAttributes.max).toEqual('31');
    });

    it('should have month limit of 12', () => {
      const inputAttributes = wrapper.find("[id='monthId']").attributes();

      expect(inputAttributes.max).toEqual('12');
    });

    it('date should have pattern for only numbers', () => {
      const inputAttributes = wrapper.find("[id='dayId']").attributes();

      expect(inputAttributes.pattern).toEqual('[0-9]*');
    });

    it('month should have pattern for only numbers', () => {
      const inputAttributes = wrapper.find("[id='monthId']").attributes();

      expect(inputAttributes.pattern).toEqual('[0-9]*');
    });

    it('year should have pattern for only numbers', () => {
      const inputAttributes = wrapper.find("[id='yearId']").attributes();

      expect(inputAttributes.pattern).toEqual('[0-9]*');
    });

    it('should emit the input if the month is changed', () => {
      const input = wrapper.find("[id='monthId']");
      input.element.value = 11;
      input.trigger('input');

      expect(wrapper.emitted('input')).toBeDefined();
      expect(wrapper.emitted().input[0].length).toEqual(1);
      expect(wrapper.emitted().input[0][0]).toEqual({ day: 15, month: 11, year: 2019 });
    });

    it('should emit the input if the date is changed', () => {
      const input = wrapper.find("[id='dayId']");
      input.element.value = 13;
      input.trigger('input');

      expect(wrapper.emitted('input')).toBeDefined();
      expect(wrapper.emitted().input[0].length).toEqual(1);
      expect(wrapper.emitted().input[0][0]).toEqual({ day: 13, month: 12, year: 2019 });
    });

    it('should emit the input if the year is changed', () => {
      const input = wrapper.find("[id='yearId']");
      input.element.value = 2020;
      input.trigger('input');

      expect(wrapper.emitted('input')).toBeDefined();
      expect(wrapper.emitted().input[0].length).toEqual(1);
      expect(wrapper.emitted().input[0][0]).toEqual({ day: 15, month: 12, year: 2020 });
    });
  });
});
