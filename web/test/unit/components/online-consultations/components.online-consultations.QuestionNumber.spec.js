/* eslint-disable no-console */
import each from 'jest-each';
import QuestionNumber from '@/components/online-consultations/QuestionNumber';
import GenericTextInput from '@/components/widgets/GenericTextInput';
import { mount, shallowMount } from '../../helpers';

const defaultQuestionId = 'number-question';
const defaultAnswerIdName = 'number-answer';
const testQuestionId = 'test-question-id';
const integerPattern = '^-?\\d+$';

const defaultPropsData = {
  text: 'This is a <strong>question?</strong>',
  min: 1,
  max: 100,
  value: 20,
  error: true,
  errorText: 'this is an error',
  questionId: 'number-question',
  answerId: 'number-answer',
  name: 'number-answer',
};

let wrapper;
const getQuestionWrapper = (id = defaultQuestionId) => wrapper.find(`label#${id}`);
const getInputWrapper = (useComponent = true) => wrapper.find(useComponent ? GenericTextInput : 'input[type=number]');
const getInputWrappers = () => wrapper.findAll(GenericTextInput);

const mountQuestion = ({
  propsData = defaultPropsData,
  shallow = true,
} = {}) => (shallow ? shallowMount : mount)(QuestionNumber, {
  propsData,
  state: {
    device: {
      isNativeApp: false,
    },
  },
});

describe('QuestionNumber.vue', () => {
  describe('Data properties', () => {
    it('should be properly configured based on the component properties', () => {
      wrapper = mountQuestion();
      wrapper.vm.isValidInput = jest.fn().mockReturnValue(true);

      expect(wrapper.vm.value).toEqual(defaultPropsData.value);
      expect(wrapper.vm.isInteger).toEqual(true);
      expect(wrapper.vm.hasErrored).toEqual(true);
      expect(wrapper.vm.isValid).toEqual(true);
      expect(wrapper.vm.pattern).toEqual(integerPattern);
      expect(wrapper.vm.regex).toEqual(new RegExp(integerPattern));
    });
  });

  describe('Computed properties', () => {
    describe('formGroupClasses', () => {
      each([{
        hasErrored: false,
        classes: ['nhsuk-form-group', undefined],
      }, {
        hasErrored: true,
        classes: ['nhsuk-form-group', 'nhsuk-form-group--error'],
      }]).it('should return appropriate classes based on if the error should be shown', (data) => {
        wrapper = mountQuestion();
        wrapper.vm.hasErrored = data.hasErrored;

        expect(wrapper.vm.formGroupClasses).toEqual(data.classes);
      });
    });

    describe('showError', () => {
      each([true, false]).it('should return whether the validation has errored', (hasErrored) => {
        wrapper = mountQuestion();
        wrapper.vm.hasErrored = hasErrored;

        expect(wrapper.vm.showError).toEqual(hasErrored);
      });
    });

    describe('numberValue (with v-model support)', () => {
      it('should return the value in value prop', () => {
        wrapper = mountQuestion();

        expect(wrapper.vm.numberValue).toEqual(defaultPropsData.value);
      });
      it('should call isValidInput and assign the result to isValid', () => {
        wrapper = mountQuestion();
        wrapper.vm.isValidInput = jest.fn().mockReturnValue(true);

        wrapper.vm.numberValue = '222';

        expect(wrapper.vm.isValid).toEqual(true);
        expect(wrapper.vm.isValidInput).toHaveBeenCalled();
      });
      it('should emit an input event on set with the new value', () => {
        wrapper = mountQuestion();
        wrapper.vm.numberValue = '222';
        const inputsEmitted = wrapper.emitted().input;

        expect(inputsEmitted.length).toEqual(1);
        expect(inputsEmitted[0][0]).toEqual('222');
      });
    });

    it('should only emit an decimal when type is decimal', () => {
      wrapper = mountQuestion({
        propsData: {
          ...defaultPropsData,
          type: 'decimal',
        },
      });
      wrapper.vm.isValidInput = jest.fn().mockImplementation(() => true);
      wrapper.vm.numberValue = '1.23';
      expect(wrapper.emitted().input[0][0]).toEqual('1.23');
    });
  });

  describe('Methods', () => {
    describe('isValidInput', () => {
      each([{
        input: '1.23',
        type: 'integer',
        expected: false,
      }, {
        input: '0',
        type: 'decimal',
        expected: false,
      }, {
        input: '2000',
        type: 'decimal',
        expected: false,
      }, {
        input: 'invalid',
        type: 'integer',
        expected: false,
      }, {
        input: '',
        type: 'integer',
        expected: false,
      }, {
        input: '1',
        type: 'integer',
        expected: true,
      }, {
        input: '1.23',
        type: 'decimal',
        expected: true,
      }]).it('should return false if the value is the wrong type, outside min/max range or is not a number', (data) => {
        wrapper = mountQuestion({
          propsData: {
            ...defaultPropsData,
            type: data.type,
          },
        });
        const isValid = wrapper.vm.isValidInput(data.input);

        expect(isValid).toEqual(data.expected);
      });
    });

    describe('validate', () => {
      each([true, false]).it('should set hasErrored to the inverse of isValid', (isValid) => {
        wrapper = mountQuestion();
        wrapper.vm.isValid = isValid;
        wrapper.vm.validate();

        expect(wrapper.vm.hasErrored).toEqual(!isValid);
      });
    });
  });

  describe('Question', () => {
    it('should have an element to render the question html', () => {
      wrapper = mountQuestion();
      const questionWrapper = getQuestionWrapper();

      expect(questionWrapper.exists()).toEqual(true);
      expect(questionWrapper.element.innerHTML).toEqual(defaultPropsData.text);
    });

    it('should allow an id to be specified for the question element', () => {
      wrapper = mountQuestion({
        propsData: {
          ...defaultPropsData,
          questionId: testQuestionId,
        },
      });
      const questionWrapper = getQuestionWrapper(testQuestionId);

      expect(questionWrapper.exists()).toEqual(true);
      expect(questionWrapper.element.innerHTML).toEqual(defaultPropsData.text);
    });
  });

  describe('Answer', () => {
    it('should have a single input', () => {
      wrapper = mountQuestion();
      const inputWrappers = getInputWrappers();

      expect(inputWrappers.length).toEqual(1);
    });

    it('should catch input events from the answer input', () => {
      wrapper = mountQuestion({ shallow: false });
      const inputWrapper = getInputWrapper(false);
      wrapper.vm.isValidInput = jest.fn().mockImplementation(() => true);

      inputWrapper.element.value = 98;
      inputWrapper.trigger('input');

      const inputsEmitted = wrapper.emitted().input;

      expect(inputsEmitted.length).toEqual(1);
      expect(inputsEmitted[0][0]).toEqual(98);
    });

    describe('Properties', () => {
      it('should set appropriate attributes on the answer input', () => {
        wrapper = mountQuestion({
          shallow: false,
        });
        const textInputVm = getInputWrapper().vm;

        expect(textInputVm.value).toEqual(defaultPropsData.value);
        expect(textInputVm.min).toEqual(defaultPropsData.min);
        expect(textInputVm.max).toEqual(defaultPropsData.max);
        expect(textInputVm.type).toEqual('number');
        expect(textInputVm.name).toEqual(defaultAnswerIdName);
        expect(textInputVm.id).toEqual(defaultAnswerIdName);
        expect(textInputVm.step).toEqual('any');
        expect(textInputVm.error).toEqual(defaultPropsData.error);
        expect(textInputVm.errorText).toEqual(defaultPropsData.errorText);
        expect(textInputVm.aLabelledBy).toEqual(defaultPropsData.questionId);
      });
      each(['integer', 'decimal', 'random']).it('should only allow integer or decimal types', (type) => {
        const shouldError = ['integer', 'decimal'].indexOf(type) === -1;
        console.error = jest.fn();

        wrapper = mountQuestion({
          propsData: {
            ...defaultPropsData,
            type,
          },
        });

        if (shouldError) {
          expect(console.error).toHaveBeenCalled();
        } else {
          expect(console.error).not.toHaveBeenCalled();
        }
      });
    });
  });
});
