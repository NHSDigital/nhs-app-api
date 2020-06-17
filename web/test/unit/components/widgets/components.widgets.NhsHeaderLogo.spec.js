import NhsHeaderLogo from '@/components/widgets/NhsHeaderLogo';
import { RouterLinkStub } from '@vue/test-utils';
import { createStore, mount } from '../../helpers';

describe('NhsHeaderLogo.vue', () => {
  let $router;
  let $store;
  let wrapper;

  const mountAs = ({ native = true }) => {
    $store = createStore({
      state: {
        device: {
          isNativeApp: native,
        },
      },
    });

    return mount(NhsHeaderLogo, { $store, $router, stubs: { 'router-link': RouterLinkStub } });
  };

  describe('on desktop', () => {
    let span;

    beforeEach(() => {
      wrapper = mountAs({ native: false });
      span = wrapper.find('#logo-text');
    });

    it('will show desktop service header', () => {
      expect(span
        .exists())
        .toBe(true);
    });

    it('will display text from webHeader.logoText', () => {
      expect(span.text()).toEqual('translate_webHeader.logoText');
    });

    it('will have the service header design for the logo', () => {
      expect(wrapper.find('#nhs_logo').attributes('class')).toBe('nhsuk-header__link nhsuk-header__link--service');
    });
  });

  describe('on native', () => {
    let span;

    beforeEach(() => {
      wrapper = mountAs({ native: true });
      span = wrapper.find('#logo-text');
    });

    it('will not show desktop service header', () => {
      expect(span
        .exists())
        .toBe(false);
    });
  });
});
