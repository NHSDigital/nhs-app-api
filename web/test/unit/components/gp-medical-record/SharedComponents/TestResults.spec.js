import testResultsComponent from '@/components/gp-medical-record/SharedComponents/TestResults';
import MedicalRecordCardGroupItem from '@/components/gp-medical-record/SharedComponents/MedicalRecordCardGroupItem';
import i18n from '@/plugins/i18n';
import { createStore, shallowMount } from '../../../helpers';

let page;
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

const mountComponent = ({ historicTestResultData, year } = {}) => {
  const testResults = {
    data: historicTestResultData,
    hasErrored: false,
    hasAccess: true,
  };


  $store = createStore({
    state: {
      device: { isNativeApp: false },
      myRecord: {
        record: {
          supplier: 'TPP',
          historicTestResults: {
            data: {},
            hasErrored: false,
            hasAccess: true,
            _2021: testResults,
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

  page = shallowMount(testResultsComponent, {
    $store,
    $router,
    mountOpts: { i18n },
    propsData: {
      results: testResults,
    },
  });
};

describe('gp-medical-record past test results', () => {
  it('will display an error componet when no results exist', async () => {
    const year = 2021;
    mountComponent({ historicTestResultData: [], year });
    await page.vm.$nextTick();
    const errorComponent = page.find('dcr-error-no-access-gp-record-stub');
    expect(errorComponent.exists()).toBe(true);
  });

  it('will render a card for every test result', async () => {
    mountComponent({ historicTestResultData: [
      createTestResults({
        associatedTexts: [' first associated texts'],
        date: { value: '2022-01-10T12:03:44+00:00' },
        description: 'first description text',
        id: '1',
        testResultChildLineItems: [
          {
            associatedTexts: ['first child associated texts'],
            description: 'first child description text',
          },
        ],
      }),
      createTestResults({
        associatedTexts: ['second associated texts'],
        date: { value: '2022-01-01T12:03:44+00:00' },
        description: 'second description text',
        id: '2',
        testResultChildLineItems: [
          {
            associatedTexts: ['second child associated texts'],
            description: 'second child description text',
          },
        ],
      }),
    ],
    });

    await page.vm.$nextTick();
    const results = page.findAll(MedicalRecordCardGroupItem);

    const firstResult = results.wrappers[0].find('[id="view-test-results-0"]');
    const firstResultHtml = firstResult.html();

    const secondResult = results.wrappers[1].find('[id="view-test-results-1"]');
    const secondResultHtml = secondResult.html();


    expect(firstResult.exists()).toBe(true);
    expect(firstResultHtml.includes('first description text')).toBe(true);
    expect(secondResult.exists()).toBe(true);
    expect(secondResultHtml.includes('second description text')).toBe(true);
  });
});
