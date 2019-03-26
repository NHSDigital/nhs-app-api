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
      },
    });
  });

  it('will verify that a checkbox has an associated label', () => {
    expect(wrapper.find("input[type='checkbox'][id='checkboxId']")
      .exists()).toEqual(true);

    expect(wrapper.find("label[for='checkboxId']")
      .exists()).toEqual(true);
  });
});
