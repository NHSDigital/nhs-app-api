import each from 'jest-each';
import mockdate from 'mockdate';
import moment from 'moment';
import {
  isFalsy,
  isTruthy,
  redirectTo,
  redirectByName,
  readableBytes,
  stripHtml,
  formatInboxMessageTime,
  formatIndividualMessageTime,
  getPathAndQuery,
  getThirdPartyLocaleText,
  mimeType,
  resetPageFocus,
} from '@/lib/utils';
import { INDEX_PATH, INDEX_PATH_PARAM } from '@/router/paths';
import NativeCallbacks from '@/services/native-app';
import { EventBus, FOCUS_NHSAPP_ROOT } from '@/services/event-bus';
import { create$T } from '../helpers';

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

describe('util library', () => {
  let loggedIn = false;
  const PATIENT_ID = 1234;

  beforeEach(() => {
    NativeCallbacks.resetPageFocus.mockClear();

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
    const $t = create$T(false);

    beforeEach(() => {
      mockdate.set('2020-01-28T13:11:00.000Z');
    });

    describe('formatInboxMessageTime', () => {
      each([
        { messageDate: '2020-01-28T11:23:01.000Z', expectedFormattedDate: '11:23am' },
        { messageDate: '2020-01-28T12:00:01.000Z', expectedFormattedDate: 'Midday' },
        { messageDate: '2020-01-28T00:00:01.000Z', expectedFormattedDate: 'Midnight' },
        { messageDate: '2020-01-27T11:23:01.000Z', expectedFormattedDate: 'Yesterday' },
        { messageDate: '2020-01-26T11:23:01.000Z', expectedFormattedDate: 'Sunday' },
        { messageDate: '2020-01-22T11:23:01.000Z', expectedFormattedDate: 'Wednesday' },
        { messageDate: '2020-01-21T11:23:01.000Z', expectedFormattedDate: '21 January 2020' },
      ]).it('will format the dates appropriately for displaying in the inbox',
        ({ messageDate, expectedFormattedDate }) => {
          expect(formatInboxMessageTime(messageDate, $t)).toEqual(expectedFormattedDate);
        });
    });

    describe('formatIndividualMessageTime', () => {
      each([{
        messageDate: '2020-01-28T11:23:01.000Z',
        expectedFormattedDate: 'Sent today at 11:23am',
      }, {
        messageDate: '2020-01-28T12:00:01.000Z',
        expectedFormattedDate: 'Sent today at midday',
      }, {
        messageDate: '2020-01-28T00:00:01.000Z',
        expectedFormattedDate: 'Sent today at midnight',
      }, {
        messageDate: '2020-01-27T11:23:01.000Z',
        expectedFormattedDate: 'Sent yesterday at 11:23am',
      }, {
        messageDate: '2020-01-26T14:12:01.000Z',
        expectedFormattedDate: 'Sent 26 January 2020 at 2:12pm',
      }, {
        messageDate: '2020-01-22T12:00:01.000Z',
        expectedFormattedDate: 'Sent 22 January 2020 at midday',
      }, {
        messageDate: '2020-01-21T00:00:01.000Z',
        expectedFormattedDate: 'Sent 21 January 2020 at midnight',
      }]).it('will format the dates appropriately for displaying beneath an individual message',
        ({ messageDate, expectedFormattedDate }) => {
          expect(formatIndividualMessageTime(messageDate, $t)).toEqual(expectedFormattedDate);
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
      const $te = create$T(false);
      each([{
        textType: 'headerText',
        redirectPath: '/nhs-login/login?phrPath=/auth/getInbox.action?tab=messages',
        feature: 'jumpOffContent',
        expectedText: 'Messages and consultations with a doctor or health professional',
      }, {
        textType: 'featureName',
        redirectPath: '/nhs-login/login?phrPath=/auth/getInbox.action?tab=messages',
        feature: 'thirdPartyWarning',
        expectedText: 'Messages and online consultations',
      }]).it('will bring back the correct third party locale text',
        ({ textType, redirectPath, feature, expectedText }) => {
          const thirdPartyLocales = $te('thirdPartyProviders.pkb') ? $t('thirdPartyProviders.pkb') : '';
          const retrievedText =
            getThirdPartyLocaleText(thirdPartyLocales, redirectPath, feature, textType);

          expect(retrievedText).toEqual(expectedText);
        });
    });

    describe('resetPageFocus', () => {
      it('will call NativeCallbacks.resetPageFocus when isNativeApp is true', () => {
        resetPageFocus({ state: { device: { isNativeApp: true } } });

        expect(NativeCallbacks.resetPageFocus).toHaveBeenCalled();
      });
      it('will not call NativeCallbacks.resetPageFocus when isNativeApp is false', () => {
        resetPageFocus({ state: { device: { isNativeApp: false } } });

        expect(NativeCallbacks.resetPageFocus).not.toHaveBeenCalled();
      });

      it('will emit FOCUS_NHSAPP_ROOT on event bus', () => {
        resetPageFocus({ state: { device: {} } });

        expect(EventBus.$emit).toHaveBeenCalledWith(FOCUS_NHSAPP_ROOT);
      });

      it('will reset window scroll position', () => {
        window.scrollTo = jest.fn();
        resetPageFocus({ state: { device: {} } });

        expect(window.scrollTo).toHaveBeenCalledWith(0, 0);
      });
    });
  });
});
