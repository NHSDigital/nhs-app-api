import RedirectorMixin from '@/components/wayfinder/RedirectorMixin';
import { mount, createStore } from '../../helpers';

const goToUrl = jest.fn();
window.open = jest.fn();

const deepLinkUrl = 'https://appointments.stubs.local/1';

let wrapper;

const mountComponent = ({ isNativeApp = false } = {}) => (mount(
  { render() {}, mixins: [RedirectorMixin] },
  {
    methods: {
      goToUrl,
    },
    $store: createStore({
      state: { device: { isNativeApp } },
    }),
  },
));

describe('goToUrlViaRedirector', () => {
  beforeEach(() => {
    goToUrl.mockClear();
    window.open.mockClear();
  });

  describe('on native', () => {
    beforeEach(() => {
      wrapper = mountComponent({ isNativeApp: true });
      wrapper.vm.goToUrlViaRedirector(deepLinkUrl);
    });

    it('will call goToUrl with a path to redirector and redirect_to query with encoded url', () => {
      expect(goToUrl).toHaveBeenCalledWith('/redirector?redirect_to=https%3A%2F%2Fappointments.stubs.local%2F1');
    });

    it('will not call window.open', () => {
      expect(window.open).not.toHaveBeenCalled();
    });
  });

  describe('on web', () => {
    beforeEach(() => {
      wrapper = mountComponent();
      wrapper.vm.goToUrlViaRedirector(deepLinkUrl);
    });

    it('will not call goToUrl', () => {
      expect(goToUrl).not.toHaveBeenCalled();
    });

    it('will call window.open with a path to redirector and redirect_to query with encoded url', () => {
      expect(window.open).toHaveBeenCalledWith(
        '/redirector?redirect_to=https%3A%2F%2Fappointments.stubs.local%2F1',
        '_blank',
        'noopener,noreferrer',
      );
    });
  });
});
