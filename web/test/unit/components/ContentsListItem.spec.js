import ContentsListItem from '@/components/ContentsListItem';
import { mount } from '../helpers';

const mountContentsListItem = ({ href, routeName }) => mount(ContentsListItem, {
  $route: {
    path: routeName,
  },
  propsData: {
    id: 'id',
    href,
  },
});

describe('Contents List Item', () => {
  const currentPageClass = '.nhsuk-contents-list__current';
  const otherPageClass = '.nhsuk-contents-list__link';
  let wrapper;

  describe('when it is the current page', () => {
    beforeEach(() => {
      wrapper = mountContentsListItem({
        href: '/data-sharing/where-used',
        routeName: '/data-sharing/where-used',
      });
    });

    it('will display as a text', () => {
      expect(wrapper.find(currentPageClass).exists()).toBe(true);
    });

    it('will not be displayed as a link', () => {
      expect(wrapper.find(otherPageClass).exists()).toBe(false);
    });
  });

  describe('when it is not the current page', () => {
    beforeEach(() => {
      wrapper = mountContentsListItem({
        href: '/data-sharing/where-used',
        routeName: '/data-sharing',
      });
    });

    it('will not be displayed as a text', () => {
      expect(wrapper.find(currentPageClass).exists()).toBe(false);
    });

    it('will display as a link', () => {
      expect(wrapper.find(otherPageClass).exists()).toBe(true);
    });
  });
});
