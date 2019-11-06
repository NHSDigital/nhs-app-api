// import each from 'jest-each';
import QuestionInteger from '@/components/online-consultations/QuestionText';
import GenericTextArea from '@/components/widgets/GenericTextArea';
import { questionTextAnswerValid } from '@/lib/online-consultations/answer-validators';
import { shallowMount, mount } from '../../helpers';

jest.mock('@/lib/online-consultations/answer-validators');

const defaultAnswerId = 'text-answer';

let wrapper;
const getInputWrapper = (useComponent = true) => wrapper.find(useComponent ? GenericTextArea : 'textarea');

const defaultPropsData = {
  value: 'answer',
  maxLength: '20',
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

      expect(inputsEmitted.length).toBe(1);
      expect(inputsEmitted[0][0]).toEqual('testString');
    });
  });

  describe('dataProperties', () => {
    it('will verify the component properties are set correctly', () => {
      wrapper = mountQuestion();

      expect(wrapper.vm.value).toEqual(defaultPropsData.value);
    });
  });

  describe('Properties', () => {
    it('will set appropriate attributes on the text area component', () => {
      const textAreaInputVm = getInputWrapper().vm;

      expect(textAreaInputVm.name).toEqual(defaultAnswerId);
      expect(textAreaInputVm.id).toEqual(defaultAnswerId);
      expect(textAreaInputVm.maxlength).toEqual('20');
    });
  });

  describe('methods', () => {
    describe('checkAndEmitIsValueValid', () => {
      beforeAll(() => {
        questionTextAnswerValid.mockClear();
      });
      it('will validate the answer and emit the result', () => {
        // Arrange
        questionTextAnswerValid.mockReturnValue({ valid: false, message: 'too long' });
        wrapper = mountQuestion();

        // Act
        wrapper.vm.checkAndEmitIsValueValid('an answer longer than 20 characters');

        // Assert
        expect(questionTextAnswerValid).toHaveBeenCalledWith('an answer longer than 20 characters', true, '20');
        expect(wrapper.emitted('validate')[1][0]).toEqual({ valid: false, message: 'too long' });
      });
    });
  });
});
