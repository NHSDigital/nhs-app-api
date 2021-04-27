import _locale from '@/locale/en';
import each from 'jest-each';
import get from 'lodash/fp/get';
import isString from 'lodash/fp/isString';
import mockdate from 'mockdate';
import {
  formatInboxMessageTime,
  formatIndividualMessageTime,
  getPathAndQuery,
  getThirdPartyLocaleText,
  gpSessionErrorHasRetried,
  isSameHostNameAndProtocol,
  resetPageFocus,
} from '@/lib/utils';
import { EventBus, FOCUS_NHSAPP_TITLE } from '@/services/event-bus';

jest.mock('@/services/native-app');
jest.mock('@/services/event-bus', () => ({
  ...jest.requireActual('@/services/event-bus'),
  EventBus: { $emit: jest.fn() },
}));

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

describe('util library datetime', () => {
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
        expectedText: 'Consultations, events and messages',
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
});
