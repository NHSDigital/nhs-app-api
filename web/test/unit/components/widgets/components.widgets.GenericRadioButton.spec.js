import GenericRadioButton from '@/components/widgets/GenericRadioButton';
import { mount } from '../../helpers';

let radioButton;

const mountRadioButton = ({ propsData = {} } = {}) => {
  radioButton = mount(GenericRadioButton, {
    propsData,
  });
};

describe('generic radio button', () => {
  it('will render input labels as html if renderAsHtml is true', () => {
    mountRadioButton({
      propsData: {
        renderAsHtml: true,
        label: '<span id="one">One</span>',
      },
    });

    const label = radioButton.find('label span#one');

    expect(label).toBeDefined();
  });

  it('will not render input labels as html if renderAsHtml is false', () => {
    mountRadioButton({
      propsData: {
        renderAsHtml: false,
        label: 'One',
      },
    });

    const label = radioButton.find('label.nhsuk-label');

    expect(label).toBeDefined();
    expect(label.element.innerHTML).toEqual('One');
  });
});
