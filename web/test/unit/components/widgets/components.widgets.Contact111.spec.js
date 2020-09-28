import Contact111 from '@/components/widgets/Contact111';
import { mount } from '../../helpers';

describe('Contact111', () => {
  const mountContact111 = ({
    text = 'some text',
    ariaLabel = undefined,
    clazz = undefined,
  } = {}) => mount(Contact111, {
    propsData: {
      text,
      ariaLabel,
      clazz,
    },
  });

  let wrapper;
  let paragraph;
  let link;

  beforeEach(() => {
    wrapper = mountContact111();
    paragraph = wrapper.find('p');
    link = wrapper.find('a');
  });

  describe('exists', () => {
    it('will exist', () => {
      expect(paragraph.exists()).toBe(true);
    });

    it('will not have arial label', () => {
      expect(paragraph.attributes('aria-label')).toBeUndefined();
    });

    it('will set text', () => {
      expect(paragraph.text()).toContain('some text');
      expect(paragraph.text()).toContain('111.nhs.uk');
      expect(paragraph.text()).toContain('or call 111.');
    });

    it('will contain hyperlink', () => {
      expect(link.exists()).toBe(true);
    });

    it(' hyperlink goes to 111 website on new page', () => {
      expect(link.attributes().target).toEqual('_blank');
      expect(link.attributes().href).toEqual('https://111.nhs.uk');
    });
  });

  describe('aria label', () => {
    it('will set aria label', () => {
      wrapper = mountContact111({ ariaLabel: 'aria label contents' });
      expect(wrapper.find('p').attributes('aria-label')).toBe('aria label contents');
    });
  });

  describe('class', () => {
    it('will set the class', () => {
      wrapper = mountContact111({ clazz: 'some-class' });
      expect(wrapper.find('p').attributes('class')).toBe('some-class');
    });
  });
});
