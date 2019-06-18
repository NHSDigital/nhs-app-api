import QuestionMultipleChoice from '@/components/online-consultations/QuestionMultipleChoice';
import GenericCheckbox from '@/components/widgets/GenericCheckbox';
import CheckboxGroup from '@/components/CheckboxGroup';
import { mount } from '../../helpers';

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

const optionsSelected = [
  { label: 'ONE', selected: true, code: 1 },
  { label: 'TWO', selected: false, code: 2 },
  { label: 'THREE', selected: false, code: 3 },
];


describe('QuestionMultipleChoice.vue', () => {
  beforeEach(() => {
    wrapper = mountQuestion();
  });

  it('will verify the checkbox group is present on the page', () => {
    const checkboxGroup = getCheckboxGroup();
    expect(checkboxGroup.exists()).toBe(true);
  });

  it('will verify that the correct number of checkboxes are present on the page', () => {
    wrapper = mountQuestion();
    expect(wrapper.findAll(GenericCheckbox).length).toBe(3);
  });

  describe('Initial values', () => {
    it('should emit if data is invalid', () => {
      wrapper = mountQuestion();
      wrapper.vm.checkAndEmitIsValueValid(initialOptions);
      expect(wrapper.emitted('validate')).toBeDefined();
      expect(wrapper.emitted().validate[0].length).toEqual(1);
      expect(wrapper.emitted().validate[0][0]).toBe(false);
    });

    it('should emit if data is valid', () => {
      wrapper = mountQuestion({ propsData: { required: false } });
      wrapper.vm.checkAndEmitIsValueValid(optionsSelected);
      expect(wrapper.emitted('validate')).toBeDefined();
      expect(wrapper.emitted().validate[0].length).toEqual(1);
      expect(wrapper.emitted().validate[0][0]).toBe(true);
    });
  });

  describe('Methods', () => {
    describe('isValid', () => {
      it('should validate true if selection is not required no checkboxes are selected', () => {
        wrapper = mountQuestion({ propsData: { required: false } });
        const isValidInput = wrapper.vm.isValidInput([]);
        expect(isValidInput).toEqual(true);
      });

      it('should validate true if selection is required and checkbox is selected', () => {
        wrapper = mountQuestion();
        const isValidInput = wrapper.vm.isValidInput([1, 3]);
        expect(isValidInput).toEqual(true);
      });

      it('should validate to false if selection is required and no checkbox is selected', () => {
        wrapper = mountQuestion();
        const isValidInput = wrapper.vm.isValidInput([]);
        expect(isValidInput).toBe(false);
      });
    });

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



