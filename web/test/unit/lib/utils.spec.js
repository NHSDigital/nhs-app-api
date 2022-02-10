import each from 'jest-each';
import {
  createRouteByNameObject,
  getThirdPartyJumpOff,
  mimeType,
  isNhsAppHost,
  generateContextualHelpLink,
} from '@/lib/utils';
import * as window from '@/lib/window';

describe('util library', () => {
  describe('createRouteByNameObject', () => {
    const patientId = 'patientId';
    const name = 'foo';
    let query;
    let store;
    let params;
    let isLoggedIn;

    beforeEach(() => {
      query = {};
      isLoggedIn = false;
      params = { patientId: 'some other value' };
      store = {
        getters: {
          'linkedAccounts/getPatientId': patientId,
          'linkedAccounts/isActingOnBehalfOfPatient': false,
          'session/isLoggedIn': () => isLoggedIn,
        },
      };
    });

    describe('is logged in', () => {
      let route;

      beforeEach(() => {
        isLoggedIn = true;
        route = createRouteByNameObject({ name, query, params, store });
      });

      it('will not contain patient id', () => {
        expect(route).toStrictEqual({ name, query, params: {} });
      });

      describe('patient id is not empty', () => {
        beforeEach(() => {
          store.getters['linkedAccounts/isActingOnBehalfOfPatient'] = true;
          route = createRouteByNameObject({ name, query, params, store });
        });

        it('will contain patient id', () => {
          expect(route).toStrictEqual({ name, query, params: { patientId } });
        });
      });
    });

    describe('is not logged in', () => {
      let route;

      beforeEach(() => {
        isLoggedIn = false;
        route = createRouteByNameObject({ name, query, params, store });
      });

      it('will not contain patient id', () => {
        expect(route).toStrictEqual({ name, query, params: {} });
      });
    });
  });

  describe('mime type', () => {
    each([
      ['jpg', 'image/jpeg'],
      ['dib', 'image/bmp'],
      ['pdf', 'application/pdf'],
      ['spooby', 'application/octet-stream'],
    ]).it('will parse the %s file mime type correctly', async (type, expectedMimeType) => {
      // Act
      const mimeTypeProperty = mimeType(type);

      // Assert
      expect(mimeTypeProperty).toEqual(expectedMimeType);
    });
  });

  describe('isNhsAppHost', () => {
    beforeEach(() => {
      window.getWindowLocationOrigin = jest.fn().mockReturnValue('https://www.nhsapp.com');
    });

    it('will return true with matching url', () => {
      expect(isNhsAppHost('https://www.nhsapp.com/something')).toBe(true);
    });

    it('will return false with non matching url', () => {
      expect(isNhsAppHost('https://www.test.com')).toBe(false);
    });
  });

  describe('getThirdPartyJumpOff', () => {
    const fullUri = { acceptablePathsRegex: '^\\/full-uri\\?param=\\/another\\/path\\?tab=appointments$' };
    const fullUriWithOptionalValuesAfter = { acceptablePathsRegex: '^\\/full-uri\\?param=\\/another\\/path\\?tab=messages.*$' };
    const uriPathNoQueryString = { acceptablePathsRegex: '^\\/foo/path$' };
    const uriPathWithOptionalQueryString = { acceptablePathsRegex: '^\\/foo(\\/?\\?.*)?$' };

    const thirdPartyConfig = {
      jumpOffs: [
        fullUri,
        fullUriWithOptionalValuesAfter,
        uriPathNoQueryString,
        uriPathWithOptionalQueryString,
      ],
    };

    each([
      ['/full-uri?param=/another/path?tab=appointment', ''], // query string value too short (missing s)
      ['/full-uri?param=/another/path?tab=appointment&q=1', ''], // unexpected extra query param
      ['/full-uri?param=/another/path?tab=prescriptions', ''], // unknown query param
      ['/full-uri', ''], // no match as jump off defines that the url must have the query parms
      ['/full-uri?param=/another/path?tab=appointments', fullUri], // exact match
      ['/full-uri?param=/another/path?tab=messages&q=1', fullUriWithOptionalValuesAfter], // extra query parameters are allowed
      ['/full-uri?param=%2Fanother%2Fpath%3Ftab%3Dmessages%26q%3D1', fullUriWithOptionalValuesAfter], // extra query parameters are allowed (encoded)
      ['/foo/path', uriPathNoQueryString], // exact match
      ['/foo/path/more', ''], // path not recognised
      ['/foo', uriPathWithOptionalQueryString], // without optional query paramaters
      ['/foo/?param=value', uriPathWithOptionalQueryString], // extra query parameters after forward slash
      ['/foo?param=value', uriPathWithOptionalQueryString], // extra query parameters
    ]).it('will correctly match redirect path %s with third party jump off point', (redirectPath, expectedResultMatch) => {
      // act
      const result = getThirdPartyJumpOff(thirdPartyConfig, redirectPath);

      // assert
      expect(result).toBe(expectedResultMatch);
    });
  });

  describe('generateContextualHelpLink', () => {
    const mockStore = {
      $env: { BASE_NHS_APP_HELP_URL: 'http://stubs.local.bitraft.io' },
    };

    each([
      ['http://stubs.local.bitraft.io', null],
      ['http://stubs.local.bitraft.io', ''],
      ['http://stubs.local.bitraft.io/help/test', '/help/test'],
      ['http://stubs.local.bitraft.io/help-and-support/test', '/help-and-support/test'],
    ]).it('will correctly generate %s as the the contextual help link from store and route link %s', (expectedResult, helpPath) => {
      const mockCurrentRoute = { meta: { helpPath } };

      // act
      const result = generateContextualHelpLink(mockStore, mockCurrentRoute);

      // assert
      expect(result).toBe(expectedResult);
    });
  });
});
