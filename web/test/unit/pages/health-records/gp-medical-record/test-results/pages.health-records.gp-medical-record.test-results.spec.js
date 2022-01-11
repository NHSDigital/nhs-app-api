import testResultsPage from '@/pages/health-records/gp-medical-record/test-results';
import i18n from '@/plugins/i18n';
import { createStore, shallowMount } from '../../../../helpers';

let page;
let $store;

const mountPage = ({ testResults = undefined, isNative = false } = {}) => {
  $store = createStore({
    state: {
      device: { isNativeApp: isNative },
      myRecord: {
        record: {
          testResults,
        },
      },
    },
    dispatch: jest.fn(),
  });

  page = shallowMount(testResultsPage, {
    $store,
    mountOpts: { i18n },
  });
};

describe('gp-medical-record test results', () => {
  it('will load myRecord if test results have not been loaded', async () => {
    mountPage();
    expect($store.dispatch).toHaveBeenCalledWith('myRecord/load');
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

  it('will have a back link on the desktop website', async () => {
    mountPage();
    await page.vm.$nextTick();

    const backlink = page.find('[id="desktopBackLink"]');
    expect(backlink.exists()).toBe(true);
  });

  it('will not have a back link on the native app', async () => {
    mountPage({ isNative: true });
    await page.vm.$nextTick();

    const backlink = page.find('[id="desktopBackLink"]');
    expect(backlink.exists()).toBe(false);
  });
});
