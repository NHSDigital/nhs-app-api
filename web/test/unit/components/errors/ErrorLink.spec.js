import ErrorLink from '@/components/errors/ErrorLink';
import { mount } from '../../helpers';

describe('ErrorLink', () => {
  let wrapper;
  let goToUrl;

  const mountWrapper = ({
    action,
    desktopOnly,
    from,
    isNativeApp = true,
    queryParam,
    target }) => mount(ErrorLink, {
    $store: {
      state: {
        device: {
          isNativeApp,
        },
      },
    },
    methods: {
      goToUrl,
    },
    propsData: {
      action,
      desktopOnly,
      from,
      queryParam,
      target,
    },
  });

  beforeEach(() => {
    goToUrl = jest.fn();
  });

  describe('from', () => {
    describe('translates to text', () => {
      beforeEach(() => {
        wrapper = mountWrapper({ from: 'appTitle' });
      });

      describe('link', () => {
        let link;

        beforeEach(() => {
          link = wrapper.find('a');
        });

        it('will exist', () => {
          expect(link.exists()).toBe(true);
        });

        it('will not have aria label', () => {
          expect(link.attributes('aria-label')).toBeUndefined();
        });

        it('will translate `from`', () => {
          expect(link.text()).toBe('NHS App');
        });
      });
    });

    describe('translates to object', () => {
      beforeEach(() => {
        wrapper = mountWrapper({ from: 'login.authReturn.ifYouNeed' });
      });

      describe('link', () => {
        let link;

        beforeEach(() => {
          link = wrapper.find('a');
        });

        it('will exist', () => {
          expect(link.exists()).toBe(true);
        });

        it('will have aria label', () => {
          expect(link.attributes('aria-label')).toBe('If you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, visit 111.nhs.uk or call one one one.');
        });

        it('will display `from` text', () => {
          expect(link.text()).toBe('If you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, visit 111.nhs.uk or call 111.');
        });
      });
    });
  });

  describe('desktop only', () => {
    let desktopOnly;

    describe('is true', () => {
      beforeEach(() => {
        desktopOnly = true;
      });

      describe('is native', () => {
        beforeEach(() => {
          wrapper = mountWrapper({ desktopOnly, from: 'appTitle', isNativeApp: true });
        });

        it('will not display link', () => {
          expect(wrapper.find('a').exists()).toBe(false);
        });
      });

      describe('is not native', () => {
        beforeEach(() => {
          wrapper = mountWrapper({ desktopOnly, from: 'appTitle', isNativeApp: false });
        });

        it('will display link', () => {
          expect(wrapper.find('a').exists()).toBe(true);
        });
      });
    });

    describe('is false', () => {
      beforeEach(() => {
        wrapper = mountWrapper({ desktopOnly: false, from: 'appTitle', isNativeApp: true });
      });

      it('will always display link', () => {
        expect(wrapper.find('a').exists()).toBe(true);
      });
    });
  });

  describe('target', () => {
    describe('on link click with `_blank` target', () => {
      beforeEach(() => {
        global.open = jest.fn();
        wrapper = mountWrapper({ action: '/example', from: 'appTitle', target: '_blank' });
        wrapper.find('a').trigger('click');
      });

      it('will open the url on a tab', () => {
        expect(global.open).toBeCalledWith('/example', '');
      });

      it('will not go to url', () => {
        expect(goToUrl).not.toBeCalled();
      });
    });

    describe('on link click with any other target', () => {
      beforeEach(() => {
        global.open = jest.fn();
        wrapper = mountWrapper({ action: '/example', from: 'appTitle', target: 'foo' });
        wrapper.find('a').trigger('click');
      });

      it('will not open the url on a tab', () => {
        expect(global.open).not.toBeCalled();
      });

      it('will go to url', () => {
        expect(goToUrl).toBeCalledWith('/example');
      });
    });
  });

  describe('with query parameter', () => {
    beforeEach(() => {
      wrapper = mountWrapper({ action: '/example', from: 'appTitle', queryParam: { param: 'param', value: 'value' } });
    });

    describe('on link click', () => {
      beforeEach(() => {
        wrapper.find('a').trigger('click');
      });

      it('will append query parameter to the action url', () => {
        expect(goToUrl).toBeCalledWith('/example?param=value');
      });
    });
  });
});
