import MarkdownContent from '@/components/widgets/MarkdownContent';
import each from 'jest-each';
import { key } from '@/lib/utils';
import { createRouter, createStore, mount, normaliseNewLines } from '../../helpers';

const createEvent = ({ keyName, href, target = '' }) => ({
  key: keyName,
  preventDefault: jest.fn(),
  target: {
    getAttribute: jest.fn().mockImplementation((attribute) => {
      if (attribute === 'href') {
        return href;
      }
      if (attribute === 'target') {
        return target;
      }
      return '';
    }),
  },
});

describe('MarkdownContent', () => {
  let expectedMessageId;
  let $router;
  let $store;
  let wrapper;

  const mountMarkdownContent = ({
    content = 'lorem ipsum dolor sit amet',
    id,
    messageId = '1234-abcd',
    hasKnownService = false,
    requiresAssertedLoginIdentity = false,
  } = {}) => {
    expectedMessageId = messageId;
    $store = createStore({
      getters: {
        'session/isLoggedIn': jest.fn(),
        'knownServices/matchOneByUrl': jest.fn().mockImplementation((url) => {
          if (hasKnownService) {
            return {
              id: 'pkb',
              requiresAssertedLoginIdentity,
              url,
            };
          }
          return undefined;
        }),
      },
    });
    return mount(MarkdownContent, {
      $router,
      $store,
      propsData: {
        content,
        id,
        messageId,
      },
    });
  };

  beforeEach(() => {
    $router = createRouter();
    wrapper = mountMarkdownContent();
  });

  describe('props', () => {
    describe('id', () => {
      it('will not add an `id` to root element by default', () => {
        expect(wrapper.attributes('id')).toBeUndefined();
      });

      describe('with value', () => {
        beforeEach(() => {
          wrapper = mountMarkdownContent({ id: 'default-id' });
        });

        it('will add an `id` to root element with the passed value', () => {
          expect(wrapper.attributes('id')).toBe('default-id');
        });
      });
    });

    describe('content', () => {
      it('will display the content as root element html', () => {
        expect(wrapper.find('p').html()).toEqual('<p>lorem ipsum dolor sit amet</p>');
      });

      describe('with allowed markdown', () => {
        each([
          ['**', 'strong'],
          ['__', 'strong'],
          ['*', 'em'],
          ['_', 'em'],
        ]).describe('%s emphasis tag', (markdownTag, tag) => {
          beforeEach(() => {
            wrapper = mountMarkdownContent({ content: `${markdownTag}Lorem ipsum dolor sit amet${markdownTag}` });
          });

          it(`will render ${tag} tag`, () => {
            expect(wrapper.find('p > *').html()).toEqual(`<${tag}>Lorem ipsum dolor sit amet</${tag}>`);
          });
        });

        each([
          ['+', 'ul'],
          ['-', 'ul'],
          ['*', 'ul'],
          ['1.', 'ol'],
        ]).describe('%s list tag', (markdownTag, tag) => {
          beforeEach(() => {
            wrapper = mountMarkdownContent({ content: `${markdownTag} Lorem ipsum dolor sit amet` });
          });

          it(`will render ${tag} tag`, () => {
            expect(normaliseNewLines(wrapper.find('div > *').html())).toEqual(`<${tag}><li>Lorem ipsum dolor sit amet</li></${tag}>`);
          });
        });

        describe('newlines', () => {
          let brElement;

          beforeEach(() => {
            wrapper = mountMarkdownContent({ content: 'Lorem ipsum  \r\ndolor sit amet' });
            brElement = wrapper.find('br');
          });

          it('will render newline', () => {
            expect(brElement.exists()).toBe(true);
          });
        });

        describe('images', () => {
          let imageElement;

          beforeEach(() => {
            wrapper = mountMarkdownContent({ content: '![Lorem ipsum dolor sit amet](https://test.com/images.png)' });
            imageElement = wrapper.find('img');
          });

          it('will render image', () => {
            expect(imageElement.exists()).toBe(true);
          });

          it('will set src attribute', () => {
            expect(imageElement.attributes().src).toEqual('https://test.com/images.png');
          });

          it('will set alt text', () => {
            expect(imageElement.attributes().alt).toEqual('Lorem ipsum dolor sit amet');
          });
        });

        describe('internal markdown links', () => {
          let linkElement;

          beforeEach(() => {
            wrapper = mountMarkdownContent({ content: '[Lorem ipsum dolor sit amet](/test/path)' });
            linkElement = wrapper.find('a');
          });

          it('will render link', () => {
            expect(linkElement.exists()).toBe(true);
          });

          it('will set href attribute', () => {
            expect(linkElement.attributes().href).toEqual('/test/path');
          });

          it('will set target attribute to _self', () => {
            expect(linkElement.attributes().target).toEqual('_self');
          });

          it('will set link text', () => {
            expect(linkElement.text()).toEqual('Lorem ipsum dolor sit amet');
          });
        });

        describe('external markdown links', () => {
          let linkElement;

          beforeEach(() => {
            wrapper = mountMarkdownContent({ content: '[Lorem ipsum dolor sit amet](https://test.com)' });
            linkElement = wrapper.find('a');
          });

          it('will render link', () => {
            expect(linkElement.exists()).toBe(true);
          });

          it('will set href attribute', () => {
            expect(linkElement.attributes().href).toEqual('https://test.com');
          });

          it('will set target attribute to _blank', () => {
            expect(linkElement.attributes().target).toEqual('_blank');
          });

          it('will set link text', () => {
            expect(linkElement.text()).toEqual('Lorem ipsum dolor sit amet');
          });
        });

        describe('external markdown links to a knownSerivce', () => {
          let linkElement;

          beforeEach(() => {
            wrapper = mountMarkdownContent({
              content: '[Lorem ipsum dolor sit amet](https://test.com)',
              hasKnownService: true,
              requiresAssertedLoginIdentity: false,
            });
            linkElement = wrapper.find('a');
          });

          it('will render link', () => {
            expect(linkElement.exists()).toBe(true);
          });

          it('will set href attribute', () => {
            expect(linkElement.attributes().href).toEqual('https://test.com');
          });

          it('will set target attribute to _blank', () => {
            expect(linkElement.attributes().target).toEqual('_blank');
          });

          it('will set link text', () => {
            expect(linkElement.text()).toEqual('Lorem ipsum dolor sit amet');
          });
        });

        describe('external markdown links to a knownSerivce that requires asserted login identity', () => {
          let linkElement;

          beforeEach(() => {
            wrapper = mountMarkdownContent({
              content: '[Lorem ipsum dolor sit amet](https://ssotest.com)',
              hasKnownService: true,
              requiresAssertedLoginIdentity: true,
            });
            linkElement = wrapper.find('a');
          });

          it('will render link', () => {
            expect(linkElement.exists()).toBe(true);
          });

          it('will set href attribute', () => {
            expect(linkElement.attributes().href).toEqual('/redirector?redirect_to=https%3A%2F%2Fssotest.com');
          });

          it('will set target attribute to _blank', () => {
            expect(linkElement.attributes().target).toEqual('_blank');
          });

          it('will set link text', () => {
            expect(linkElement.text()).toEqual('Lorem ipsum dolor sit amet');
          });
        });

        describe('external markdown links with titles', () => {
          let linkElement;

          beforeEach(() => {
            wrapper = mountMarkdownContent({ content: '[Lorem ipsum dolor sit amet](https://test.com "Opens in a new window")' });
            linkElement = wrapper.find('a');
          });

          it('will render link', () => {
            expect(linkElement.exists()).toBe(true);
          });

          it('will set href attribute', () => {
            expect(linkElement.attributes().href).toEqual('https://test.com');
          });

          it('will set title attribute', () => {
            expect(linkElement.attributes().title).toEqual('Opens in a new window');
          });

          it('will set target attribute to _blank', () => {
            expect(linkElement.attributes().target).toEqual('_blank');
          });

          it('will set link text', () => {
            expect(linkElement.text()).toEqual('Lorem ipsum dolor sit amet');
          });
        });

        describe('email address links with', () => {
          let linkElement;

          beforeEach(() => {
            wrapper = mountMarkdownContent({ content: '[email@address.com](mailto:email@address.com)' });
            linkElement = wrapper.find('a');
          });

          it('will render link', () => {
            expect(linkElement.exists()).toBe(true);
          });

          it('will set href attribute', () => {
            expect(linkElement.attributes().href).toEqual('mailto:email@address.com');
          });

          it('will set link text', () => {
            expect(linkElement.text()).toEqual('email@address.com');
          });
        });

        describe('tel links with', () => {
          let linkElement;

          beforeEach(() => {
            wrapper = mountMarkdownContent({ content: '[Call 111](tel:111)' });
            linkElement = wrapper.find('a');
          });

          it('will render link', () => {
            expect(linkElement.exists()).toBe(true);
          });

          it('will set href attribute', () => {
            expect(linkElement.attributes().href).toEqual('tel:111');
          });

          it('will set link text', () => {
            expect(linkElement.text()).toEqual('Call 111');
          });
        });

        describe.each([
          ['1', '#', 'h1'],
          ['2', '##', 'h2'],
          ['3', '###', 'h3'],
          ['4', '####', 'h4'],
          ['5', '#####', 'h5'],
          ['6', '######', 'h6'],
        ])('heading %s', (_, markup, elementType) => {
          let heading;

          beforeEach(() => {
            wrapper = mountMarkdownContent({ content: `${markup} Lorem ipsum dolor sit amet` });
            heading = wrapper.find(elementType);
          });

          it('will render', () => {
            expect(heading.exists()).toBe(true);
          });

          it('will set text', () => {
            expect(heading.text()).toEqual('Lorem ipsum dolor sit amet');
          });
        });

        describe('with not allowed markdown', () => {
          each([
            ['horizontal rule (_)', '___', '<hr>'],
            ['horizontal rule (*)', '***', '<hr>'],
            ['horizontal rule (*)', '***', '<hr>'],
            ['blockquotes', '> Blockquotes can also be nested', '<blockquote>'],
            ['strikethrough', '~~Strikethrough~~', '<s>'],
            ['Inline code', 'Inline `code`', '<code>'],
            ['Inline code', 'Inline `code`', '<code>'],
            ['indented codeblock', '\tintented code\n\tsecondline', '<pre>'],
            ['backtick codeblock', '```\nintented code\nsecondline\n```', '<pre>'],
            ['table', '| Option | Description |\n| ------ | ----------- |\n| data | Lorem ipsum. |', '<table>'],
            ['inserted text', '++Inserted text++', '<ins>'],
            ['marked text', '==Marked text==', '<mark>'],
            ['angle bracket links', '<http://www.test.com>', '<a>'],
          ]).describe('%s markdown tag', (_, content, tag) => {
            beforeEach(() => {
              wrapper = mountMarkdownContent({ content });
            });

            it(`will not render ${tag}`, () => {
              expect(wrapper.find('p').html()).toEqual(expect.not.stringContaining(tag));
            });
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
            expect($router.push).toBeCalledWith({ path: href });
          });

          it('will call event preventDefault', () => {
            expect(event.preventDefault).toBeCalled();
          });

          it('will not dispatch', () => {
            expect($store.dispatch).not.toBeCalled();
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

          it('will not dispatch', () => {
            expect($store.dispatch).not.toBeCalled();
          });
        });

        describe('with an external link', () => {
          const href = 'https://link-to-external-site.com';
          const target = '_blank';

          beforeEach(() => {
            event = createEvent({ keyName: key.Enter, href, target });
            wrapper.vm.navigateTo(event);
          });

          it('will not push to the router', () => {
            expect($router.push).not.toBeCalled();
          });

          it('will not call event preventDefault', () => {
            expect(event.preventDefault).not.toBeCalled();
          });

          it('will dispatch `messaging/linkClicked` with correct arguments ', () => {
            expect($store.dispatch).toBeCalledWith('messaging/linkClicked', {
              link: href,
              messageId: expectedMessageId,
            });
          });
        });

        describe('with a redirector external link', () => {
          const href = '/redirector?redirect_to=https://link-to-external-site.com';
          const target = '_blank';

          beforeEach(() => {
            event = createEvent({ keyName: key.Enter, href, target });
            wrapper.vm.navigateTo(event);
          });

          it('will not push to the router', () => {
            expect($router.push).not.toBeCalled();
          });

          it('will not call event preventDefault', () => {
            expect(event.preventDefault).not.toBeCalled();
          });

          it('will not dispatch', () => {
            expect($store.dispatch).not.toBeCalled();
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
            expect($router.push).toBeCalledWith({ path: href });
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
});
