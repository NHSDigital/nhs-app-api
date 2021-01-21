import each from 'jest-each';
import InsetText from '@/components/InsetText';
import { mount } from '../helpers';

const createInsetTextComponent = (compact = false) => mount(InsetText, {
  propsData: {
    compact,
  },
  slots: {
    default: '<a id="slotLink"></a>',
  },
});

describe('InsetText', () => {
  it('includes the hidden span', () => {
    const wrapper = createInsetTextComponent();

    expect(wrapper.find('.nhsuk-u-visually-hidden').exists()).toBe(true);
  });

  it('has a default slot for content', () => {
    const wrapper = createInsetTextComponent();

    expect(wrapper.find('a[id=slotLink]').exists()).toBe(true);
  });

  each([true, false]).it('will have top and bottom margin when compact', (compact) => {
    const classes = createInsetTextComponent(compact)
      .find('[data-purpose=inset-text]').classes() || [];

    expect(classes.includes('nhsuk-u-margin-top-1')).toBe(compact);
    expect(classes.includes('nhsuk-u-margin-bottom-1')).toBe(compact);
  });
});
