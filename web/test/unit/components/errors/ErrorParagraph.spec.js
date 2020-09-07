import ErrorParagraph from '@/components/errors/ErrorParagraph';
import { mount } from '../../helpers';

const mountWrapper = ({ mocks = {}, from, variable }) => mount(ErrorParagraph, {
  propsData: {
    from,
    variable,
  },
  mocks,
});

describe('ErrorParagraph', () => {
  let wrapper;

  describe('from', () => {
    describe('translates to text', () => {
      beforeEach(() => {
        wrapper = mountWrapper({ from: 'appTitle' });
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
          expect(paragraph.text()).toBe('NHS App');
        });
      });
    });

    describe('translates to object', () => {
      beforeEach(() => {
        wrapper = mountWrapper({ from: 'login.authReturn.ifYouNeed' });
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
          expect(paragraph.attributes('aria-label')).toBe('If you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, visit 111.nhs.uk or call one one one.');
        });

        it('will display `from` text', () => {
          expect(paragraph.text()).toBe('If you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, visit 111.nhs.uk or call 111.');
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
        wrapper = mountWrapper({ mocks: { $t }, from, variable });
      });

      it('will not display paragraph', () => {
        expect(wrapper.find('p').exists()).toBe(false);
      });
    });
  });
});
