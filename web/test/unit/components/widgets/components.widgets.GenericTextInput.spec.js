import GenericTextInput from '@/components/widgets/GenericTextInput';
import each from 'jest-each';
import { mount } from '../../helpers';

const state = {
  device: {
    isNativeApp: false,
  },
};

let wrapper;

const mountComponent = ({ propsData } = {}) => mount(GenericTextInput, {
  state,
  propsData,
});

describe('GenericTextInput.vue', () => {
  describe('input', () => {
    it('should have v-model support', () => {
      wrapper = mountComponent({
        propsData: {
          value: '12',
        },
      });

      expect(wrapper.vm.textValue).toEqual('12');

      wrapper.vm.textValue = 'this is a new value';
      const inputsEmitted = wrapper.emitted().input;

      expect(inputsEmitted.length).toEqual(1);
      expect(inputsEmitted[0][0]).toEqual('this is a new value');
    });

    it('should appropriately set aria label based on error state and property', () => {
      wrapper = mountComponent({
        propsData: {
          error: true,
          id: 'question',
          errorText: 'errorMessage',
          aLabelledBy: 'testAriaLabel',
        },
      });
      const inputAttributes = wrapper.find('input').attributes();

      expect(inputAttributes['aria-labelledby']).toEqual('testAriaLabel question-error-message');
    });

    it('should appropriately set aria described-by based on error state and property', () => {
      wrapper = mountComponent({
        propsData: {
          error: true,
          required: false,
          id: 'question',
          errorText: 'errorMessage',
          aDescribedBy: 'testAriaLabel',
        },
      });
      const inputAttributes = wrapper.find('input').attributes();

      expect(inputAttributes['aria-describedby']).toEqual('testAriaLabel question-error-message');
    });

    it('should allow text limit', () => {
      wrapper = mountComponent({
        propsData: {
          maxlength: '25',
        },
      });
      const inputAttributes = wrapper.find('input').attributes();

      expect(inputAttributes.maxlength).toEqual('25');
    });

    it('should allow type to be number', () => {
      wrapper = mountComponent({
        propsData: {
          type: 'number',
          min: 1,
          max: 20,
          step: 'any',
        },
      });

      const inputAttributes = wrapper.find('input').attributes();

      expect(inputAttributes.type).toEqual('number');
      expect(inputAttributes.min).toEqual('1');
      expect(inputAttributes.max).toEqual('20');
      expect(inputAttributes.step).toEqual('any');
    });

    it('should allow pattern matching validation', () => {
      wrapper = mountComponent({
        propsData: {
          pattern: 'testPattern',
        },
      });
      const inputAttributes = wrapper.find('input').attributes();

      expect(inputAttributes.pattern).toEqual('testPattern');
    });

    it('should disable autocorrect', () => {
      wrapper = mountComponent();

      const inputAttributes = wrapper.find('input').attributes();

      expect(inputAttributes.autocorrect).toEqual('off');
      expect(inputAttributes.autocapitalize).toEqual('off');
      expect(inputAttributes.autocomplete).toEqual('off');
      expect(inputAttributes.spellcheck).toEqual('false');
    });

    it('should have error style if there is an error', () => {
      wrapper = mountComponent({
        propsData: {
          error: true,
        },
      });

      const inputWithErrorStyle = wrapper.find('input.nhsuk-input--error');

      expect(inputWithErrorStyle).toBeDefined();
    });

    each([true, false])
      .it('should only show an error if error is true', (error) => {
        wrapper = mountComponent({
          propsData: {
            error,
          },
        });

        const inputWithErrorStyle = wrapper.find('span.nhsuk-error-message').element;

        expect(inputWithErrorStyle !== undefined).toEqual(error);
      });
  });
});
