import chooseTestResultYearPage from '@/pages/health-records/gp-medical-record/choose-test-result-year';
import i18n from '@/plugins/i18n';
import * as dependency from '@/lib/utils';
import { TEST_RESULTS_FOR_YEAR_PATH, CHOOSE_TEST_RESULT_YEAR_PATH } from '@/router/paths';
import { createStore, mount } from '../../../../helpers';

let page;
let $store;
let $route;

const mountPage = ({ linkedAccountData, testResultData, pageNumber, isProxying } = {}) => {
  $store = createStore({
    state: {
      device: { isNativeApp: false },
      myRecord: {
        record: {
          testResults: {
            data: testResultData,
            hasErrored: false,
            hasAccess: true,
          },
        },
      },
      linkedAccounts: linkedAccountData,
      session: {
        dateOfBirth: '1948-01-01T00:00:00',
      },
    },
    app: {
      $analytics: {
        trackButtonClick: jest.fn(),
      },
    },
    dispatch: jest.fn(),
    getters: {
      'session/isProxying': isProxying,
    },
  });
  $route = {
    query: {
      page: pageNumber,
    },
  };

  page = mount(chooseTestResultYearPage, {
    $store,
    $route,
    mountOpts: { i18n },
  });
};

describe('gp-medical-record choose test result year', () => {
  beforeEach(() => {
    global.digitalData = {};
    dependency.redirectTo = jest.fn();
  });
  describe('choose test results', () => {
    it('will render a card for the previous 5 years', async () => {
      mountPage({ linkedAccountData: [], testResultData: [], pageNumber: 1 });
      await page.vm.$nextTick();

      const year = page.vm.historicYears;

      const firstYearMenuItem = page.find(`[id="view-older-results-${year[0]}"]`);
      const secondYearMenuItem = page.find(`[id="view-older-results-${year[1]}"]`);
      const thirdYearMenuItem = page.find(`[id="view-older-results-${year[2]}"]`);
      const fourthYearMenuItem = page.find(`[id="view-older-results-${year[3]}"]`);
      const fifthYearMenuItem = page.find(`[id="view-older-results-${year[4]}"]`);

      expect(firstYearMenuItem.exists()).toBe(true);
      expect(secondYearMenuItem.exists()).toBe(true);
      expect(thirdYearMenuItem.exists()).toBe(true);
      expect(fourthYearMenuItem.exists()).toBe(true);
      expect(fifthYearMenuItem.exists()).toBe(true);
    });

    it('will redirect to correct past results page when year menu item is clicked', () => {
      mountPage({ linkedAccountData: [], testResultData: [], pageNumber: 1 });

      const years = page.vm.historicYears;
      const yearMenuItem = page.find(`#view-older-results-${years[0]}`);

      yearMenuItem.trigger('click');
      expect(dependency.redirectTo)
        .toHaveBeenCalledWith(page.vm, `${TEST_RESULTS_FOR_YEAR_PATH}?year=${years[0]}`);
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
      it('href will redirect to the next group of years when the next link clicked', () => {
        mountPage({ linkedAccountData, testResultData: [], pageNumber: 1, isProxying });

        const nextPagination = page.find('[id="nextPagination"]');

        expect(nextPagination.attributes().href)
          .toEqual(`${CHOOSE_TEST_RESULT_YEAR_PATH}?page=2`);
      });

      it('href will redirect to the previous group of years when the previous link clicked', () => {
        mountPage({ linkedAccountData, testResultData: [], pageNumber: 2, isProxying });

        const previousPagination = page.find('[id="previousPagination"]');

        expect(previousPagination.attributes().href)
          .toEqual(`${CHOOSE_TEST_RESULT_YEAR_PATH}?page=1`);
      });

      it('will not show next link when already displaying users birth year', () => {
        mountPage({ linkedAccountData, testResultData: [], pageNumber: 15, isProxying });

        const nextPagination = page.find('[id="nextPagination"]');

        expect(nextPagination.exists()).toBe(false);
      });

      it('will not show next previous when already displaying previous year', () => {
        mountPage({ linkedAccountData, testResultData: [], pageNumber: 1, isProxying });

        const previousPagination = page.find('[id="previousPagination"]');

        expect(previousPagination.exists()).toBe(false);
      });

      it('will redirect to page 1 when a user enters an invalid page number in the url', () => {
        mountPage({ linkedAccountData, testResultData: [], pageNumber: 100, isProxying });

        expect(dependency.redirectTo)
          .toHaveBeenCalledWith(page.vm, `${CHOOSE_TEST_RESULT_YEAR_PATH}?page=1`);
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
      it('href will redirect to the next group of years when the next link clicked', () => {
        mountPage({ linkedAccountData, testResultData: [], pageNumber: 1, isProxying });

        const nextPagination = page.find('[id="nextPagination"]');

        expect(nextPagination.attributes().href)
          .toEqual(`${CHOOSE_TEST_RESULT_YEAR_PATH}?page=2`);
      });

      it('href will redirect to the previous group of years when the previous link clicked', () => {
        mountPage({ linkedAccountData, testResultData: [], pageNumber: 2, isProxying });

        const previousPagination = page.find('[id="previousPagination"]');

        expect(previousPagination.attributes().href)
          .toEqual(`${CHOOSE_TEST_RESULT_YEAR_PATH}?page=1`);
      });

      it('will not show next link when already displaying users birth year', () => {
        mountPage({ linkedAccountData, testResultData: [], pageNumber: 8, isProxying });

        const nextPagination = page.find('[id="nextPagination"]');

        expect(nextPagination.exists()).toBe(false);
      });

      it('will not show next previous when already displaying previous year', () => {
        mountPage({ linkedAccountData, testResultData: [], pageNumber: 1, isProxying });

        const previousPagination = page.find('[id="previousPagination"]');

        expect(previousPagination.exists()).toBe(false);
      });

      it('will redirect to page 1 when a user enters an invalid page number in the url', () => {
        mountPage({ linkedAccountData, testResultData: [], pageNumber: 100, isProxying });

        expect(dependency.redirectTo)
          .toHaveBeenCalledWith(page.vm, `${CHOOSE_TEST_RESULT_YEAR_PATH}?page=1`);
      });

      it('will not show previous and next pagination links when user is too young to require them', () => {
        linkedAccountData = {
          actingAsUser: {
            ageYears: 2,
          },
        };
        mountPage({ linkedAccountData, testResultData: [], pageNumber: 1, isProxying });

        const previousPagination = page.find('[id="previousPagination"]');
        const nextPagination = page.find('[id="nextPagination"]');

        expect(previousPagination.exists()).toBe(false);
        expect(nextPagination.exists()).toBe(false);
      });
    });
  });
});
