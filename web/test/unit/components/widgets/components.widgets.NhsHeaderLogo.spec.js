import { createStore, mount } from '../../helpers';
import NhsHeaderLogo from '@/components/widgets/NhsHeaderLogo';

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

    return mount(NhsHeaderLogo, { $store, $router });
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
