import TestResult from '@/pages/my-record/testresultdetail/_testResultId';
import { create$T, mount } from '../../../helpers';

const mountTestResult = ({ detailedTestResult, $t }) => mount(TestResult, {
  $t,
  state: {
    device: {
      isNativeApp: true,
    },
    myRecord: {
      detailedTestResult,
    },
  },
});

describe('test result', () => {
  let $t;
  let detailedTestResult;
  let wrapper;

  beforeEach(() => {
    $t = create$T();
    detailedTestResult = { data: undefined };
    wrapper = mountTestResult({ detailedTestResult, $t });
  });

  describe('has results', () => {
    const testResult = '<span id="testResult">an HTML test result</span>';

    beforeEach(() => {
      detailedTestResult.data = { testResult };
    });

    it('will not display `no test result section`', () => {
      expect(wrapper.find('#noTestResult').exists()).toBe(false);
    });

    describe('result details section', () => {
      let resultDetails;

      beforeEach(() => {
        resultDetails = wrapper.find('#resultDetails');
      });

      it('will exist', () => {
        expect(resultDetails.exists()).toBe(true);
      });

      it('will display test results html', () => {
        expect(resultDetails.find('#testResult').exists()).toBe(true);
      });
    });
  });

  describe('has no results', () => {
    it('will not display result details', () => {
      expect(wrapper.find('#testResult').exists()).toBe(false);
    });

    describe('no test result section', () => {
      let noTestResult;

      beforeEach(() => {
        noTestResult = wrapper.find('#noTestResult');
      });

      it('will exist', () => {
        expect(noTestResult.exists()).toBe(true);
      });

      it('will show the `no test result data` text', () => {
        expect(noTestResult.find('p').text()).toBe('translate_my_record.testresultdetail.noTestResultData');
      });
    });
  });
});
