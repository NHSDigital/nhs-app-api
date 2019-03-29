import namedObjectComparator from '@/lib/comparators';


describe('comparators', () => {
  describe('namedObjectComparator based on natural order', () => {
    it('should return -1 where the first named item is not supplied however another named item is.', () =>
      expect(namedObjectComparator(undefined, { name: 'b' })).toEqual(-1));

    it('should return -1 where the first named item name is not supplied however another named item is.', () =>
      expect(namedObjectComparator({ name: undefined }, { name: 'b' })).toEqual(-1));

    it('should return -1 where the first named item is numeric before another named item.', () =>
      expect(namedObjectComparator({ name: '0' }, { name: 'a' })).toEqual(-1));

    it('should return -1 where the first named item is before another named item.', () =>
      expect(namedObjectComparator({ name: 'a' }, { name: 'b' })).toEqual(-1));

    it('should return -1 where the first named item is before another named item ignoring case.', () =>
      expect(namedObjectComparator({ name: 'A' }, { name: 'b' })).toEqual(-1));

    it('should return -1 where the first named item is before another named item including natural numeric sort order.', () =>
      expect(namedObjectComparator({ name: 'z2' }, { name: 'z11' })).toEqual(-1));

    it('should return -1 where the first named item is before another named item ' +
      'including natural case insensitive numeric sort order.', () =>
      expect(namedObjectComparator({ name: 'aBc' }, { name: 'bCd' })).toEqual(-1));

    it('should return 0 where no item is supplied.', () =>
      expect(namedObjectComparator(undefined, undefined)).toEqual(0));

    it('should return 0 where no item name is supplied.', () =>
      expect(namedObjectComparator({ name: undefined }, { name: undefined })).toEqual(0));

    it('should return 0 where the first named item is equal another named item.', () =>
      expect(namedObjectComparator({ name: 'a' }, { name: 'a' })).toEqual(0));

    it('should return 0 where the first named item is equal another named item ignoring case.', () =>
      expect(namedObjectComparator({ name: 'a' }, { name: 'A' })).toEqual(0));

    it('should return +1 where the second named item is not supplied however the first named item is.', () =>
      expect(namedObjectComparator({ name: 'b' }, undefined)).toEqual(1));

    it('should return +1 where the second named item name is not supplied however the first named item is.', () =>
      expect(namedObjectComparator({ name: 'b' }, { name: undefined })).toEqual(1));

    it('should return +1 where the first named item is after another numeric named item.', () =>
      expect(namedObjectComparator({ name: 'a' }, { name: '0' })).toEqual(1));

    it('should return +1 where the first named item is after another named item.', () =>
      expect(namedObjectComparator({ name: 'b' }, { name: 'a' })).toEqual(1));

    it('should return +1 where the first named item is after another named item ignoring case.', () =>
      expect(namedObjectComparator({ name: 'Z' }, { name: 'a' })).toEqual(1));

    it('should return +1 where the first named item is after another named item including natural numeric sort order.', () =>
      expect(namedObjectComparator({ name: 'z11' }, { name: 'z2' })).toEqual(1));

    it('should return +1 where the first named item is after another named ' +
      'item including natural case insensitive numeric sort order.', () =>
      expect(namedObjectComparator({ name: 'bCd' }, { name: 'AbC' })).toEqual(1));
  });
});
