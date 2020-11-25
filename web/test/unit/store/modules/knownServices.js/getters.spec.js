
import getters from '@/store/modules/knownServices/getters';

describe('known serivces getters', () => {
  describe('matchOneById', () => {
    const { matchOneById } = getters;
    let currentState;

    beforeEach(() => {
      currentState = {
        knownServices: [
          {
            id: 'first',
            url: 'https://www.first.com',
          },
          {
            id: 'second',
            url: 'https://www.second.com',
          },
        ],
      };
    });

    it('will return service for an existing id', () => {
      expect(matchOneById(currentState)('second')).toStrictEqual({ id: 'second', url: 'https://www.second.com' });
    });

    it('will return undefined from an unknown id', () => {
      expect(matchOneById(currentState)('first.more')).toBeUndefined();
    });
  });

  describe('matchOneByUrl', () => {
    const { matchOneByUrl } = getters;
    let currentState;

    beforeEach(() => {
      currentState = {
        knownServices: [
          {
            id: 'first',
            url: 'https://www.first.com',
          },
          {
            id: 'second',
            url: 'https://www.second.com',
          },
        ],
      };
    });

    it('will return service for an exact url match', () => {
      expect(matchOneByUrl(currentState)('https://www.second.com')).toStrictEqual({ id: 'second', url: 'https://www.second.com' });
    });

    it('will return service for starting url match', () => {
      expect(matchOneByUrl(currentState)('https://www.first.com/query/value')).toStrictEqual({ id: 'first', url: 'https://www.first.com' });
    });

    it.each([
      ['incomplete', 'https//www.first'],
      ['missing start', '//www.first.com/query/value'],
      ['unknown other', 'https://www.other.com'],
    ])('will return undefined from an %s url', (_, url) => {
      expect(matchOneByUrl(currentState)(url)).toBeUndefined();
    });
  });
});
