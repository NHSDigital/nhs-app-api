import QuestionMultipleChoice from '@/components/online-consultations/QuestionMultipleChoice';
import GenericCheckbox from '@/components/widgets/GenericCheckbox';
import CheckboxGroup from '@/components/CheckboxGroup';
import { mount } from '../../helpers';
import each from 'jest-each';

const multiChoiceValidationMessage = 'onlineConsultations.validationErrors.message.multiple_choiceAtLeastOneRequired';
let wrapper;

const getCheckboxGroup = (useComponent = true) => wrapper.find(useComponent ? CheckboxGroup : 'checkbox-group');

const initialOptions = [
  { label: 'ONE', selected: false, code: 1 },
  { label: 'TWO', selected: false, code: 2 },
  { label: 'THREE', selected: false, code: 3 },
];

const mountQuestion = ({ propsData = {}, methods = {} } = {}) =>
  mount(QuestionMultipleChoice, {
    propsData: {
      name: 'question-multiple-choice',
      options: initialOptions,
      methods,
      ...propsData,
    },
  });

describe('QuestionMultipleChoice.vue', () => {
  beforeEach(() => {
    wrapper = mountQuestion();
  });

  it('will verify the checkbox group is present on the page', () => {
    const checkboxGroup = getCheckboxGroup();
    expect(checkboxGroup.exists()).toBe(true);
  });

  it('will verify that the correct number of checkboxes are present on the page', () => {
    expect(wrapper.findAll(GenericCheckbox).length).toBe(3);
  });

  describe('Initial values', () => {
    it('should emit if data is invalid', () => {
      wrapper = mountQuestion({ propsData: { value: [] } });
      const emittedValidates = wrapper.emitted('validate');
      expect(emittedValidates).toBeDefined();
      expect(emittedValidates[0].length).toEqual(1);
      expect(emittedValidates[0][0]).toEqual({
        isValid: false,
        isEmpty: true,
        message: multiChoiceValidationMessage,
      });
    });

    it('should emit if data is valid', () => {
      wrapper = mountQuestion({ propsData: { value: [1, 2], required: false } });
      const emittedValidates = wrapper.emitted('validate');
      expect(emittedValidates).toBeDefined();
      expect(emittedValidates[0].length).toEqual(1);
      expect(emittedValidates[0][0]).toEqual({
        isValid: true,
        isEmpty: false,
        message: multiChoiceValidationMessage,
      });
    });
    each([
      true,
      false,
    ]).it('will set renderAsHtml on radio group', (renderAsHtml) => {
      wrapper = mountQuestion({ propsData: { renderAsHtml } });

      const checkboxGroup = wrapper.find(CheckboxGroup);

      expect(checkboxGroup.vm.renderAsHtml).toEqual(renderAsHtml);
    });
  });

  describe('Methods', () => {
    describe('selectedValuesChanged', () => {
      it('should emit on checkbox selected', () => {
        const checkAndEmitIsValueValid = jest.fn();
        wrapper = mountQuestion({
          methods: {
            checkAndEmitIsValueValid,
          },
        });
        const changeEvent = [1];
        wrapper.vm.selectedValuesChanged(changeEvent);
        expect(wrapper.emitted().input[0].length).toEqual(1);
        expect(wrapper.emitted().input[0][0]).toEqual([1]);
      });
    });
  });
});
