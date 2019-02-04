import ResultsPage from '@/pages/gp-finder/results';
import 'babel-polyfill';

describe(('GP Finder results page'), () => {
  describe(('getPracticeCodeFromNACSCode'), () => {
    it('will remove branch numbers from odscodes to give practice code', () => {
      // setup
      const odsCodeWithBranchCode = 'F11111001';
      const expectedOdsCodeWithBranchCode = 'F11111';

      const odsCodeWithoutBranchCode = 'F12345';
      const expectedOdsCodeWithoutBranchCode = 'F12345';

      const invalidOdsCode = '234';
      const expectedInvalidOdsCode = '234';

      const undefinedOdsCode = undefined;
      const expectedUndefinedOdsCode = undefined;

      // work
      const actualOdsCodeWithBranchCode =
        ResultsPage.methods.getPracticeCodeFromNACSCode(odsCodeWithBranchCode);

      const actualOdsCodeWithoutBranchCode =
        ResultsPage.methods.getPracticeCodeFromNACSCode(odsCodeWithoutBranchCode);

      const actualInvalidOdsCode =
        ResultsPage.methods.getPracticeCodeFromNACSCode(invalidOdsCode);

      const actualUndefinedOdsCode =
        ResultsPage.methods.getPracticeCodeFromNACSCode(undefinedOdsCode);

      // expect
      expect(actualOdsCodeWithBranchCode).toEqual(expectedOdsCodeWithBranchCode);

      expect(actualOdsCodeWithoutBranchCode).toEqual(expectedOdsCodeWithoutBranchCode);

      expect(actualInvalidOdsCode).toEqual(expectedInvalidOdsCode);

      expect(actualUndefinedOdsCode).toEqual(expectedUndefinedOdsCode);
    });
  });
});
