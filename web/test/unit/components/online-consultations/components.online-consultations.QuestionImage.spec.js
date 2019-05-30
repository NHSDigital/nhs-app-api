/* eslint-disable no-console */
import QuestionImage from '@/components/online-consultations/QuestionImage';
import GenericImageInput from '@/components/widgets/GenericImageInput';
import { mount } from '../../helpers';

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
      wrapper.vm.checkAndEmitIsValueValid([]);

      expect(wrapper.emitted('validate')).toBeDefined();
      expect(wrapper.emitted().validate[0].length).toEqual(1);
      expect(wrapper.emitted().validate[0][0]).toBe(false);
    });

    it('should emit if data is invalid', () => {
      const propsData = { required: false };
      wrapper = mountQuestion({ propsData });
      wrapper.vm.checkAndEmitIsValueValid([]);
      expect(wrapper.emitted('validate')).toBeDefined();
      expect(wrapper.emitted().validate[0].length).toEqual(1);
      expect(wrapper.emitted().validate[0][0]).toBe(true);
    });
  });

  describe('Methods', () => {
    describe('isValid', () => {
      it('should validate true if input it not required and no input is given', () => {
        const propsData = { required: false };
        wrapper = mountQuestion({ propsData });
        const isValidInput = wrapper.vm.isValidInput([]);
        expect(isValidInput).toEqual(true);
      });

      it('should validate true if input is required and input is given', () => {
        const propsData = { required: true };
        wrapper = mountQuestion({ propsData });
        const isValidInput = wrapper.vm.isValidInput([{ x: 10, y: 15 }]);
        expect(isValidInput).toEqual(true);
      });

      it('should validate false if input is required and no input is given', () => {
        const propsData = { required: true };
        wrapper = mountQuestion({ propsData });
        const isValidInput = wrapper.vm.isValidInput([]);
        expect(isValidInput).toEqual(false);
      });
    });

    // {x: 10, y: 15}
    describe('onImageClicked', () => {
      it('should emit on image clicked', () => {
        const eventClientX = 200;
        const eventCLientY = 100;
        const offsetLeft = 36;
        const offsetTop = -89;
        const xCoordinate = eventClientX - offsetLeft;
        const yCoordinate = eventCLientY - offsetTop;

        const clickEvent = {
          clientX: 200,
          clientY: 100,
          target: {
            offsetLeft,
            offsetTop,
          },
        };

        const checkAndEmitIsValueValid = jest.fn();
        wrapper = mountQuestion({
          methods: {
            checkAndEmitIsValueValid,
          },
        });

        wrapper.vm.onImageClicked(clickEvent);

        expect(wrapper.emitted('input')).toBeDefined();
        expect(checkAndEmitIsValueValid).toBeCalledWith([{ x: xCoordinate, y: yCoordinate }]);
        expect(wrapper.emitted().input[0].length).toEqual(1);
        expect(wrapper.emitted().input[0][0]).toEqual([{ x: xCoordinate, y: yCoordinate }]);
      });
    });
  });
});
