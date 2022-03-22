import testResultsForYearPage from '@/pages/health-records/gp-medical-record/test-results-for-year';
import i18n from '@/plugins/i18n';
import * as dependency from '@/lib/utils';
import { TEST_RESULTS_FOR_YEAR_PATH } from '@/router/paths';
import { createStore, mount } from '../../../../helpers';

let page;
let $store;
let $route;

const createTestResults = ({
  associatedTexts,
  date,
  description,
  id,
  testResultChildLineItems,
} = {}) =>
  ({
    associatedTexts,
    date,
    description,
    id,
    testResultChildLineItems,
  });

const mountPage = async ({ linkedAccountData, historicTestResultData, year, isProxying } = {}) => {
  $store = createStore({
    state: {
      device: { isNativeApp: false },
      myRecord: {
        record: {
          historicTestResults: {
            data: {},
            hasErrored: false,
            hasAccess: true,
            _2021: {
              data: historicTestResultData,
              hasErrored: false,
              hasAccess: true,
            },
          },
        },
        hasLoaded: true,
      },
      linkedAccounts: linkedAccountData,
      session: {
        dateOfBirth: '1948-01-01T00:00:00',
      },
      hasLoaded: true,
    },
    dispatch: jest.fn(),
    $env: {
      CLINICAL_ABBREVIATIONS_URL: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/abbreviations/',
    },
    getters: {
      'session/isProxying': isProxying,
    },
  });
  $route = {
    query: {
      year,
    },
  };

  page = mount(testResultsForYearPage, {
    $store,
    $route,
    mountOpts: { i18n },
  });
};

describe('gp-medical-record test results for year', () => {
  describe('specific year test results', () => {
    it('will call dispatch for loadHistoricTestResult when data for specific year is empty', async () => {
      const year = 2020;
      await mountPage({ linkedAccountData: [], historicTestResultData: [], year });

      expect($store.dispatch).toHaveBeenCalledWith('myRecord/loadHistoricTestResult', year);
    });

    it('will not call dispatch for loadHistoricTestResult when data for specific year is empty', async () => {
      const year = '2021';
      await mountPage({
        linkedAccountData: [],
        historicTestResultData: [
          createTestResults({
            associatedTexts: ['associated texts'],
            date: { value: '2022-01-10T12:03:44+00:00' },
            description: 'first description text',
            id: '1',
            testResultChildLineItems: [
              {
                associatedTexts: ['child associated texts'],
                description: 'child description text',
              },
            ],
          }),
        ],
        year,
      });

      expect($store.dispatch).not.toHaveBeenCalledWith('myRecord/loadHistoricTestResult', year);
    });
  });

  describe('pagination', () => {
    describe('is not proxying', () => {
      let linkedAccountData;
      let isProxying;
      beforeEach(() => {
        global.digitalData = {};
        dependency.redirectTo = jest.fn();
        linkedAccountData = [];
        isProxying = false;
      });
      it('href will redirect to the next year when the next link is clicked', () => {
        mountPage({ linkedAccountData, testResultData: [], year: '2020', isProxying });

        const nextPagination = page.find('[id="nextPagination"]');

        expect(nextPagination.attributes().href)
          .toEqual(`${TEST_RESULTS_FOR_YEAR_PATH}?year=2019`);
      });

      it('href will redirect to the previous year when the previous link clicked', () => {
        mountPage({ linkedAccountData, testResultData: [], year: '2020', isProxying });

        const previousPagination = page.find('[id="previousPagination"]');

        expect(previousPagination.attributes().href)
          .toEqual(`${TEST_RESULTS_FOR_YEAR_PATH}?year=2021`);
      });

      it('will not show next link when already displaying users birth year', () => {
        mountPage({ linkedAccountData, testResultData: [], year: '1948', isProxying });

        const nextPagination = page.find('[id="nextPagination"]');

        expect(nextPagination.exists()).toBe(false);
      });

      it('will not show previous link when already displaying previous year', () => {
        const currentYear = new Date().getFullYear();
        mountPage({
          linkedAccountData,
          testResultData: [],
          year: currentYear.toString(),
          isProxying });

        const previousPagination = page.find('[id="previousPagination"]');

        expect(previousPagination.exists()).toBe(false);
      });

      it('will redirect to current year when a user enters an invalid page number in the url', () => {
        const currentYear = new Date().getFullYear();
        global.digitalData = {};
        dependency.redirectTo = jest.fn();
        mountPage({ linkedAccountData, testResultData: [], year: '3000', isProxying });

        expect(dependency.redirectTo)
          .toHaveBeenCalledWith(page.vm, `${TEST_RESULTS_FOR_YEAR_PATH}?year=${currentYear.toString()}`);
      });
    });

    describe('is proxying', () => {
      let linkedAccountData;
      let isProxying;
      beforeEach(() => {
        global.digitalData = {};
        dependency.redirectTo = jest.fn();
        linkedAccountData = {
          actingAsUser: {
            ageYears: 36,
          },
        };
        isProxying = true;
      });
      it('href will redirect to the next year when the next link is clicked', () => {
        mountPage({ linkedAccountData, testResultData: [], year: '2020', isProxying });

        const nextPagination = page.find('[id="nextPagination"]');

        expect(nextPagination.attributes().href)
          .toEqual(`${TEST_RESULTS_FOR_YEAR_PATH}?year=2019`);
      });

      it('href will redirect to the previous year when the previous link clicked', () => {
        mountPage({ linkedAccountData, testResultData: [], year: '2020', isProxying });

        const previousPagination = page.find('[id="previousPagination"]');

        expect(previousPagination.attributes().href)
          .toEqual(`${TEST_RESULTS_FOR_YEAR_PATH}?year=2021`);
      });

      it('will not show next link when already displaying users birth year', () => {
        mountPage({ linkedAccountData, testResultData: [], year: '1986', isProxying });

        const nextPagination = page.find('[id="nextPagination"]');

        expect(nextPagination.exists()).toBe(false);
      });

      it('will not show previous link when already displaying previous year', () => {
        const currentYear = new Date().getFullYear();
        mountPage({
          linkedAccountData,
          testResultData: [],
          year: currentYear.toString(),
          isProxying });

        const previousPagination = page.find('[id="previousPagination"]');

        expect(previousPagination.exists()).toBe(false);
      });

      it('will redirect to page 1 when a user enters an invalid page number in the url', () => {
        const currentYear = new Date().getFullYear();
        mountPage({ linkedAccountData, testResultData: [], year: '3000', isProxying });

        expect(dependency.redirectTo)
          .toHaveBeenCalledWith(page.vm, `${TEST_RESULTS_FOR_YEAR_PATH}?year=${currentYear}`);
      });
    });
  });
});
