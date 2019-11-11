import each from 'jest-each';
import { isFalsy, isTruthy, redirectTo, readableBytes, stripHtml } from '@/lib/utils';

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
        state: {
          device: {
            isNativeApp: false,
            source: 'web',
          },
        },
      },
      $router: {
        currentRoute: {
          path: '/foo',
        },
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
    describe('on server', () => {
      beforeEach(() => {
        process.server = true;
      });

      it('will call redirect with query when query is not null', () => {
        redirectTo(self, 'a-path', { query: 'aQuery' });
        expect(self.$store.app.context.redirect).toHaveBeenCalledWith(302, 'a-path', { query: 'aQuery' });
      });

      it('will call redirect without query when query is null', () => {
        process.server = true;
        redirectTo(self, 'a-path');
        expect(self.$store.app.context.redirect).toHaveBeenCalledWith('a-path');
      });
    });

    describe('on client', () => {
      beforeEach(() => {
        process.server = false;
      });

      it('will call push without a query if query is null', () => {
        redirectTo(self, 'a-path', null);
        expect(self.$router.push).toHaveBeenCalledWith('a-path');
      });

      it('will call push with a query if query is not null', () => {
        redirectTo(self, 'a-path', { query: 'aQuery' });
        expect(self.$router.push).toHaveBeenCalledWith({ path: 'a-path', query: { query: 'aQuery' } });
      });

      describe('same page', () => {
        let path;

        beforeEach(() => {
          // eslint-disable-next-line prefer-destructuring
          path = self.$router.currentRoute.path;
        });

        it('will call go if query is null', () => {
          redirectTo(self, path, null);
          expect(self.$router.go).toHaveBeenCalled();
        });

        it('will call go if query is the same as current', () => {
          const query = { source: 'ios' };
          self.$router.currentRoute.query = query;
          redirectTo(self, path, query);
          expect(self.$router.go).toHaveBeenCalled();
        });

        it('will call push with query if query is not the same as current', () => {
          const query = { source: 'ios' };
          redirectTo(self, path, query);
          expect(self.$router.push).toHaveBeenCalledWith({ path, query });
        });

        it('will call push with query if native and previously had no query', () => {
          self.$store.state.device.isNativeApp = true;
          self.$store.state.device.source = 'ios';
          redirectTo(self, path);
          expect(self.$router.push).toHaveBeenCalledWith({ path, query: { source: 'ios' } });
        });

        it('will call push with query if native and previous query is different', () => {
          self.$store.state.device.isNativeApp = true;
          self.$store.state.device.source = 'ios';
          self.$router.currentRoute.query = { value: 'boom' };
          redirectTo(self, path);
          expect(self.$router.push).toHaveBeenCalledWith({ path, query: { source: 'ios', value: 'boom' } });
        });
      });
    });
  });

  describe('readableBytes', () => {
    each([
      { bytes: 0, expectedOutput: '0B' },
      { bytes: 1, expectedOutput: '1B' },
      { bytes: 999, expectedOutput: '999B' },
      { bytes: 999.9, expectedOutput: '1KB' },
      { bytes: 1000, expectedOutput: '1KB' },
      { bytes: 1001, expectedOutput: '1KB' },
      { bytes: 1450, expectedOutput: '1KB' },
      { bytes: 1500, expectedOutput: '2KB' },
      { bytes: 999449, expectedOutput: '999KB' },
      { bytes: 1000000, expectedOutput: '1MB' },
      { bytes: 1000001, expectedOutput: '1MB' },
      { bytes: 1010000, expectedOutput: '1.01MB' },
      { bytes: 1200000, expectedOutput: '1.2MB' },
      { bytes: 1019500, expectedOutput: '1.02MB' },
      { bytes: 1500000, expectedOutput: '1.5MB' },
    ]).it('will convert a number of bytes into a readable format', ({ bytes, expectedOutput }) => {
      // Act
      const output = readableBytes(bytes);

      // Assert
      expect(output).toEqual(expectedOutput);
    });

    each([
      'a random string',
      {},
      -11234,
      -1.2,
    ]).it('will return the value if it is not a number or is negative', (bytes) => {
      // Act
      const output = readableBytes(bytes);

      // Assert
      expect(output).toEqual(bytes);
    });
  });

  describe('stripHtml', () => {
    let result;

    beforeEach(() => {
      result = stripHtml('Sample <b>content</b> with html');
    });

    it('will return sanitized content', () => {
      expect(result).toBe('Sample content with html');
    });
  });
});
