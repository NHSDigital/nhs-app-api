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
          'linkedAccounts/isPatientIdNotEmpty': false,
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
          store.getters['linkedAccounts/isPatientIdNotEmpty'] = true;
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
    const pkbJumpOffWithQueryString = { redirectPath: '/pkb.com/sso?jump=appointments' };
    const ersJumpOffNoQueryString = { redirectPath: '/ers.com/login' };
    const pkbCieEncodedPartsInQuery = {
      redirectPath: '/nhs-login/login?phrPath=%2Ftest%2FmyTests.action&brand=cie',
    };
    const thirdPartyConfig = {
      jumpOffs: [pkbJumpOffWithQueryString, ersJumpOffNoQueryString, pkbCieEncodedPartsInQuery],
    };

    each([{
      redirectPath: '/pkb.com/sso?jump=appointment', // query string value too short (missing s)
      expectedResultMatch: '',
    }, {
      redirectPath: '/pkb.com/sso?jump=appointments&q=1', // unexpected extra query param
      expectedResultMatch: '',
    }, {
      redirectPath: '/pkb.com/sso?q=1', // doesn't have required query param
      expectedResultMatch: '',
    }, {
      redirectPath: '/pkb.com/sso', // no match as jump off defines that the url must have the query parms
      expectedResultMatch: '',
    }, {
      redirectPath: '/pkb.com/sso?jump=appointments',
      expectedResultMatch: pkbJumpOffWithQueryString,
    }, {
      redirectPath: '/ers.com/login',
      expectedResultMatch: ersJumpOffNoQueryString,
    }, {
      redirectPath: '/ers.com/loginx', // path has extra letter
      expectedResultMatch: '',
    }, {
      redirectPath: '/ers.com/logi', // path too short
      expectedResultMatch: '',
    }, {
      redirectPath: '/ers.com/login?source=login',
      expectedResultMatch: ersJumpOffNoQueryString,
    }, {
      redirectPath: '/nhs-login/login?phrPath=%2Ftest%2FmyTests.action&brand=cie', // encoded parts
      expectedResultMatch: pkbCieEncodedPartsInQuery,
    }]).it('will correctly match third party jump off points', (data) => {
      // act
      const result = getThirdPartyJumpOff(thirdPartyConfig, data.redirectPath);

      // assert
      expect(result).toBe(data.expectedResultMatch);
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
