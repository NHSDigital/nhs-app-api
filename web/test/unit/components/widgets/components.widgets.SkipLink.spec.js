import i18n from '@/plugins/i18n';
import SkipLink from '@/components/widgets/SkipLink';
import { createStore, mount } from '../../helpers';

describe('SkipLink.vue', () => {
  let $router;
  let $store;
  let wrapper;

  const mountAs = ({ native = true, methods }) => {
    $store = createStore({
      state: {
        device: {
          isNativeApp: native,
        },
      },
    });

    return mount(SkipLink, { $store, $router, methods, mountOpts: { i18n } });
  };
  describe('Skip link is present and usable in desktop', () => {
    const setFocus = jest.fn();
    let link;

    beforeEach(() => {
      wrapper = mountAs({ native: false,
        methods: {
          setFocus,
        } });
      link = wrapper.find('a');
    });

    it('will show desktop view shows the Skip Link', () => {
      expect(link
        .exists())
        .toBe(true);
    });

    it('will display skip to main content text', () => {
      expect(link.text()).toEqual('Skip to main content');
    });

    it('clicking the link calls the correct function', () => {
      link.trigger('click');
      expect(setFocus).toBeCalled();
    });
  });

  describe('Skip link not visible in native', () => {
    it('will not display the link in native', () => {
      wrapper = mountAs({ native: true });
      expect(wrapper.find('a')
        .exists())
        .toBe(false);
    });
  });
});

