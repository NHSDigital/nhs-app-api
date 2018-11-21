import noJsState from '@/middleware/noJsState';

const createReq = (key, json = '{"name":"fred"}') => {
  if (key) return { url: `/foo?${key}=${json}` };
  return { url: '/foo' };
};

describe('noJsState middleware', () => {
  let store;
  beforeEach(() => {
    store = {
      state: {
        name: 'barney',
      },
    };
  });

  describe('process on client', () => {
    beforeEach(() => {
      process.client = true;
    });

    it('will not update the state even with a valid query string key of "nojs"', () => {
      const req = createReq('nojs');
      noJsState({ store, req });
      expect(store.state.name).toEqual('barney');
    });
  });

  describe('process on server', () => {
    beforeEach(() => {
      process.client = false;
    });

    it('will add new properties to the state with a valid query string', () => {
      const req = createReq('nojs', '{"lastName":"flintstone"}');
      noJsState({ store, req });
      expect(store.state.name).toEqual('barney');
      expect(store.state.lastName).toEqual('flintstone');
    });

    it('will not update the state when the value is invalid json', () => {
      const req = createReq('nojs', 'foobar');
      noJsState({ store, req });
      expect(store.state.name).toEqual('barney');
    });

    it('will not update the state with no query string', () => {
      const req = createReq();
      noJsState({ store, req });
      expect(store.state.name).toEqual('barney');
    });

    it('will not update the state with an unknown query string key', () => {
      const req = createReq('unknown');
      noJsState({ store, req });
      expect(store.state.name).toEqual('barney');
    });

    it('will update the state with a query string key of "nojs"', () => {
      const req = createReq('nojs');
      noJsState({ store, req });
      expect(store.state.name).toEqual('fred');
    });

    it('will update the state with a query string key of "NOJS"', () => {
      const req = createReq('NOJS');
      noJsState({ store, req });
      expect(store.state.name).toEqual('fred');
    });

    it('will update the state with a query string key of "noJs"', () => {
      const req = createReq('noJs');
      noJsState({ store, req });
      expect(store.state.name).toEqual('fred');
    });

    it('will create a "noJs" property on the state', () => {
      const req = createReq('noJs');
      noJsState({ store, req });
      expect(store.state.noJs).not.toBeUndefined();
    });
  });
});
