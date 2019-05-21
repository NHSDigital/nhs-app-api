import QuestionAttachment from '@/components/online-consultations/QuestionAttachment';
import GenericAttachment from '@/components/widgets/GenericAttachment';

import { mount } from '../../helpers';

let wrapper;
const getInputWrapper = (useComponent = true) => wrapper.find(useComponent ? GenericAttachment : 'input[type=file]');

const file = new File(['This is a pdf'], 'aDocument.pdf', { type: '.pdf' });

const state = {
  device: {
    isNativeApp: false,
  },
};

const mountQuestion = ({ propsData = {}, methods = {} } = {}) =>
  mount(QuestionAttachment, {
    state,
    propsData: {
      name: 'file-attachment',
      id: 'file-attachment-id',
      ...propsData,
    },
    methods,
  });

describe('QuestionAttachment.vue', () => {
  beforeEach(() => {
    wrapper = mountQuestion();
  });

  it('will verify the input is present on the page', () => {
    const input = getInputWrapper();

    expect(input.exists()).toBe(true);
  });

  describe('Data properties', () => {
    it('should be properly configured initially', () => {
      /* eslint-disable no-underscore-dangle */
      expect(wrapper.vm._props.value).toEqual(undefined);
    });
  });

  describe('Initial values', () => {
    it('should emit if data is valid', () => {
      wrapper = mountQuestion();
      wrapper.vm.checkAndEmitValueIsValid(file);

      expect(wrapper.emitted('validate')).toBeDefined();
      expect(wrapper.emitted().validate[0].length).toEqual(1);
      expect(wrapper.emitted().validate[0][0]).toBe(true);
    });

    it('should emit if data is invalid', () => {
      wrapper = mountQuestion();
      wrapper.vm.required = true;
      wrapper.vm.checkAndEmitValueIsValid(undefined);

      expect(wrapper.emitted('validate')).toBeDefined();
      expect(wrapper.emitted().validate[0].length).toEqual(1);
      expect(wrapper.emitted().validate[0][0]).toBe(false);
    });
  });

  describe('Methods', () => {
    describe('isValid', () => {
      it('should evaluate true if value is not required and value has not been chosen', () => {
        const propsData = { required: false };
        wrapper = mountQuestion({ propsData });
        const isValidInput = wrapper.vm.isValidInput('');
        expect(isValidInput).toEqual(true);
      });

      it('should validate false if value is required and no value has been chosen', () => {
        const propsData = { required: true };
        wrapper = mountQuestion({ propsData });
        const isValidInput = wrapper.vm.isValidInput(undefined);
        expect(isValidInput).toEqual(false);
      });

      it('should validate true if value is required and value has been chosen', () => {
        const propsData = { required: true };
        wrapper = mountQuestion({ propsData });
        const isValidInput = wrapper.vm.isValidInput(file);
        expect(isValidInput).toEqual(true);
      });
    });

    describe('onSelectedFileChanged', () => {
      it('should emit on selected file change', () => {
        const changeEvent = {
          target: {
            files: ['aFile'],
          },
        };
        const checkAndEmitValueIsValid = jest.fn();
        wrapper = mountQuestion({
          methods: {
            checkAndEmitValueIsValid,
          },
        });
        wrapper.vm.onSelectedFileChanged(changeEvent);

        expect(wrapper.emitted('input')).toBeDefined();
        expect(checkAndEmitValueIsValid).toBeCalledWith('aFile');
        expect(wrapper.emitted().input[0].length).toEqual(1);
        expect(wrapper.emitted().input[0][0]).toBe('aFile');
      });
    });
  });
});
