import QuestionString from '@/components/online-consultations/QuestionString';
import GenericTextInput from '@/components/widgets/GenericTextInput';
import { questionStringAnswerValid } from '@/lib/online-consultations/answer-validators';
import { mount } from '../../helpers';

jest.mock('@/lib/online-consultations/answer-validators');

let wrapper;
const getInputWrapper = (useComponent = true) => wrapper.find(useComponent ? GenericTextInput : 'input[type=text]');

const mountQuestion = ({ propsData } = {}) =>
  mount(QuestionString, {
    propsData,
    state: {
      device: {
        isNativeApp: false,
      },
    },
  });

describe('QuestionString.vue', () => {
  beforeEach(() => {
    wrapper = mountQuestion({
      propsData: {
        name: 'name',
        maxLength: '20',
      },
    });
  });

  describe('Answer', () => {
    it('should have an input', () => {
      const inputWrapper = getInputWrapper();

      expect(inputWrapper.exists()).toBe(true);
    });

    it('will have an aria described of optional-label if not required', () => {
      // Arrange
      wrapper = mountQuestion({
        propsData: {
          id: 'id',
          required: false,
          error: true,
          errorText: ['Error'],
        },
      });

      // Act
      const inputAttributes = wrapper.find('input').attributes();

      // Assert
      expect(inputAttributes['aria-describedby']).toBe('iderror optional-label id-error-message');
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

  describe('methods', () => {
    describe('checkAndEmitIsValueValid', () => {
      beforeAll(() => {
        questionStringAnswerValid.mockClear();
      });
      it('will validate the answer and emit the result', () => {
        // Arrange
        questionStringAnswerValid.mockReturnValue({ valid: false, message: 'too long' });

        // Act
        wrapper.vm.checkAndEmitIsValueValid('an answer longer than 20 characters');

        // Assert
        expect(questionStringAnswerValid).toHaveBeenCalledWith('an answer longer than 20 characters', true, '20');
        expect(wrapper.emitted('validate')[1][0]).toEqual({ valid: false, message: 'too long' });
      });
    });
  });
});
