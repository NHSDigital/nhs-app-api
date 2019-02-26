import each from 'jest-each';
import { isFalsy, isTruthy, redirectTo } from '@/lib/utils';

let self;

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
  beforeEach(() => {
    self = {
      $store: {
        app: {
          context: {
            redirect: jest.fn(),
          },
        },
      },
      $router: {
        go: jest.fn(),
        push: jest.fn(),
      },
    };
  });

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

  describe('redirectTo', () => {
    it('will call redirect with query when query is not null', () => {
      process.server = true;
      redirectTo(self, 'a-path', { query: 'aQuery' });
      expect(self.$store.app.context.redirect).toHaveBeenCalledWith(302, 'a-path', { query: 'aQuery' });
    });

    it('will call redirect without query when query is null', () => {
      process.server = true;
      redirectTo(self, 'a-path');
      expect(self.$store.app.context.redirect).toHaveBeenCalledWith('a-path');
    });

    it('will call push without a query if process.server is false and query is null', () => {
      process.server = false;
      redirectTo(self, 'a-path', null);
      expect(self.$router.push).toHaveBeenCalledWith('a-path');
    });

    it('will call push with a query if process.server is false and query is not null', () => {
      process.server = false;
      redirectTo(self, 'a-path', { query: 'aQuery' });
      expect(self.$router.push).toHaveBeenCalledWith({ path: 'a-path', query: { query: 'aQuery' } });
    });
  });
});
