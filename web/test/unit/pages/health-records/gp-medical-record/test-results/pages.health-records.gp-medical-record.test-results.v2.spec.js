import testResultsV2Page from '@/pages/health-records/gp-medical-record/test-results-v2';
import i18n from '@/plugins/i18n';
import { createStore, mount } from '../../../../helpers';

let page;
let $store;

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

const mountPage = ({ testResults = undefined } = {}) => {
  $store = createStore({
    state: {
      device: { isNativeApp: false },
      myRecord: {
        record: {
          testResults,
          supplier: 'TPP',
        },
      },
    },
    $env: {
      CLINICAL_ABBREVIATIONS_URL: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/abbreviations/',
    },
    dispatch: jest.fn(),
  });

  page = mount(testResultsV2Page, {
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
    expect($store.dispatch).not.toHaveBeenCalledWith('myRecord/load');
  });

  it('will direct user to the choose test result year page', async () => {
    mountPage();
    await page.vm.$nextTick();

    const olderTestResultsLink = page.find('[id="view-older-results"]');
    expect(olderTestResultsLink.attributes().href)
      .toEqual('health-records/gp-medical-record/choose-test-result-year');
  });

  it('will direct user to the test result detail page', async () => {
    mountPage({
      testResults: {
        data: [
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
        hasErrored: false,
        hasAccess: true,
      },
    });
    await page.vm.$nextTick();

    const testResultsLink = page.find('[id="view-test-results-0"]');
    expect(testResultsLink.attributes().href)
      .toEqual('health-records/gp-medical-record/testresultdetail/1');
  });
});
