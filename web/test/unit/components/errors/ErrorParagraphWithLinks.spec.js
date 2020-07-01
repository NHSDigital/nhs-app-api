import ErrorParagraphWithLinks from '@/components/errors/ErrorParagraphWithLinks';
import { mount } from '../../helpers';

const CONTACT_US_URL = 'http://contact.us/';

const mountWrapper = ({ $t, from, queryParam }) => mount(ErrorParagraphWithLinks, {
  $env: {
    CONTACT_US_URL,
  },
  $t,
  propsData: {
    from,
    queryParam,
  },
});

describe('ErrorParagraphWithLinks', () => {
  let wrapper;

  describe('from', () => {
    describe('translates to link', () => {
      const from = 'foo.text';
      let paragraph;
      let links;

      describe('single link', () => {
        const contents = [
          {
            text: 'Go to ',
            linkUrl: 'https://example.com',
            linkText: 'example link',
          },
          {
            text: ' or call 111.',
          },
        ];

        beforeEach(() => {
          const $t = jest.fn().mockImplementation((key) => {
            switch (key) {
              case 'foo.text':
                return contents;
              default:
                return undefined;
            }
          });

          wrapper = mountWrapper({ $t, from });
          paragraph = wrapper.find('p');
          links = paragraph.findAll('a');
        });

        it('will exist', () => {
          expect(paragraph.exists()).toBe(true);
          expect(links.exists()).toBe(true);
          expect(links.length).toBe(1);
        });

        it('will translate `from`', () => {
          expect(links.at(0).attributes('href')).toBe('https://example.com');
          expect(paragraph.text()).toContain('Go to');
          expect(paragraph.text()).toContain('example link');
          expect(paragraph.text()).toContain('or call 111.');
        });
      });

      describe('multiple links', () => {
        const contents = [
          {
            text: 'Go to ',
            linkUrl: 'https://example.com',
            linkText: 'link 1',
          },
          {
            text: ' or ',
            linkUrl: 'https://google.com',
            linkText: 'link 2',
          },
          {
            text: ' or call 111.',
          },
        ];

        beforeEach(() => {
          const $t = jest.fn().mockImplementation((key) => {
            switch (key) {
              case 'foo.text':
                return contents;
              default:
                return undefined;
            }
          });

          wrapper = mountWrapper({ $t, from });
          paragraph = wrapper.find('p');
          links = paragraph.findAll('a');
        });

        it('will exist', () => {
          expect(paragraph.exists()).toBe(true);

          expect(links.exists()).toBe(true);
          expect(links.length).toBe(2);
        });

        it('will translate `from`', () => {
          expect(links.at(0).attributes('href')).toBe('https://example.com');
          expect(links.at(1).attributes('href')).toBe('https://google.com');

          expect(paragraph.text()).toContain('Go to');
          expect(paragraph.text()).toContain('link 1');
          expect(paragraph.text()).toContain('or');
          expect(paragraph.text()).toContain('link 2');
          expect(paragraph.text()).toContain('or call 111.');
        });
      });

      describe('with query param', () => {
        const contents = [
          {
            text: 'If you still need to access the app, ',
            linkText: 'contact us',
          },
        ];
        const queryParam = {
          param: 'errorcode',
          value: '3fxxxx',
        };

        beforeEach(() => {
          const $t = jest.fn().mockImplementation((key) => {
            switch (key) {
              case 'foo.text':
                return contents;
              default:
                return undefined;
            }
          });

          wrapper = mountWrapper({ $t, from, queryParam });
          paragraph = wrapper.find('p');
          links = paragraph.findAll('a');
        });

        it('will exist', () => {
          expect(paragraph.exists()).toBe(true);
          expect(links.exists()).toBe(true);
          expect(links.length).toBe(1);
        });

        it('will translate `from`', () => {
          expect(links.at(0).attributes('href')).toBe(`${CONTACT_US_URL}?${queryParam.param}=${queryParam.value}`);

          expect(paragraph.text()).toContain('If you still need to access the app,');
          expect(paragraph.text()).toContain('contact us');
        });
      });
    });
  });
});
