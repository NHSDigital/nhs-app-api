import myRecordAcceptance from '@/middleware/myRecordAcceptance';
import { initialState } from '@/store/modules/myRecord/mutation-types';
import { createStore } from '../helpers';

const createState = () => ({
  myRecord: initialState(),
});
const createApp = ({ route, store = createStore({ state: createState() }) }) => ({
  route,
  store,
});

describe('my-record acceptance middleware', () => {
  let app;
  describe('route not in my-record', () => {
    beforeEach(() => {
      app = createApp({
        route: {
          name: 'Login',
          path: '/login',
        },
      });
    });

    it('will dispatch "myRecord/resetTerms" when the terms are accepted', () => {
      app.store.state.myRecord.hasAcceptedTerms = true;
      myRecordAcceptance(app);
      expect(app.store.dispatch).toHaveBeenCalledWith('myRecord/resetTerms');
    });

    it('will not dispatch "myRecord/resetTerms" when terms are not accepted', () => {
      app.store.state.myRecord.hasAcceptedTerms = false;
      myRecordAcceptance(app);
      expect(app.store.dispatch).not.toHaveBeenCalledWith('myRecord/resetTerms');
    });
  });

  describe('my-record index page', () => {
    beforeEach(() => {
      app = createApp({
        route: {
          name: 'my-record',
          path: '/my-record',
        },
      });
    });

    it('will not dispatch "myRecord/resetTerms" when the terms are accepted', () => {
      app.store.state.myRecord.hasAcceptedTerms = true;
      myRecordAcceptance(app);
      expect(app.store.dispatch).not.toHaveBeenCalledWith('myRecord/resetTerms');
    });

    it('will not dispatch "myRecord/resetTerms" when the terms are not accepted', () => {
      app.store.state.myRecord.hasAcceptedTerms = false;
      myRecordAcceptance(app);
      expect(app.store.dispatch).not.toHaveBeenCalledWith('myRecord/resetTerms');
    });
  });

  describe('my-record test results page', () => {
    beforeEach(() => {
      app = createApp({
        route: {
          name: 'my-record-testresultdetail',
          path: '/my-record/testresultdetail/1',
        },
      });
    });

    it('will not dispatch "myRecord/resetTerms" when the terms are accepted', () => {
      app.store.state.myRecord.hasAcceptedTerms = true;
      myRecordAcceptance(app);
      expect(app.store.dispatch).not.toHaveBeenCalledWith('myRecord/resetTerms');
    });

    it('will not dispatch "myRecord/resetTerms" when the terms are not accepted', () => {
      app.store.state.myRecord.hasAcceptedTerms = false;
      myRecordAcceptance(app);
      expect(app.store.dispatch).not.toHaveBeenCalledWith('myRecord/resetTerms');
    });
  });
});
