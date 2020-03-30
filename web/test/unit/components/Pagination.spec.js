import Pagination from '@/components/Pagination';
import { mount } from '../helpers';

const mountPagination = ({
  previousLink,
  previousTitle,
  nextLink,
  nextTitle,
}) => mount(Pagination, {
  propsData: {
    previousLink,
    previousTitle,
    nextLink,
    nextTitle,
  },
});

describe('Pagination', () => {
  const previousClass = '.nhsuk-pagination-item--previous';
  const nextClass = '.nhsuk-pagination-item--next';
  let wrapper;

  describe('with previous and next pages', () => {
    beforeEach(() => {
      wrapper = mountPagination({
        previousLink: 'previousLink',
        previousTitle: 'previousTitle',
        nextLink: 'nextLink',
        nextTitle: 'nextTitle',
      });
    });

    describe('next page', () => {
      let nextPage;

      beforeEach(() => {
        nextPage = wrapper.find(nextClass);
      });

      it('will show', () => {
        expect(nextPage.exists()).toBe(true);
      });

      it('will point the passed link', () => {
        expect(nextPage.find('a').attributes('href')).toBe('nextLink');
      });
    });

    describe('previous page', () => {
      let previousPage;

      beforeEach(() => {
        previousPage = wrapper.find(previousClass);
      });

      it('will show', () => {
        expect(previousPage.exists()).toBe(true);
      });

      it('will point the passed link', () => {
        expect(previousPage.find('a').attributes('href')).toBe('previousLink');
      });
    });
  });

  describe('with next page', () => {
    beforeEach(() => {
      wrapper = mountPagination({
        nextLink: 'nextLink',
        nextTitle: 'nextTitle',
      });
    });

    describe('next page', () => {
      let nextPage;

      beforeEach(() => {
        nextPage = wrapper.find(nextClass);
      });

      it('will show', () => {
        expect(nextPage.exists()).toBe(true);
      });

      it('will point the passed link', () => {
        expect(nextPage.find('a').attributes('href')).toBe('nextLink');
      });
    });

    describe('previous page', () => {
      let previousPage;

      beforeEach(() => {
        previousPage = wrapper.find(previousClass);
      });

      it('will not show', () => {
        expect(previousPage.exists()).toBe(false);
      });
    });
  });

  describe('with previous page', () => {
    beforeEach(() => {
      wrapper = mountPagination({
        previousLink: 'previousLink',
        previousTitle: 'previousTitle',
      });
    });

    describe('next page', () => {
      let nextPage;

      beforeEach(() => {
        nextPage = wrapper.find(nextClass);
      });

      it('will not show', () => {
        expect(nextPage.exists()).toBe(false);
      });
    });

    describe('previous page', () => {
      let previousPage;

      beforeEach(() => {
        previousPage = wrapper.find(previousClass);
      });

      it('will show', () => {
        expect(previousPage.exists()).toBe(true);
      });

      it('will point the passed link', () => {
        expect(previousPage.find('a').attributes('href')).toBe('previousLink');
      });
    });
  });

  describe('with no pages', () => {
    beforeEach(() => {
      wrapper = mountPagination({});
    });

    describe('next page', () => {
      let nextPage;

      beforeEach(() => {
        nextPage = wrapper.find(nextClass);
      });

      it('will not show', () => {
        expect(nextPage.exists()).toBe(false);
      });
    });

    describe('previous page', () => {
      let previousPage;

      beforeEach(() => {
        previousPage = wrapper.find(previousClass);
      });

      it('will not show', () => {
        expect(previousPage.exists()).toBe(false);
      });
    });
  });

  describe('with partial props complete', () => {
    beforeEach(() => {
      wrapper = mountPagination({
        previousLink: 'previousLink',
        nextLink: 'nextLink',
      });
    });

    describe('next page', () => {
      let nextPage;

      beforeEach(() => {
        nextPage = wrapper.find(nextClass);
      });

      it('will not show', () => {
        expect(nextPage.exists()).toBe(false);
      });
    });

    describe('previous page', () => {
      let previousPage;

      beforeEach(() => {
        previousPage = wrapper.find(previousClass);
      });

      it('will not show', () => {
        expect(previousPage.exists()).toBe(false);
      });
    });
  });
});
