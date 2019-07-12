/* eslint-disable no-console */
import QuestionImage from '@/components/online-consultations/QuestionImage';
import GenericImageInput from '@/components/widgets/GenericImageInput';
import { mount } from '../../helpers';

const imageValidationMessage = 'appointments.admin_help.errors.validation.message.image';
let wrapper;

const getImageWrapper = (useComponent = true) => wrapper.find(useComponent ? GenericImageInput : 'img');

const state = {
  device: {
    isNativeApp: false,
  },
};

const mountQuestion = ({ propsData = {}, methods = {} } = {}) =>
  mount(QuestionImage, {
    state,
    propsData: {
      name: 'image',
      id: 'image-id',
      ...propsData,
    },
    methods,
  });

describe('QuestionImage.vue', () => {
  beforeEach(() => {
    wrapper = mountQuestion();
  });

  it('will verify the image is present on the page', () => {
    const image = getImageWrapper();
    expect(image.exists()).toBe(true);
  });

  describe('Initial values', () => {
    it('should emit if data is invalid', () => {
      wrapper = mountQuestion();
      wrapper.vm.checkAndEmitIsValueValid();
      const expectedValidation = {
        isValid: false,
        isEmpty: true,
        message: imageValidationMessage,
      };

      const emittedInputs = wrapper.emitted('validate');
      expect(emittedInputs).toBeDefined();
      expect(emittedInputs[0].length).toEqual(1);
      expect(emittedInputs[0][0]).toEqual(expectedValidation);
    });

    it('should emit if data is invalid', () => {
      const propsData = { required: false };
      wrapper = mountQuestion({ propsData });
      wrapper.vm.checkAndEmitIsValueValid({ offsetX: 10, offsetY: 10 });
      const expectedValidation = {
        isValid: true,
        isEmpty: true,
      };

      const emittedInputs = wrapper.emitted('validate');
      expect(emittedInputs).toBeDefined();
      expect(emittedInputs[0].length).toEqual(1);
      expect(emittedInputs[0][0]).toEqual(expectedValidation);
    });
  });

  describe('Methods', () => {
    describe('onImageClicked', () => {
      it('should emit on image clicked', () => {
        const offsetX = 200;
        const offsetY = 100;

        const clickEvent = {
          offsetX,
          offsetY,
        };

        const checkAndEmitIsValueValid = jest.fn();
        wrapper = mountQuestion({
          methods: {
            checkAndEmitIsValueValid,
          },
        });

        wrapper.vm.onImageClicked(clickEvent);

        const emittedInputs = wrapper.emitted('input');
        expect(emittedInputs).toBeDefined();
        expect(emittedInputs[0].length).toEqual(1);
        expect(emittedInputs[0][0]).toEqual({ x: offsetX, y: offsetY });
        expect(checkAndEmitIsValueValid).toBeCalledWith({ x: offsetX, y: offsetY });
      });
    });
  });
});
