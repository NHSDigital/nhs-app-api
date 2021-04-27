import each from 'jest-each';
import {
  normaliseWhiteSpace,
  readableBytes,
  stripHtml,
} from '@/lib/utils';

describe('util library format', () => {
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

  describe('normaliseWhiteSpace', () => {
    each([undefined, null, '', {}, 4])
      .it('will return the given argument if it is not a string or is blank', (text) => {
        expect(normaliseWhiteSpace(text)).toEqual(text);
      });

    each([
      ['\n\rmultiple\nnew\nlines', ' multiple new lines'],
      ['more\n\rnew\r lines\r\n\r', 'more new lines '],
      ['lots    of \n\n  \r\n    spaces', 'lots of spaces'],
    ]).it('will normalise all white space sequences to a single space', (text, formattedText) => {
      expect(normaliseWhiteSpace(text)).toEqual(formattedText);
    });
  });
});
