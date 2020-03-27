import WarningCallout from '@/components/widgets/WarningCallout';
import { mount } from '../../helpers';

let wrapper;
const mountCallout = ({ slot = '' } = {}) => (
  mount(WarningCallout, {
    propsData: {
      title: 'Test Callout',
    },
    slots: { default: slot },
  })
);

describe('warning callout', () => {
  it('will use nhsuk warning callout styles', () => {
    wrapper = mountCallout();
    expect(wrapper.find('.nhsuk-warning-callout > h3.nhsuk-warning-callout__label').exists())
      .toBe(true);
  });

  it('will have a required title prop', () => {
    wrapper = mountCallout();

    expect(wrapper.vm.$options.props.title.required).toEqual(true);
  });

  it('will set the warning callout label to the title prop', () => {
    wrapper = mountCallout();
    expect(wrapper.find('.nhsuk-warning-callout__label').text()).toEqual('Test Callout');
  });

  it('will have a slot for rendering callout content', () => {
    wrapper = mountCallout({ slot: 'Content provided via slot' });
    expect(wrapper.find('.nhsuk-warning-callout').text()).toContain('Content provided via slot');
  });
});
