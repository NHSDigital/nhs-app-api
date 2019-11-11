import LinkifyContent from '@/components/widgets/LinkifyContent';
import { createRouter, mount } from '../../helpers';
import { key } from '@/lib/utils';

const createEvent = ({ keyName, href }) => ({
  key: keyName,
  preventDefault: jest.fn(),
  target: {
    getAttribute: jest.fn().mockImplementation((attribute) => {
      if (attribute === 'href') {
        return href;
      }
      return '';
    }),
  },
});

const state = {
  device: {
    isNativeApp: false,
  },
};
const defaultContent = 'lorem ipsum dolor sit amet';

describe('LinkifyContent', () => {
  let $router;
  let wrapper;

  const mountLinkifyContent = ({ content = defaultContent, id, tag } = {}) =>
    mount(LinkifyContent, {
      $router,
      state,
      propsData: {
        content,
        id,
        tag,
      },
    });

  beforeEach(() => {
    $router = createRouter();
    wrapper = mountLinkifyContent();
  });

  describe('props', () => {
    describe('tag', () => {
      it('will render root element as `DIV` by default', () => {
        expect(wrapper.is('div')).toBe(true);
      });

      describe('with `P` value', () => {
        beforeEach(() => {
          wrapper = mountLinkifyContent({ tag: 'P' });
        });

        it('will render root element as `P`', () => {
          expect(wrapper.is('p')).toBe(true);
        });
      });
    });

    describe('id', () => {
      it('will not add an `id` to root element by default', () => {
        expect(wrapper.attributes('id')).toBeUndefined();
      });

      describe('with value', () => {
        beforeEach(() => {
          wrapper = mountLinkifyContent({ id: 'default-id' });
        });

        it('will add an `id` to root element with the passed value', () => {
          expect(wrapper.attributes('id')).toBe('default-id');
        });
      });
    });

    describe('content', () => {
      it('will display the content as root element html', () => {
        expect(wrapper.text()).toEqual(defaultContent);
      });

      describe('with links', () => {
        beforeEach(() => {
          wrapper = mountLinkifyContent({ content: 'lorem ipsum dolor 111.nhs.uk sit amet' });
        });

        it('will display the content as root element html', () => {
          expect(wrapper.text()).toEqual('lorem ipsum dolor 111.nhs.uk sit amet');
        });

        it('will convert the text links to `A` tags', () => {
          const linkElement = wrapper.find('a');

          expect(linkElement.exists()).toBe(true);
          expect(linkElement.attributes().href).toEqual('https://111.nhs.uk');
          expect(linkElement.text()).toEqual('111.nhs.uk');
        });
      });
    });
  });

  describe('methods', () => {
    describe('navigateTo', () => {
      let event;

      describe('with a `/` href', () => {
        const href = '/link-to-page';

        beforeEach(() => {
          event = createEvent({ keyName: key.Enter, href });
          wrapper.vm.navigateTo(event);
        });

        it('will push href to the router', () => {
          expect($router.push).toBeCalledWith(href);
        });

        it('will call event preventDefault', () => {
          expect(event.preventDefault).toBeCalled();
        });
      });

      describe('with a `//` href', () => {
        const href = '//link-to-page';

        beforeEach(() => {
          event = createEvent({ keyName: key.Enter, href });
          wrapper.vm.navigateTo(event);
        });

        it('will not push to the router', () => {
          expect($router.push).not.toBeCalled();
        });

        it('will not call event preventDefault', () => {
          expect(event.preventDefault).not.toBeCalled();
        });
      });
    });

    describe('onKeyDown', () => {
      const href = '/link-to-page';

      describe('on Enter', () => {
        beforeEach(() => {
          const event = createEvent({ keyName: key.Enter, href });
          wrapper.vm.onKeyDown(event);
        });

        it('will push href to the router', () => {
          expect($router.push).toBeCalledWith(href);
        });
      });

      describe('on any other key', () => {
        beforeEach(() => {
          const event = createEvent({ keyName: key.ArrowLeft, href });
          wrapper.vm.onKeyDown(event);
        });

        it('will not push to the router', () => {
          expect($router.push).not.toBeCalled();
        });
      });
    });
  });
});
