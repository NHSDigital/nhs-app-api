import QuestionInteger from '@/components/online-consultations/QuestionText';
import GenericTextArea from '@/components/widgets/GenericTextArea';
import { shallowMount, mount } from '../../helpers';

const defaultQuestionId = 'text-question';
const defaultAnswerId = 'text-answer';

let wrapper;
const getQuestionWrapper = (id = defaultQuestionId) => wrapper.find(`label#${id}`);
const getInputWrapper = (useComponent = true) => wrapper.find(useComponent ? GenericTextArea : 'textarea');

const defaultPropsData = {
  text: '<h1>What are you asking?</h1>',
  value: 'answer',
};

const mountQuestion = ({
  propsData = defaultPropsData,
  shallow = true,
} = {}) => (shallow ? shallowMount : mount)(QuestionInteger, {
  state: {
    device: {
      isNativeApp: false,
    },
  },
  propsData,
});

describe('QuestionText.vue', () => {
  it('will verify the question is present on the page', () => {
    wrapper = mountQuestion();
    const question = getQuestionWrapper();

    expect(question.exists()).toBe(true);
  });

  describe('Answer', () => {
    beforeEach(() => {
      wrapper = mountQuestion();
    });

    it('will verify the input is present on the page', () => {
      const input = getInputWrapper();

      expect(input.exists()).toBe(true);
    });

    it('will catch events emitted by the input', () => {
      wrapper = mountQuestion({ shallow: false });
      const textAreaInput = getInputWrapper(false);

      textAreaInput.element.value = 'testString';
      textAreaInput.trigger('input');
      const inputsEmitted = wrapper.emitted().input;

      expect(wrapper.vm.textValue).toEqual('testString');
      expect(inputsEmitted.length).toBe(1);
      expect(inputsEmitted[0][0]).toEqual('testString');
    });
  });

  describe('dataProperties', () => {
    it('will verify the component properties are set correctly', () => {
      wrapper = mountQuestion();

      expect(wrapper.vm.value).toEqual(defaultPropsData.value);
      expect(wrapper.vm.text).toEqual(defaultPropsData.text);
    });
  });

  describe('Properties', () => {
    it('will set appropriate attributes on the text area component', () => {
      const textAreaInputVm = getInputWrapper().vm;

      expect(textAreaInputVm.name).toEqual(defaultAnswerId);
      expect(textAreaInputVm.id).toEqual(defaultAnswerId);
      expect(textAreaInputVm.maxlength).toEqual('255');
    });
  });
});
