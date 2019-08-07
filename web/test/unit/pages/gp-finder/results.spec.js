import ResultsPage from '@/pages/gp-finder/results';
import { mount } from '../../helpers';

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

  describe(('results section'), () => {
    const mountPage = ($store, data) => {
      const page = mount(ResultsPage, {
        $store,
        data,
      });
      page.vm.goToUrl = jest.fn();
      return page;
    };

    const createStore = () => ({
      dispatch: jest.fn(),
      state: {
        device: {
          source: 'Android',
        },
        throttling: {
          searchResults: {
            organisations: [
              {
                organisationName: 'org1',
                nacsCode: 'abc',
              },
              {
                organisationName: 'org2',
                nacsCode: 'def',
              },
            ],
            technicalError: false,
            noResultsFound: false,
            tooManyResults: false,
          },
          searchQuery: 'boom',
        },
      },
    });

    it('a user can click on a result which triggers gpPracticeClicked', () => {
      // arrange
      const $store = createStore();

      // simulate what async data does and copy things from store to data
      // so it's ready for mounted()
      const { throttling } = $store.state;

      const data = () => ({
        technicalError: throttling.searchResults.technicalError,
        noResultsFound: throttling.searchResults.noResultsFound,
        tooManyResults: throttling.searchResults.tooManyResults,
        organisations: throttling.searchResults.organisations,
        searchQuery: throttling.searchQuery,
      });

      const wrapper = mountPage($store, data);

      const organisationToClickOn = throttling.searchResults.organisations[0];

      wrapper.vm.gpPracticeClicked = jest.fn();
      const button = wrapper.find(`#btnGpPractice-${organisationToClickOn.nacsCode}`);
      expect(button).toBeDefined();

      // act
      button.trigger('click');

      // assert
      expect(button.text()).toBe(organisationToClickOn.organisationName);
      expect(wrapper.vm.gpPracticeClicked).toHaveBeenCalled();
    });
  });
});
