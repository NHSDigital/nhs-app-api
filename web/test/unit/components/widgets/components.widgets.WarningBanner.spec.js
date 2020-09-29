import WarningBanner from '@/components/widgets/WarningBanner';
import { mount } from '../../helpers';

describe('WarningBanner', () => {
  const $style = {
    yellow: 'yellow',
    silver: 'silver',
    floating: 'floating',
    border: 'border',
  };

  const mountBanner = ({ color = 'yellow',
    shouldBeFloating = false,
    hasBorder = false } = {}) => mount(WarningBanner, {
    $style,
    propsData: {
      color,
      shouldBeFloating,
      hasBorder,
    },
  });
  it('will use the `yellow` style as default with no other classes', () => {
    const wrapper = mountBanner();
    expect(wrapper.classes()).toContain($style.yellow);
    expect(wrapper.classes()).not.toContain($style.border);
    expect(wrapper.classes()).not.toContain($style.floating);
  });

  it('will use the `silver` style when the color prop is `silver`', () => {
    const wrapper = mountBanner({ color: 'silver' });
    expect(wrapper.classes()).toContain($style.silver);
  });

  it('will use the `border` style when the hasBorder prop is true', () => {
    const wrapper = mountBanner({ hasBorder: true });
    expect(wrapper.classes()).toContain($style.border);
  });

  it('will use the `floating` style when the shouldBeFloating prop is true', () => {
    const wrapper = mountBanner({ shouldBeFloating: true });
    expect(wrapper.classes()).toContain($style.floating);
  });
});
