import ErrorParagraph from '@/components/errors/ErrorParagraph';
import { locale, mount } from '../../helpers';

const mountWrapper = ({ $t, from, variable }) => mount(ErrorParagraph, {
  $t,
  propsData: {
    from,
    variable,
  },
});

describe('ErrorParagraph', () => {
  let wrapper;

  describe('from', () => {
    describe('translates to text', () => {
      beforeEach(() => {
        wrapper = mountWrapper({ from: 'foo.text' });
      });

      describe('paragraph', () => {
        let paragraph;

        beforeEach(() => {
          paragraph = wrapper.find('p');
        });

        it('will exist', () => {
          expect(paragraph.exists()).toBe(true);
        });

        it('will not have arial label', () => {
          expect(paragraph.attributes('aria-label')).toBeUndefined();
        });

        it('will translate `from`', () => {
          expect(paragraph.text()).toBe('translate_foo.text');
        });
      });
    });

    describe('translates to object', () => {
      beforeEach(() => {
        locale.foo = {
          object: {
            text: 'foo text',
            label: 'foo label',
          },
        };
        wrapper = mountWrapper({ from: 'foo.object' });
      });

      describe('paragraph', () => {
        let paragraph;

        beforeEach(() => {
          paragraph = wrapper.find('p');
        });

        it('will exist', () => {
          expect(paragraph.exists()).toBe(true);
        });

        it('will have arial label', () => {
          expect(paragraph.attributes('aria-label')).toBe('foo label');
        });

        it('will display `from` text', () => {
          expect(paragraph.text()).toBe('foo text');
        });
      });
    });
  });

  describe('no variable', () => {
    let variable;

    beforeEach(() => {
      variable = null;
    });

    describe('translated text contains {errorcode}', () => {
      beforeEach(() => {
        const from = 'foo.text';
        const $t = jest.fn().mockImplementation((key) => {
          switch (key) {
            case 'foo.text':
              return 'foo {errorCode} text';
            default:
              return undefined;
          }
        });
        wrapper = mountWrapper({ $t, from, variable });
      });

      it('will not display paragraph', () => {
        expect(wrapper.find('p').exists()).toBe(false);
      });
    });
  });
});
