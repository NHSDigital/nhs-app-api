import GenericAttachment from '@/components/widgets/GenericAttachment';
import { mount } from '../../helpers';
import each from 'jest-each';

const state = {
  device: {
    isNativeApp: false,
  },
};

let wrapper;

describe('GenericAttachment.vue', () => {
  const mountConfirmation = ({ propsData = {}, methods = {} } = {}) =>
    mount(GenericAttachment, {
      state,
      propsData,
      methods,
    });

  describe('input', () => {
    it('should call selected file change on change event', () => {
      wrapper = mountConfirmation({
        propsData: {
          id: 'id',
        },
      });
      const input = wrapper.find("[id='id']");
      input.trigger('change');
      expect(wrapper.emitted('input')).toBeDefined();
    });

    it('should emit when selected file changed is called', () => {
      const onSelectedFileChange = jest.fn();
      wrapper = mountConfirmation({
        propsData: {
          id: 'id',
        },
        methods: {
          onSelectedFileChange,
        },
      });

      const input = wrapper.find("[id='id']");
      input.trigger('change');

      expect(onSelectedFileChange).toHaveBeenCalledTimes(1);
    });

    each([{
      id: 'testId',
      errorId: 'testId-error-message',
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
