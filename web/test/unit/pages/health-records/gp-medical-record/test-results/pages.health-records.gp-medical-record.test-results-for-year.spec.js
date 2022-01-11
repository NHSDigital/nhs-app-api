import testResultsForYearPage from '@/pages/health-records/gp-medical-record/test-results-for-year';
import i18n from '@/plugins/i18n';
import { createStore, shallowMount } from '../../../../helpers';

let $store;
let $router;

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

const mountPage = ({ historicTestResultData, year } = {}) => {
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
      },
    },
    dispatch: jest.fn(),
  });
  $router = {
    currentRoute: {
      query: {
        year,
      },
    },
  };

  shallowMount(testResultsForYearPage, {
    $store,
    $router,
    mountOpts: { i18n },
  });
};

describe('gp-medical-record past test results', () => {
  it('will call dispatch for loadHistoricTestResult when data for specific year is empty', async () => {
    const year = 2020;
    mountPage({ historicTestResultData: [], year });

    expect($store.dispatch).toHaveBeenCalledWith('myRecord/loadHistoricTestResult', year);
  });

  it('will not call dispatch for loadHistoricTestResult when data for specific year is empty', async () => {
    const year = 2021;
    mountPage({ historicTestResultData: [
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

    expect($store.dispatch).not.toBeCalledWith('myRecord/loadHistoricTestResult', year);
  });
});
