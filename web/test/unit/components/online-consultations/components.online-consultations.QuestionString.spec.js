import QuestionString from '@/components/online-consultations/QuestionString';
import GenericTextInput from '@/components/widgets/GenericTextInput';
import { mount } from '../../helpers';


let wrapper;
const getInputWrapper = (useComponent = true) => wrapper.find(useComponent ? GenericTextInput : 'input[type=text]');


describe('QuestionString.vue', () => {
  beforeEach(() => {
    const mountConfirmation = ({ propsData } = {}) =>
      mount(QuestionString, {
        propsData,
        state: {
          device: {
            isNativeApp: false,
          },
        },
      });
    wrapper = mountConfirmation({
      propsData: {
        name: 'name',
      },
    });
  });

  describe('Answer', () => {
    it('should have an input', () => {
      const inputWrapper = getInputWrapper();

      expect(inputWrapper.exists()).toBe(true);
    });

    it('should emit an input event', () => {
      const input = wrapper.find('input');
      input.element.value = 'test';
      input.trigger('input');

      const emittedInputs = wrapper.emitted('input');
      expect(emittedInputs).toBeDefined();
      expect(emittedInputs[0].length).toEqual(1);
      expect(emittedInputs[0][0]).toEqual('test');
    });

    it('has a type of text', async () => {
      const input = wrapper.find('input');
      expect(input.attributes().type).toBe('text');
    });
  });
});
