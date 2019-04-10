import each from 'jest-each';
import QuestionNumber from '@/components/online-consultations/QuestionNumber';
import GenericTextInput from '@/components/widgets/GenericTextInput';
import { mount, shallowMount } from '../../helpers';

const defaultAnswerIdName = 'number-answer';
const integerPattern = '^-?\\d+$';

const defaultPropsData = {
  min: 1,
  max: 100,
  value: 20,
  error: true,
  errorText: 'this is an error',
  questionId: 'number-question',
  answerId: 'number-answer',
  name: 'number-answer',
  required: true,
};

let wrapper;
const getInputWrapper = (useComponent = true) => wrapper.find(useComponent ? GenericTextInput : 'input[type=tel]');
const getInputWrappers = () => wrapper.findAll(GenericTextInput);

const mountQuestion = ({
  propsData = defaultPropsData,
  shallow = true,
  methods = {},
} = {}) => (shallow ? shallowMount : mount)(QuestionNumber, {
  propsData,
  state: {
    device: {
      isNativeApp: false,
    },
  },
  methods,
});

describe('QuestionNumber.vue', () => {
  describe('Lifecycle hooks', () => {
    describe('created', () => {
      it('should call checkAndEmitIsValueValid', () => {
        const checkAndEmitIsValueValid = jest.fn();
        wrapper = mountQuestion({
          methods: {
            checkAndEmitIsValueValid,
          },
        });
        expect(checkAndEmitIsValueValid).toHaveBeenCalledTimes(1);
      });
    });
  });

  describe('Data properties', () => {
    each([true, false]).it('should emit a validate event indicating whether the initial value is valid or not', (isValid) => {
      const isValidInput = jest.fn().mockReturnValue(isValid);
      wrapper = mountQuestion({
        methods: {
          isValidInput,
        },
      });
      const validateEmitted = wrapper.emitted().validate;

      expect(validateEmitted.length).toEqual(1);
      expect(validateEmitted[0][0]).toEqual(isValid);
    });
    it('should be properly configured based on the component properties', () => {
      wrapper = mountQuestion();

      expect(wrapper.vm.value).toEqual(defaultPropsData.value);
      expect(wrapper.vm.isInteger).toEqual(true);
      expect(wrapper.vm.pattern).toEqual(integerPattern);
      expect(wrapper.vm.regex).toEqual(new RegExp(integerPattern));
    });
  });

  describe('Computed properties', () => {
    describe('numberValue (with v-model support)', () => {
      it('should return the value in value prop', () => {
        wrapper = mountQuestion();

        expect(wrapper.vm.numberValue).toEqual(defaultPropsData.value);
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
      const isValidInput = jest.fn().mockImplementation(() => true);
      wrapper = mountQuestion({
        propsData: {
          ...defaultPropsData,
          type: 'decimal',
        },
        methods: {
          isValidInput,
        },
      });

      wrapper.vm.numberValue = '1.23';
      expect(wrapper.emitted().input[0][0]).toEqual('1.23');
    });
  });

  describe('Methods', () => {
    describe('checkAndEmitIsValueValid', () => {
      each([{
        isValid: true,
        value: 1,
      }, {
        isValid: false,
        value: 2,
      }]).it('should check if the value prop is valid and emit the result', (data) => {
        const isValidInput = jest.fn().mockReturnValue(data.isValid);
        wrapper = mountQuestion({
          propsData: {
            value: data.value,
          },
          methods: {
            isValidInput,
          },
        });
        expect(isValidInput).toHaveBeenCalledTimes(1);
        expect(isValidInput).toHaveBeenCalledWith(data.value);
        expect(wrapper.vm.isValid).toEqual(data.isValid);
      });
    });
    describe('isValidInput', () => {
      each([{
        input: '1.23',
        type: 'integer',
        required: true,
        expected: false,
      }, {
        input: '0',
        type: 'decimal',
        required: true,
        expected: false,
      }, {
        input: '2000',
        type: 'decimal',
        required: true,
        expected: false,
      }, {
        input: 'invalid',
        type: 'integer',
        required: true,
        expected: false,
      }, {
        input: '',
        type: 'integer',
        required: true,
        expected: false,
      }, {
        input: '1',
        type: 'integer',
        required: true,
        expected: true,
      }, {
        input: '1.23',
        type: 'decimal',
        required: true,
        expected: true,
      }, {
        input: '',
        type: 'decimal',
        required: false,
        expected: true,
      }, {
        input: '12.3',
        type: 'decimal',
        required: false,
        expected: true,
      }, {
        input: '12.3.3.3',
        type: 'decimal',
        required: false,
        expected: false,
      }, {
        input: '12.',
        type: 'decimal',
        required: false,
        expected: false,
      }]).it('should return false if the value is required, the wrong type, outside min/max range or is not a number', (data) => {
        wrapper = mountQuestion({
          propsData: {
            ...defaultPropsData,
            type: data.type,
            required: data.required,
          },
        });
        const isValid = wrapper.vm.isValidInput(data.input);

        expect(isValid).toEqual(data.expected);
      });
    });
  });

  describe('Answer', () => {
    it('should have a single input', () => {
      wrapper = mountQuestion();
      const inputWrappers = getInputWrappers();

      expect(inputWrappers.length).toEqual(1);
    });

    it('should catch input events from the answer input', () => {
      const isValidInput = jest.fn().mockImplementation(() => true);
      wrapper = mountQuestion({
        methods: {
          isValidInput,
        },
        shallow: false,
      });
      const inputWrapper = getInputWrapper(false);

      inputWrapper.element.value = 98;
      inputWrapper.trigger('input');

      const inputsEmitted = wrapper.emitted().input;

      expect(inputsEmitted.length).toEqual(1);
      expect(inputsEmitted[0][0]).toEqual('98');
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
        expect(textInputVm.type).toEqual('tel');
        expect(textInputVm.name).toEqual(defaultAnswerIdName);
        expect(textInputVm.id).toEqual(defaultAnswerIdName);
        expect(textInputVm.step).toEqual('any');
        expect(textInputVm.error).toEqual(defaultPropsData.error);
        expect(textInputVm.errorText).toEqual(defaultPropsData.errorText);
        expect(textInputVm.aLabelledBy).toEqual(defaultPropsData.questionId);
      });

      each([{
        value: 'integer',
        isValid: true,
      }, {
        value: 'decimal',
        isValid: true,
      }, {
        value: undefined,
        isValid: false,
      }, {
        value: 'random',
        isValid: false,
      }, {
        value: '',
        isValid: false,
      }]).it('should only allow type integer or decimal', (data) => {
        const { validator } = wrapper.vm.$options.props.type;

        expect(validator && validator(data.value)).toBe(data.isValid);
      });
    });
  });
});
