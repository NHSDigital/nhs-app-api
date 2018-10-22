import axios from 'axios';
import NHSOnlineApi from '@/services/nhsonlineapi';


describe('services/nhsonlineapi', () => {
  beforeEach(() => {
    axios.mockClear();
  });

  describe('request', () => {
    let store;
    beforeEach(() => {
      store = {
        dispatch: jest.fn(),
        state: {},
      };
    });

    describe('cookie', () => {
      let headers;
      beforeEach(() => {
        headers = [];
      });

      describe('cookie exists', () => {
        it('will set the cookie in the headers if it is set on the NHSOnlineApi instance', () => {
          const api = new NHSOnlineApi({ store });
          api.cookie = 'chocolate chip';
          api.request({ headers });
          expect(headers.Cookie).toEqual(api.cookie);
        });

        it('will prefer the received cookie parameter over the instance cookie', () => {
          const api = new NHSOnlineApi({ store });
          const cookie = 'shortbread';
          api.cookie = 'chocolate chip';
          api.request({ headers, parameters: { cookie } });
          expect(headers.Cookie).toEqual(cookie);
        });

        it('will not set the Cookie header if no cookie is received', () => {
          const api = new NHSOnlineApi({ store });
          api.request({ headers });
          expect(headers.Cookie).toBeUndefined();
        });
      });
    });

    describe('csrf token', () => {
      let headers;
      beforeEach(() => {
        headers = [];
      });

      describe('token exists', () => {
        beforeEach(() => {
          store.state = {
            session: {
              csrfToken: 'boo',
            },
          };
        });

        it('will set the "X-CSRF-TOKEN" header in the request from the store', () => {
          new NHSOnlineApi({ store }).request({ headers });
          expect(headers['X-CSRF-TOKEN']).toEqual(store.state.session.csrfToken);
        });

        it('will prefer the received parameter over the store value', () => {
          const csrfToken = 'hoo';
          new NHSOnlineApi({ store }).request({ headers, parameters: { csrfToken } });
          expect(headers['X-CSRF-TOKEN']).toEqual(csrfToken);
        });
      });

      describe('token does not exist', () => {
        it('will not set the "X-CSRF-TOKEN header in the request', () => {
          new NHSOnlineApi({ store }).request({ headers });
          expect(headers).toEqual([]);
        });
      });
    });

    describe('query parameters', () => {
      it('will not include a query string when query parameters are empty', () => {
        const queryParameters = {};
        const url = 'http://foo/';
        new NHSOnlineApi({ store }).request({ queryParameters, url });
        expect(axios.mock.calls[0][0].url).toEqual(url);
      });

      it('will not include a query string when query parameters are undefined', () => {
        const queryParameters = undefined;
        const url = 'http://foo/';
        new NHSOnlineApi({ store }).request({ queryParameters, url });
        expect(axios.mock.calls[0][0].url).toEqual(url);
      });

      it('will include a query string when there are query parameters passed', () => {
        const url = 'http://foo/';
        const queryParameters = {
          boo: 'hoo',
          foo: 'bar',
        };
        new NHSOnlineApi({ store }).request({ queryParameters, url });
        expect(axios.mock.calls[0][0].url).toEqual(`${url}?boo=hoo&foo=bar`);
      });

      it('will URI encode the query string values and names', () => {
        const url = 'http://foo/';
        const queryParameters = {
          'works?': '&',
        };
        new NHSOnlineApi({ store }).request({ queryParameters, url });
        expect(axios.mock.calls[0][0].url).toEqual(`${url}?works%3F=%26`);
      });
    });

    it('will dispatch "http/isLoading"', () => {
      new NHSOnlineApi({ store }).request({});
      expect(store.dispatch).toHaveBeenCalledWith('http/isLoading');
    });
  });
});
