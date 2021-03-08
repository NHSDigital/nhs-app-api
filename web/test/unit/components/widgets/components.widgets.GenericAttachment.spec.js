import GenericAttachment from '@/components/widgets/GenericAttachment';
import each from 'jest-each';
import { mount } from '../../helpers';

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
      expect(wrapper.emitted('change')).toBeDefined();
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

    it('should appropriately set aria described-by based on error state and property', () => {
      wrapper = mountConfirmation({
        propsData: {
          error: true,
          required: false,
          id: 'question',
          errorText: 'errorMessage',
          aDescribedBy: 'testAriaLabel',
        },
      });
      const inputAttributes = wrapper.find('input').attributes();

      expect(inputAttributes['aria-describedby']).toEqual('testAriaLabel question-error-message');
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
