import ScrollToAnchor from '@/components/widgets/ScrollToAnchor';
import { mount, createStore } from '../../helpers';

let wrapper;

const mountComponent = ({ isNativeApp } = {}) => {
  wrapper = mount(ScrollToAnchor, {
    $style: {
      scrollToAnchorPosition: 'scrollToAnchor',
      scrollToAnchorNativePosition: 'nativeScrollToAnchor',
    },
    $store: createStore({
      state: { device: { isNativeApp } },
    }),
  });
};

describe('Scroll to anchor', () => {
  describe('classes', () => {
    describe('isNativeApp is false', () => {
      beforeEach(() => {
        mountComponent();
      });

      it('will contain only scrollToAnchorPosition style', () => {
        expect(wrapper.vm.classes).toEqual(['scrollToAnchor', true]);
      });
    });
    describe('isNativeApp is true', () => {
      beforeEach(() => {
        mountComponent({ isNativeApp: true });
      });

      it('will contain scrollToAnchorPosition and scrollToAnchorNativePosition styles', () => {
        expect(wrapper.vm.classes).toEqual([
          'scrollToAnchor',
          'nativeScrollToAnchor',
        ]);
      });
    });
  });

  describe('anchor tag', () => {
    let scrollToAnchor;

    beforeEach(() => {
      mountComponent({ isNativeApp: true });
      scrollToAnchor = wrapper.find('a');
    });

    it('will have classes equal to classes field', () => {
      expect(scrollToAnchor.attributes().class).toEqual('scrollToAnchor nativeScrollToAnchor');
    });

    it('will have no href', () => {
      expect(scrollToAnchor.attributes().href).toBeUndefined();
    });

    it('will have no innerHTML', () => {
      expect(scrollToAnchor.element.innerHTML).toEqual('');
    });
  });
});
