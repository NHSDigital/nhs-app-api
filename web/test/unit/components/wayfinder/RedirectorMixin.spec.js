import RedirectorMixin from '@/components/wayfinder/RedirectorMixin';
import { mount, createStore } from '../../helpers';

const goToUrl = jest.fn();
window.open = jest.fn();

let wrapper;

const mountComponent = ({ isNativeApp = false } = {}) => (mount(
  { render() {}, mixins: [RedirectorMixin] },
  {
    propsData: {
      deepLinkUrl: 'test.deeplink/app',
    },
    methods: {
      goToUrl,
    },
    $store: createStore({
      state: { device: { isNativeApp } },
    }),
  },
));

describe('onClick', () => {
  beforeEach(() => {
    goToUrl.mockClear();
    window.open.mockClear();
  });

  describe('on native', () => {
    beforeEach(() => {
      wrapper = mountComponent({ isNativeApp: true });
    });

    it('will call goToUrl with a path to redirector and redirect_to query with encoded url', () => {
      wrapper.vm.onClick();

      expect(goToUrl).toHaveBeenCalledWith('/redirector?redirect_to=test.deeplink%2Fapp');
    });

    it('will not call window.open', () => {
      wrapper.vm.onClick();

      expect(window.open).not.toHaveBeenCalled();
    });
  });

  describe('on web', () => {
    beforeEach(() => {
      wrapper = mountComponent();
    });

    it('will not call goToUrl', () => {
      wrapper.vm.onClick();

      expect(goToUrl).not.toHaveBeenCalled();
    });

    it('will call window.open with a path to redirector and redirect_to query with encoded url', () => {
      wrapper.vm.onClick();

      expect(window.open).toHaveBeenCalledWith(
        '/redirector?redirect_to=test.deeplink%2Fapp',
        '_blank',
        'noopener,noreferrer',
      );
    });
  });
});
