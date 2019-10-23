import { formatDate, longDate } from '@/plugins/filters';

describe('filters', () => {
  describe('longDate', () => {
    let result;

    beforeEach(() => {
      result = longDate('2019-09-05T02:15:12.356Z');
    });

    it('will convert timezone to `Europe/London` and format to `D MMMM YYYY`', () => {
      expect(result).toBe('5 September 2019');
    });

    describe('no date time', () => {
      beforeEach(() => {
        result = longDate();
      });

      it('will return empty string', () => {
        expect(result).toBe('');
      });
    });
  });

  describe('formatDate', () => {
    let result;

    describe('no date time', () => {
      beforeEach(() => {
        result = formatDate();
      });

      it('will return empty string', () => {
        expect(result).toBe('');
      });
    });

    describe('no format', () => {
      beforeEach(() => {
        result = formatDate('2019-09-14T02:15:12.356Z');
      });

      it('will convert timezone to `Europe/London` and use default format (ISO 8601)', () => {
        expect(result).toBe('2019-09-14T03:15:12+01:00');
      });
    });

    describe('YYYY-MM-DD h:mma format', () => {
      beforeEach(() => {
        result = formatDate('2019-09-14T02:15:12.356Z', 'YYYY-MM-DD h:mma');
      });

      it('will convert timezone to `Europe/London` and format', () => {
        expect(result).toBe('2019-09-14 3:15am');
      });
    });
  });
});
