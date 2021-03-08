import GenericTextArea from '@/components/widgets/GenericTextArea';
import { mount } from '../../helpers';

const state = {
  device: {
    isNativeApp: false,
  },
};

const mountConfirmation = ({ propsData } = {}) =>
  mount(GenericTextArea, {
    state,
    propsData,
  });

describe('GenericTextArea.vue', () => {
  let wrapper;
  let genericTextArea;

  beforeEach(() => {
    wrapper = mountConfirmation();
    genericTextArea = wrapper.find('textarea');
  });

  it('will verify the textarea component exists', () => {
    expect(genericTextArea.exists()).toBe(true);
  });

  it('will verify the textarea component with the passed in id exists', () => {
    wrapper = mountConfirmation({
      propsData: {
        id: 'anId',
      },
    });
    genericTextArea = wrapper.find('textarea#anId');

    expect(genericTextArea.exists()).toBe(true);
  });

  it('will verify the attributes are being set correctly', () => {
    wrapper = mountConfirmation({
      propsData: {
        id: 'anId',
        maxlength: '10',
      },
    });
    genericTextArea = wrapper.find('textarea');
    const attributes = genericTextArea.attributes();

    expect(attributes.maxlength).toEqual('10');
    expect(attributes.id).toEqual('anId');
  });

  it('will verify a value is emitted when input is set', () => {
    wrapper.vm.textValue = 'Emit this';
    wrapper.trigger('input');

    expect(wrapper.emitted('input')).toBeDefined();
    expect(wrapper.emitted('input').length).toBe(1);
    expect(wrapper.emitted('input')[0][0]).toEqual('Emit this');

    wrapper.vm.textValue = 'Update and emit this';
    expect(wrapper.emitted('input').length).toBe(2);
    expect(wrapper.emitted('input')[1][0]).toEqual('Update and emit this');
  });

  it('will emit a \'focus\' event when the textarea is focused', () => {
    wrapper.find('textarea').trigger('focus');

    expect(wrapper.emitted('focus').length).toBe(1);
  });

  it('should appropriately set aria described-by based on error state and property', () => {
    wrapper = mountConfirmation({
      propsData: {
        id: 'anId',
        error: true,
        required: false,
        errorText: 'errorMessage',
        aDescribedBy: 'testAriaLabel',
      },
    });

    genericTextArea = wrapper.find('textarea#anId');
    const inputAttributes = genericTextArea.attributes();

    expect(inputAttributes['aria-describedby']).toEqual('testAriaLabel anId-error-message');
  });
});
