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
        id: 'test-id',
        name: 'test-name',
        text: 'This is a <strong>sample question</strong>?',
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
      expect(wrapper.find("[id='test-id-day']")
        .exists())
        .toEqual(true);

      expect(wrapper.find("[id='test-id-month']")
        .exists())
        .toEqual(true);

      expect(wrapper.find("[id='test-id-year']")
        .exists())
        .toEqual(true);
    });
    it('should have dated limit of 31', () => {
      const inputAttributes = wrapper.find("[id='test-id-day']").attributes();

      expect(inputAttributes.max).toEqual('31');
    });

    it('should have month limit of 12', () => {
      const inputAttributes = wrapper.find("[id='test-id-month']").attributes();

      expect(inputAttributes.max).toEqual('12');
    });

    it('date should have pattern for only numbers', () => {
      const inputAttributes = wrapper.find("[id='test-id-day']").attributes();

      expect(inputAttributes.pattern).toEqual('[0-9]*');
    });

    it('month should have pattern for only numbers', () => {
      const inputAttributes = wrapper.find("[id='test-id-month']").attributes();

      expect(inputAttributes.pattern).toEqual('[0-9]*');
    });

    it('year should have pattern for only numbers', () => {
      const inputAttributes = wrapper.find("[id='test-id-year']").attributes();

      expect(inputAttributes.pattern).toEqual('[0-9]*');
    });

    it('should emit the input if the month is changed', () => {
      const input = wrapper.find("[id='test-id-month']");
      input.element.value = 11;
      input.trigger('input');

      expect(wrapper.emitted('input')).toBeDefined();
      expect(wrapper.emitted().input[0].length).toEqual(1);
      expect(wrapper.emitted().input[0][0]).toEqual({ day: 15, month: 11, year: 2019 });
    });

    it('should emit the input if the date is changed', () => {
      const input = wrapper.find("[id='test-id-day']");
      input.element.value = 13;
      input.trigger('input');

      expect(wrapper.emitted('input')).toBeDefined();
      expect(wrapper.emitted().input[0].length).toEqual(1);
      expect(wrapper.emitted().input[0][0]).toEqual({ day: 13, month: 12, year: 2019 });
    });

    it('should emit the input if the year is changed', () => {
      const input = wrapper.find("[id='test-id-year']");
      input.element.value = 2020;
      input.trigger('input');

      expect(wrapper.emitted('input')).toBeDefined();
      expect(wrapper.emitted().input[0].length).toEqual(1);
      expect(wrapper.emitted().input[0][0]).toEqual({ day: 15, month: 12, year: 2020 });
    });
  });
});
