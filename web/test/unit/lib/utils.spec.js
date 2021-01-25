import _locale from '@/locale/en';
import each from 'jest-each';
import get from 'lodash/fp/get';
import isString from 'lodash/fp/isString';
import mockdate from 'mockdate';
import moment from 'moment';
import {
  formatInboxMessageTime,
  formatIndividualMessageTime,
  getPathAndQuery,
  getPathWithPatientIdPrefix,
  getThirdPartyJumpOff,
  getThirdPartyLocaleText,
  gpSessionErrorHasRetried,
  isFalsy,
  isTruthy,
  isSameHostNameAndProtocol,
  mimeType,
  normaliseWhiteSpace,
  readableBytes,
  redirectTo,
  redirectByName,
  removePatientIdPrefixFromPath,
  resetPageFocus,
  stripHtml,
} from '@/lib/utils';
import { INDEX_PATH, INDEX_PATH_PARAM } from '@/router/paths';
import { EventBus, FOCUS_NHSAPP_TITLE } from '@/services/event-bus';

jest.mock('@/services/native-app');
jest.mock('@/services/event-bus', () => ({
  ...jest.requireActual('@/services/event-bus'),
  EventBus: { $emit: jest.fn() },
}));

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

const create$T = () => jest
  .fn()
  .mockImplementation((key, formatParams) => {
    let value = get(key)(_locale);
    if (isString(value) || value === undefined) {
      if (formatParams) {
        Object.keys(formatParams).forEach((formatParam) => {
          value = value.replace(`{${formatParam}}`, formatParams[formatParam]);
        });
      }
    }

    return value;
  });

describe('util library', () => {
  let loggedIn = false;
  const PATIENT_ID = 1234;

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
        getters: {
          'session/isLoggedIn': () => loggedIn,
          'linkedAccounts/getPatientId': PATIENT_ID,
          'linkedAccounts/isPatientIdNotEmpty': true,
        },
      },
      $router: {
        currentRoute: {
          path: 'foo',
          params: {},
          name: 'bar',
        },
        go: jest.fn(),
        push: jest.fn(),
      },
    };
  });

  describe('getPathWithPatientIdPrefix', () => {
    it('will not add patient id prefix when url begins with patient/', () => {
      const param = { trimmedPath: 'patient/pathName', store: self.$store };
      // act
      const result = getPathWithPatientIdPrefix(param);
      expect(result).toBe('/patient/pathName');
    });

    it('will not add patient id prefix when url begins with patient/<id>/', () => {
      const patientId = '330b2795-e20f-427e-9699-7943dd31d4db';
      const param = { trimmedPath: `patient/${patientId}/pathName`, store: self.$store };
      // act
      const result = getPathWithPatientIdPrefix(param);
      expect(result).toBe(`/patient/${patientId}/pathName`);
    });

    it('will add patient id prefix when url does not begin with patient', () => {
      const patientId = '330b2795-e20f-427e-9699-7943dd31d4db';
      self.$store.getters['linkedAccounts/isPatientIdNotEmpty'] = true;
      self.$store.getters['linkedAccounts/getPatientId'] = patientId;
      const param = { trimmedPath: 'pathName', store: self.$store };
      // act
      const result = getPathWithPatientIdPrefix(param);
      expect(result).toBe(`/patient/${patientId}/pathName`);
    });
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
    describe('logged out', () => {
      beforeEach(() => {
        loggedIn = false;
      });

      it('will call push with query if passed', () => {
        const query = { source: 'ios' };
        redirectTo(self, 'a-path', query);
        expect(self.$router.push).toBeCalledWith({ path: 'a-path', query });
      });

      each([
        null,
        undefined,
      ]).it('will call push with path only if query value is `%s`', (query) => {
        redirectTo(self, 'a-path', query);
        expect(self.$router.push).toBeCalledWith({ path: 'a-path' });
      });

      describe('same page', () => {
        let path;
        let ts;

        beforeEach(() => {
          const nowDate = moment();
          mockdate.set(nowDate);
          ts = nowDate.unix();

          ({ path } = self.$router.currentRoute);
        });

        afterEach(() => {
          mockdate.reset();
        });

        it('will call push with timespan if query is not passed', () => {
          redirectTo(self, path);
          expect(self.$router.push).toBeCalledWith({ path, query: { ts } });
        });

        it('will call push with passed query and timespan if it is the same as current', () => {
          const query = { source: 'ios', ts: 12345678 };
          self.$router.currentRoute.query = query;
          redirectTo(self, path, query);
          expect(self.$router.push).toBeCalledWith({
            path,
            query: {
              ...query,
              ts,
            },
          });
        });

        it('will call push with passed query if not the same as current', () => {
          const query = { source: 'ios' };
          redirectTo(self, path, query);
          expect(self.$router.push).toBeCalledWith({ path, query });
        });
      });
    });

    describe('logged in', () => {
      const completePath = INDEX_PATH.replace(INDEX_PATH_PARAM, PATIENT_ID);

      beforeEach(() => {
        loggedIn = true;
      });
      it('will call push with query if passed', () => {
        const query = { source: 'ios' };
        redirectTo(self, 'a-path', query);
        expect(self.$router.push).toBeCalledWith({ path: `${completePath}a-path`, query });
      });

      each([
        null,
        undefined,
      ]).it('will call push with path only if query value is `%s`', (query) => {
        redirectTo(self, 'a-path', query);
        expect(self.$router.push).toBeCalledWith({ path: `${completePath}a-path` });
      });

      describe('same page', () => {
        let path;
        let ts;

        beforeEach(() => {
          const nowDate = moment();
          mockdate.set(nowDate);
          ts = nowDate.unix();

          ({ path } = self.$router.currentRoute);
        });

        afterEach(() => {
          mockdate.reset();
        });

        it('will call push with timespan if query is not passed', () => {
          redirectTo(self, path);
          expect(self.$router.push).toBeCalledWith({ path: completePath + path, query: { ts } });
        });

        it('will call push with passed query and timespan if it is the same as current', () => {
          const query = { source: 'ios', ts: 12345678 };
          self.$router.currentRoute.query = query;
          redirectTo(self, path, query);
          expect(self.$router.push).toBeCalledWith({
            path: completePath + path,
            query: {
              ...query,
              ts,
            },
          });
        });

        it('will call push with passed query if not the same as current', () => {
          const query = { source: 'ios' };
          redirectTo(self, path, query);
          expect(self.$router.push).toBeCalledWith({ path: completePath + path, query });
        });
      });
    });
  });

  describe('redirectByName', () => {
    describe('logged out', () => {
      const params = {};

      beforeEach(() => {
        loggedIn = false;
      });

      it('will call push with query if passed', () => {
        const query = { source: 'ios' };
        redirectByName(self, 'a-name', query);
        expect(self.$router.push).toBeCalledWith({ name: 'a-name', query, params });
      });

      each([
        null,
        undefined,
      ]).it('will call push with name only if query value is `%s`', (query) => {
        redirectByName(self, 'a-name', query);
        expect(self.$router.push).toBeCalledWith({ name: 'a-name', params });
      });

      describe('same page', () => {
        let name;
        let ts;

        beforeEach(() => {
          const nowDate = moment();
          mockdate.set(nowDate);
          ts = nowDate.unix();

          ({ name } = self.$router.currentRoute);
        });

        afterEach(() => {
          mockdate.reset();
        });

        it('will call push with timespan if query is not passed', () => {
          redirectByName(self, name);
          expect(self.$router.push).toBeCalledWith({ name, query: { ts }, params });
        });

        it('will call push with passed query and timespan if it is the same as current', () => {
          const query = { source: 'ios', ts: 12345678 };
          self.$router.currentRoute.query = query;
          redirectByName(self, name, query);
          expect(self.$router.push).toBeCalledWith({
            name,
            query: {
              ...query,
              ts,
            },
            params,
          });
        });

        it('will call push with passed query if not the same as current', () => {
          const query = { source: 'ios' };
          redirectByName(self, name, query);
          expect(self.$router.push).toBeCalledWith({ name, query, params });
        });
      });
    });

    describe('logged in', () => {
      beforeEach(() => {
        loggedIn = true;
      });
      it('will call push with query if passed', () => {
        const query = { source: 'ios' };
        const params = { patientId: PATIENT_ID };
        redirectByName(self, 'a-name', query);
        expect(self.$router.push).toBeCalledWith({ name: 'a-name', query, params });
      });

      each([
        null,
        undefined,
      ]).it('will call push with path only if query value is `%s`', (query) => {
        redirectByName(self, 'a-name', query);
        const params = { patientId: PATIENT_ID };
        expect(self.$router.push).toBeCalledWith({ name: 'a-name', params });
      });

      describe('multiple parameters', () => {
        beforeEach(() => {
          loggedIn = true;
          self.$router.currentRoute.params = { pearl: 'jam' };
        });

        it('will call push with param if passed', () => {
          const query = { source: 'ios' };
          redirectByName(self, 'a-name', query);
          expect(self.$router.push).toBeCalledWith({
            name: 'a-name',
            query,
            params: {
              ...self.$router.currentRoute.params,
              patientId: PATIENT_ID,
            },
          });
        });
      });

      describe('no patient Id', () => {
        beforeEach(() => {
          loggedIn = true;
          self.$store.getters['linkedAccounts/isPatientIdNotEmpty'] = false;
        });
        it('will call push with no patientId param', () => {
          const query = { source: 'ios' };
          redirectByName(self, 'a-name', query);
          expect(self.$router.push).toBeCalledWith({ name: 'a-name', query, params: {} });
        });
      });

      describe('same page', () => {
        let ts;
        let name;

        beforeEach(() => {
          const nowDate = moment();
          mockdate.set(nowDate);
          ts = nowDate.unix();

          ({ name } = self.$router.currentRoute);
        });

        afterEach(() => {
          mockdate.reset();
        });

        it('will call push with timespan if query is not passed', () => {
          redirectByName(self, name);
          expect(self.$router.push).toBeCalledWith({
            name,
            query: { ts },
            params: { patientId: PATIENT_ID },
          });
        });

        it('will call push with passed query and timespan if it is the same as current', () => {
          const query = { source: 'ios', ts: 12345678 };
          self.$router.currentRoute.query = query;
          redirectByName(self, name, query);
          expect(self.$router.push).toBeCalledWith({
            name,
            query: {
              ...query,
              ts,
            },
            params: { patientId: PATIENT_ID },
          });
        });

        it('will call push with passed query if not the same as current', () => {
          const query = { source: 'ios' };
          redirectByName(self, name, query);
          expect(self.$router.push).toBeCalledWith({
            name,
            query,
            params: { patientId: PATIENT_ID },
          });
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

  describe('stripHtml', () => {
    let result;

    beforeEach(() => {
      result = stripHtml('Sample <b>content</b> with html');
    });

    it('will return sanitized content', () => {
      expect(result).toBe('Sample content with html');
    });
  });

  describe('message date time formatters', () => {
    const $t = create$T();

    beforeEach(() => {
      mockdate.set('2020-01-28T13:11:00.000Z');
    });

    describe('formatInboxMessageTime', () => {
      it.each([
        ['2020-01-28T11:00:00.000Z', '11am'],
        ['2020-01-28T11:23:01.000Z', '11:23am'],
        ['2020-01-28T12:00:01.000Z', 'Midday'],
        ['2020-01-28T00:00:01.000Z', 'Midnight'],
        ['2020-01-27T11:23:01.000Z', 'Yesterday'],
        ['2020-01-26T11:23:01.000Z', 'Sunday'],
        ['2020-01-22T11:23:01.000Z', 'Wednesday'],
        ['2020-01-21T11:23:01.000Z', '21 January 2020'],
      ])('will format %s date to %s', (messageDate, expectedFormattedDate) => {
        expect(formatInboxMessageTime(messageDate, $t)).toEqual(expectedFormattedDate);
      });
    });

    describe('formatIndividualMessageTime', () => {
      it.each([
        ['2020-01-28T11:00:00.000Z', 'Sent today at 11am'],
        ['2020-01-28T11:23:01.000Z', 'Sent today at 11:23am'],
        ['2020-01-28T12:00:01.000Z', 'Sent today at midday'],
        ['2020-01-28T00:00:01.000Z', 'Sent today at midnight'],
        ['2020-01-27T11:23:01.000Z', 'Sent yesterday at 11:23am'],
        ['2020-01-26T14:12:01.000Z', 'Sent 26 January 2020 at 2:12pm'],
        ['2020-01-22T12:00:01.000Z', 'Sent 22 January 2020 at midday'],
        ['2020-01-21T00:00:01.000Z', 'Sent 21 January 2020 at midnight'],
      ])('will format %s date to %s', (messageDate, expectedFormattedDate) => {
        expect(formatIndividualMessageTime(messageDate, $t)).toEqual(expectedFormattedDate);
      });
    });

    describe('isSameHostNameAndProtocol', () => {
      const { location } = window;

      beforeEach(() => {
        delete window.location;
        window.location = {
          protocol: 'https:',
          hostname: 'www.test.com',
        };
      });

      afterEach(() => {
        window.location = location;
      });

      describe('valid matching URLs', () => {
        each([
          ['https://www.test.com'],
          ['https://www.test.com?query=string'],
          ['https://www.test.com/path'],
          ['https://www.test.com/#anchor'],
        ]).it('will return true for `%s`)', (url) => {
          expect(isSameHostNameAndProtocol(url)).toEqual(true);
        });
      });

      describe('valid but not matching URLs', () => {
        each([
          ['http://www.test.com'],
          ['http://www.test.com?query=string'],
          ['http://www.test.com/path'],
          ['http://www.anothertest.com'],
          ['http://www.anothertest.com/path'],
          ['https://www.anothertest.com'],
          ['https://www.anothertest.com/path'],
          ['https://www.anothertest.com/path#anchor'],
        ]).it('will return false for `%s`', (url) => {
          expect(isSameHostNameAndProtocol(url)).toEqual(false);
        });
      });

      describe('invalid URLs', () => {
        each([
          ['/path?query=string'],
          ['some random string'],
          ['email@address.com'],
        ]).it('will return false for `%s`', (url) => {
          expect(isSameHostNameAndProtocol(url)).toEqual(false);
        });
      });
    });

    describe('getPathAndQuery', () => {
      each([
        { url: 'http://www.test.com/path?query=string', expectedResult: '/path?query=string' },
        { url: 'http://www.test.com/path', expectedResult: '/path' },
        { url: 'http://www.test.com?query=string', expectedResult: '/?query=string' },
        { url: 'http://www.test.com', expectedResult: '/' },
        { url: '/path?query=string', expectedResult: '' },
        { url: 'some random string', expectedResult: '' },
      ]).it('will return the path and querystring for a given URL',
        ({ url, expectedResult }) => {
          expect(getPathAndQuery(url)).toEqual(expectedResult);
        });
    });

    describe('getThirdPartyLocaleText', () => {
      const $te = create$T();
      each([{
        textType: 'headerText',
        jumpOffId: 'messages',
        feature: 'jumpOffContent',
        expectedText: 'Messages and consultations with a doctor or health professional',
      }, {
        textType: 'featureName',
        jumpOffId: 'messages',
        feature: 'thirdPartyWarning',
        expectedText: 'Messages and online consultations',
      }]).it('will bring back the correct third party locale text',
        ({ textType, jumpOffId, feature, expectedText }) => {
          const thirdPartyLocales = $te('thirdPartyProviders.pkb') ? $t('thirdPartyProviders.pkb') : '';
          const retrievedText =
            getThirdPartyLocaleText(thirdPartyLocales, jumpOffId, feature, textType);

          expect(retrievedText).toEqual(expectedText);
        });
    });

    describe('resetPageFocus', () => {
      it('will emit FOCUS_NHSAPP_TITLE on event bus', () => {
        resetPageFocus({ state: { device: {} } });

        expect(EventBus.$emit).toHaveBeenCalledWith(FOCUS_NHSAPP_TITLE);
      });

      it('will reset window scroll position', () => {
        window.scrollTo = jest.fn();
        resetPageFocus({ state: { device: {} } });

        expect(window.scrollTo).toHaveBeenCalledWith(0, 0);
      });
    });

    describe('gpSessionHasRetried', () => {
      const createStore = ({ isNativeApp = false, hasRetried = false }) =>
        ({
          state: {
            device: {
              isNativeApp,
            },
            session: {
              hasRetried,
            },
          },
        });

      it('will return true if session/hasRetried is true', () => {
        const $store = createStore({ hasRetried: true });

        expect(gpSessionErrorHasRetried($store)).toBe(true);
      });

      it('will return false if session/hasRetried is false', () => {
        const $store = createStore({ hasRetried: false });

        expect(gpSessionErrorHasRetried($store)).toBe(false);
      });

      describe('on native app', () => {
        it('will return true if sessionStorage hasRetried is true', () => {
          const $store = createStore({ isNativeApp: true });

          Storage.prototype.getItem = jest.fn('hasRetried').mockImplementation(() => true);
          expect(gpSessionErrorHasRetried($store)).toBe(true);
        });

        it('will return false if sessionStorage hasRetried is false', () => {
          const $store = createStore({ isNativeApp: true });

          Storage.prototype.getItem = jest.fn('hasRetried').mockImplementation(() => false);
          expect(gpSessionErrorHasRetried($store)).toBe(false);
        });
      });
    });

    afterEach(() => {
      mockdate.reset();
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

  describe('removePatientIdPrefixFromPath', () => {
    each([
      [undefined, undefined],
      ['', ''],
      [null, null],
      [1, 1],
      [{}, {}],
      ['/', '/'],
      ['/patient', '/'],
      ['/patient/a', '/a'],
      ['/patient/c5af7c34-b4ba-4f1a-bff8-476324e5f835', '/'],
      ['/patient/c5af7c34-b4ba-4f1a-bff8-476324e5f835/a', '/a'],
      ['/patient/c5af7c34-b4ba-4f1a-bff8-476324e5f835/a/b', '/a/b'],
    ]).it('will remove the /patient/patientId prefix from the path', (path, expected) => {
      expect(removePatientIdPrefixFromPath(path)).toEqual(expected);
    });
  });
});
