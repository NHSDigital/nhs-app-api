import QuestionString from '@/components/online-consultations/QuestionString';
import { mount } from '../../helpers';
import GenericTextInput from '../../../../src/components/widgets/GenericTextInput';


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
        questionId: 'qid',
        text: 'This is a <strong>sample question</strong>?',
      },
    });
  });

  describe('Question', () => {
    it('should have a question element', () => {
      expect(wrapper.find("[id='qid']").exists()).toBe(true);
    });

    it('should have an element to render the question html', () => {
      expect(wrapper.find("[id='qid']").element.innerHTML)
        .toEqual('This is a <strong>sample question</strong>?');
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

      expect(wrapper.emitted('input')).toBeDefined();
      expect(wrapper.emitted().input[0].length).toEqual(1);
      expect(wrapper.emitted().input[0][0]).toEqual('test');
    });

    it('has a type of text', async () => {
      const input = wrapper.find('input');
      expect(input.attributes('type').type).toBe('text');
    });
  });
});
