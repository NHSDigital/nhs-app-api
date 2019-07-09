import each from 'jest-each';
import myRecordAcceptance from '@/middleware/myRecordAcceptance';
import { initialState } from '@/store/modules/myRecord/mutation-types';
import { MYRECORD } from '@/lib/routes';
import { createStore } from '../helpers';
import { MY_RECORD_VISION_DIAGNOSIS_DETAIL, MY_RECORD_VISION_EXAMINATIONS_DETAIL, MY_RECORD_VISION_PROCEDURES_DETAIL, MY_RECORD_VISION_TEST_RESULTS_DETAIL } from '../../../src/lib/routes';

const createState = () => ({
  myRecord: initialState(),
});
const createApp = ({ redirect, route, store = createStore({ state: createState() }) }) => ({
  redirect,
  route,
  store,
});

describe('my-record acceptance middleware', () => {
  let app;

  describe('my-record-warning page', () => {
    each(['/my-record-warning', '/my-record-warning/'])
      .it('will redirect to /myrecord', (path) => {
        const redirect = jest.fn();
        app = createApp({
          redirect,
          route: {
            name: 'my-record-warning',
            path,
          },
        });

        myRecordAcceptance(app);
        expect(redirect).toHaveBeenCalledWith(MYRECORD.path);
      });
  });

  function checkForRedirect(path, name, hasAcceptedTerms) {
    const redirect = jest.fn();
    app = createApp({
      redirect,
      route: {
        name,
        path,
      },
    });
    app.store.state.myRecord.hasAcceptedTerms = hasAcceptedTerms;

    myRecordAcceptance(app);

    if (hasAcceptedTerms) {
      expect(redirect).not.toHaveBeenCalledWith(MYRECORD.path);
    } else {
      expect(redirect).toHaveBeenCalledWith(MYRECORD.path);
    }
  }

  describe('my-record details pages', () => {
    describe('my-record has accepted terms', () => {
      const hasAcceptedTerms = true;
      each(['/my-record/diagnosis-detail', '/my-record/diagnosis-detail/'])
        .it('will not redirect to my-record page when navigating to my-record diagnosis details page', (path) => {
          checkForRedirect(path, MY_RECORD_VISION_DIAGNOSIS_DETAIL.name, hasAcceptedTerms);
        });

      each(['/my-record/examinations-detail', '/my-record/examinations-detail/'])
        .it('will not redirect to my-record page when navigating to my-record examinations details page', (path) => {
          checkForRedirect(path, MY_RECORD_VISION_EXAMINATIONS_DETAIL.name, hasAcceptedTerms);
        });

      each(['/my-record/procedures-detail', '/my-record/procedures-detail/'])
        .it('will not redirect to my-record page when navigating to my-record procedures details page', (path) => {
          checkForRedirect(path, MY_RECORD_VISION_PROCEDURES_DETAIL.name, hasAcceptedTerms);
        });

      each(['/my-record/test-results-detail', '/my-record/test-results-detail/'])
        .it('will not redirect to my-record page when navigating to my-record test results details pages', (path) => {
          checkForRedirect(path, MY_RECORD_VISION_TEST_RESULTS_DETAIL.name, hasAcceptedTerms);
        });
    });

    describe('my-record has not accepted terms', () => {
      const hasAcceptedTerms = false;
      each(['/my-record/diagnosis-detail', '/my-record/diagnosis-detail/'])
        .it('will redirect to /my-record when navigation to my-record diagnosis details pages', (path) => {
          checkForRedirect(path, MY_RECORD_VISION_DIAGNOSIS_DETAIL.path, hasAcceptedTerms);
        });

      each(['/my-record/examinations-detail', '/my-record/examinations-detail/'])
        .it('will redirect to /my-record when navigation to my-record examinations details pages', (path) => {
          checkForRedirect(path, MY_RECORD_VISION_EXAMINATIONS_DETAIL.path, hasAcceptedTerms);
        });

      each(['/my-record/procedures-detail', '/my-record/procedures-detail/'])
        .it('will redirect to /my-record when navigation to my-record procedures details pages', (path) => {
          checkForRedirect(path, MY_RECORD_VISION_PROCEDURES_DETAIL.path, hasAcceptedTerms);
        });

      each(['/my-record/test-results-detail', '/my-record/test-results-detail/'])
        .it('will redirect to /my-record when navigation to my-record test results details pages', (path) => {
          checkForRedirect(path, MY_RECORD_VISION_TEST_RESULTS_DETAIL.path, hasAcceptedTerms);
        });
    });
  });
});
