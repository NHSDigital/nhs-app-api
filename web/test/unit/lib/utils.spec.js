import each from 'jest-each';
import { isFalsy, isTruthy } from '@/lib/utils';

const falseValues = [
  undefined,
  null,
  0,
  false,
  'false',
  NaN,
  '',
];

const trueValues = [
  true,
  1,
  '1',
  true,
  'true',
];

describe('util library', () => {
  describe('isFalsy', () => {
    each(falseValues)
      .it('will be falsy', value => expect(isFalsy(value)).toBe(true));

    each(trueValues)
      .it('will not be falsy', value => expect(isFalsy(value)).toBe(false));
  });

  describe('isTruthy', () => {
    each(trueValues)
      .it('will be truthy', value => expect(isTruthy(value)).toBe(true));

    each(falseValues)
      .it('will not be truthy', value => expect(isTruthy(value)).toBe(false));
  });
});
