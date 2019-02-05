import noJsState from '@/middleware/noJsState';
import { PRESCRIPTION_CONFIRM_COURSES } from '@/lib/routes';
import { parseSelectedRepeatCourses } from '@/lib/noJs';

jest.mock('@/lib/noJs');

describe('noJsState middleware', () => {
  const redirect = jest.fn();
  let store;
  beforeEach(() => {
    store = {
      state: {
        name: 'barney',
        friend: {
          name: 'fred',
        },
      },
    };
  });


  const createGetReq = (key, json = '{"name":"fred"}') => {
    if (key) return { url: `/foo?${key}=${json}` };
    return { url: '/foo' };
  };

  const createPostReq = (key, json = '{"name":"fred"}', url = '/foo') => {
    const req = { url, path: url };

    if (key) {
      req.body = {
        [key]: json,
      };
    }

    return req;
  };

  describe('process on client', () => {
    beforeEach(() => {
      process.client = true;
    });

    it('will not update the state even with a valid query string key of "nojs"', () => {
      const req = createGetReq('nojs');
      noJsState({ redirect, store, req });
      expect(store.state.name).toEqual('barney');
    });

    it('will not update the state even with a valid body key of "nojs"', () => {
      const req = createPostReq('nojs');
      noJsState({ store, req });
      expect(store.state.name).toEqual('barney');
    });
  });

  describe('process on server', () => {
    beforeEach(() => {
      process.client = false;
    });

    describe('from query string', () => {
      it('will not update the state when the value is invalid json', () => {
        const req = createGetReq('nojs', 'foobar');
        noJsState({ store, req });
        expect(store.state.name).toEqual('barney');
      });

      it('will not update the state with no query string values', () => {
        const req = createGetReq();
        noJsState({ store, req });
        expect(store.state.name).toEqual('barney');
      });

      it('will not update the state with an unknown query string key', () => {
        const req = createGetReq('unknown');
        noJsState({ store, req });
        expect(store.state.name).toEqual('barney');
      });

      it('will add new properties to the state with a valid query string', () => {
        const req = createGetReq('nojs', '{"lastName":"flintstone"}');
        noJsState({ store, req });
        expect(store.state.name).toEqual('barney');
        expect(store.state.lastName).toEqual('flintstone');
      });

      it('will update the state with a query string key of "nojs"', () => {
        const req = createGetReq('nojs');
        noJsState({ store, req });
        expect(store.state.name).toEqual('fred');
      });

      it('will deep merge nojs properties', () => {
        const req = createGetReq('nojs', '{"friend":{ "name": "dino" }}');
        noJsState({ store, req });
        expect(store.state.friend.name).toEqual('dino');
      });
    });

    describe('from post body', () => {
      it('will not update the state when the value is invalid json', () => {
        const req = createPostReq('nojs', 'foobar');
        noJsState({ store, req });
        expect(store.state.name).toEqual('barney');
      });

      it('will not update the state with no form values', () => {
        const req = createPostReq();
        noJsState({ store, req });
        expect(store.state.name).toEqual('barney');
      });

      it('will not update the state with an unknown form key', () => {
        const req = createPostReq('unknown');
        noJsState({ store, req });
        expect(store.state.name).toEqual('barney');
      });

      it('will add new properties to the state with a valid form value', () => {
        const req = createPostReq('nojs', '{"lastName":"flintstone"}');
        noJsState({ store, req });
        expect(store.state.name).toEqual('barney');
        expect(store.state.lastName).toEqual('flintstone');
      });

      it('will update the state with a form key of "nojs"', () => {
        const req = createPostReq('nojs');
        noJsState({ store, req });
        expect(store.state.name).toEqual('fred');
      });

      it('will deep merge properties from "noJs"', () => {
        const req = createPostReq('nojs', '{"friend":{ "name": "dino" }}');
        noJsState({ store, req });
        expect(store.state.friend.name).toEqual('dino');
      });

      it('will merge in nojs property parameters', () => {
        const req = createPostReq('nojs.friend.name', 'wilma');
        noJsState({ store, req });
        expect(store.state.friend.name).toEqual('wilma');
      });

      it('will parse JSON from nojs property parameters', () => {
        const req = createPostReq('nojs.friend', '{ "name": "wilma" }');
        noJsState({ store, req });
        expect(store.state.friend.name).toEqual('wilma');
      });

      describe('not from nojs', () => {
        it('will not redirect if there is no additional form state to parse', () => {
          const req = createPostReq('nojs', '{}', '/foo');
          noJsState({ redirect, store, req });
          expect(redirect).toHaveBeenCalledTimes(0);
        });

        it('will redirect if parsedData shouldRedirect is true', () => {
          const req = createPostReq('nojs', '{}', PRESCRIPTION_CONFIRM_COURSES.path);
          parseSelectedRepeatCourses.mockReturnValueOnce({
            shouldRedirect: true,
            redirectPath: '',
            redirectQuery: {},
          });
          noJsState({ redirect, store, req });
          expect(redirect).toHaveBeenCalledTimes(1);
        });

        it('will merge additional form state to store', () => {
          const req = createPostReq('nojs', '{}', PRESCRIPTION_CONFIRM_COURSES.path);
          parseSelectedRepeatCourses.mockReturnValueOnce({
            shouldRedirect: false,
            state: {
              mockAdditionalState: {
                foo: 'bar',
              },
            },
          });
          noJsState({ redirect, store, req });
          expect(store.state.mockAdditionalState).toBeDefined();
        });
      });
    });
  });
});
