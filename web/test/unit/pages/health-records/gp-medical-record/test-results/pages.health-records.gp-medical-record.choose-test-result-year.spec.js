import chooseTestResultYearPage from '@/pages/health-records/gp-medical-record/choose-test-result-year';
import MenuItem from '@/components/MenuItem';
import i18n from '@/plugins/i18n';
import * as dependency from '@/lib/utils';
import { TEST_RESULTS_FOR_YEAR_PATH } from '@/router/paths';
import { createStore, mount } from '../../../../helpers';

let page;
let $store;

const mountPage = ({ testResultData } = {}) => {
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
    },
    app: {
      $analytics: {
        trackButtonClick: jest.fn(),
      },
    },
    dispatch: jest.fn(),
  });

  page = mount(chooseTestResultYearPage, {
    $store,
    mountOpts: { i18n },
  });
};

describe('gp-medical-record older test results', () => {
  beforeEach(() => {
    global.digitalData = {};
    dependency.redirectTo = jest.fn();
  });
  it('will render a card for the previous 5 years', async () => {
    mountPage({ testResultData: [] });
    await page.vm.$nextTick();

    const year = page.vm.historicYears;
    const yearsMenuItems = page.findAll(MenuItem);

    const firstYearMenuItem = yearsMenuItems.wrappers[0].find(`[id="view-older-results-${year[0]}"]`);
    const secondYearMenuItem = yearsMenuItems.wrappers[1].find(`[id="view-older-results-${year[1]}"]`);
    const thirdYearMenuItem = yearsMenuItems.wrappers[2].find(`[id="view-older-results-${year[2]}"]`);
    const fourthYearMenuItem = yearsMenuItems.wrappers[3].find(`[id="view-older-results-${year[3]}"]`);
    const fifthYearMenuItem = yearsMenuItems.wrappers[4].find(`[id="view-older-results-${year[4]}"]`);

    expect(firstYearMenuItem.exists()).toBe(true);
    expect(secondYearMenuItem.exists()).toBe(true);
    expect(thirdYearMenuItem.exists()).toBe(true);
    expect(fourthYearMenuItem.exists()).toBe(true);
    expect(fifthYearMenuItem.exists()).toBe(true);
  });

  it('will redirect to correct past results page when year menu item is clicked', () => {
    mountPage({ testResultData: [] });

    const years = page.vm.historicYears;
    const yearMenuItem = page.find(`#view-older-results-${years[0]}`);

    yearMenuItem.trigger('click');
    expect(dependency.redirectTo)
      .toHaveBeenCalledWith(page.vm, `${TEST_RESULTS_FOR_YEAR_PATH}?year=${years[0]}`);
  });
});
