import GenericImageInput from '@/components/widgets/GenericImageInput';
import each from 'jest-each';
import { mount } from '../../helpers';

const state = {
  device: {
    isNativeApp: false,
  },
};

let wrapper;

describe('GenericImageInput.vue', () => {
  const mountConfirmation = ({ propsData = {}, methods = {} } = {}) =>
    mount(GenericImageInput, {
      state,
      propsData,
      methods,
    });

  describe('image click', () => {
    it('should call on image clicked on click event', () => {
      wrapper = mountConfirmation({
        propsData: {
          id: 'id',
        },
      });
      const image = wrapper.find("[id='id']");
      image.trigger('click');
      expect(wrapper.emitted('input')).toBeDefined();
    });

    it('should emit when on image clicked is called', () => {
      const onImageClicked = jest.fn();
      wrapper = mountConfirmation({
        propsData: {
          id: 'id',
        },
        methods: {
          onImageClicked,
        },
      });

      const image = wrapper.find("[id='id']");
      image.trigger('click');

      expect(onImageClicked).toHaveBeenCalledTimes(1);
    });

    each([{
      id: 'imageId',
      errorId: 'imageId-error-message',
    }, {
      id: undefined,
      errorId: 'error-message',
    }]).it('should appropriately set the error', ({ id, errorId }) => {
      wrapper = mountConfirmation({
        propsData: {
          id,
        },
      });
      expect(wrapper.vm.errorId).toBe(errorId);
    });
  });
});
