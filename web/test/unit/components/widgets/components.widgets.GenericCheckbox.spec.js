import { mount } from '../../helpers';
import GenericCheckbox from '../../../../src/components/widgets/GenericCheckbox';

describe('GenericCheckbox.vue', () => {
  let wrapper;

  beforeEach(() => {
    const mountConfirmation = ({ propsData } = {}) =>
      mount(GenericCheckbox, {
        propsData,
      });

    wrapper = mountConfirmation({
      propsData: {
        checkboxId: 'checkboxId',
        error: true,
        required: false,
        errorText: 'errorMessage',
        aDescribedBy: 'testAriaLabel',
      },
    });
  });

  it('will verify that a checkbox has an associated label', () => {
    expect(wrapper.find("input[type='checkbox'][id='checkboxId']")
      .exists()).toEqual(true);

    expect(wrapper.find("label[for='checkboxId']")
      .exists()).toEqual(true);
  });

  it('should appropriately set aria described-by based on error state and property', () => {
    const inputAttributes = wrapper.find('input').attributes();

    expect(inputAttributes['aria-describedby']).toEqual('testAriaLabel');
  });
});
