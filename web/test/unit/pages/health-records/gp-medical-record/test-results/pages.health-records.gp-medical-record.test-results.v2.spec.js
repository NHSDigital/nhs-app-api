import testResultsV2Page from '@/pages/health-records/gp-medical-record/test-results-v2';
import i18n from '@/plugins/i18n';
import { createStore, shallowMount } from '../../../../helpers';

let page;
let $store;

const mountPage = ({ testResults = undefined } = {}) => {
  $store = createStore({
    state: {
      device: { isNativeApp: false },
      myRecord: {
        record: {
          testResults,
        },
      },
    },
    dispatch: jest.fn(),
  });

  page = shallowMount(testResultsV2Page, {
    $store,
    mountOpts: { i18n },
  });
};

describe('gp-medical-record test results', () => {
  it('will load myRecord if test results have not been loaded', async () => {
    mountPage();
    expect($store.dispatch).toHaveBeenCalledWith('myRecord/load');
  });

  it('will will display the year in a H2 tag', async () => {
    mountPage();
    await page.vm.$nextTick();

    const year = page.find('h2');
    expect(year.text()).toEqual(new Date().getFullYear().toString());
  });

  it('will not load myRecord if test results have already been loaded', async () => {
    mountPage({
      testResults: {
        data: [],
        hasErrored: false,
        hasAccess: true,
      },
    });
    expect($store.dispatch).not.toHaveBeenCalled();
  });

  it('will direct user to the choose test result year page', async () => {
    mountPage();
    await page.vm.$nextTick();

    const olderTestResultsLink = page.find('[id="view-older-results"]');
    expect(olderTestResultsLink.attributes().href)
      .toEqual('health-records/gp-medical-record/choose-test-result-year');
  });
});
