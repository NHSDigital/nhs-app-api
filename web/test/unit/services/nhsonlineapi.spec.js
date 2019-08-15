import axios from 'axios';
import NHSOnlineApi from '@/services/v1nhsonlineapi';
import uuid from 'uuid';

jest.mock('uuid');

const mockNhsoRequestID = '123-456-789';

describe('services/nhsonlineapi', () => {
  const deferred = {
    resolve: jest.fn(),
    reject: jest.fn(),
  };

  beforeEach(() => {
    axios.mockClear();
    uuid.v4 = jest.fn().mockReturnValue(mockNhsoRequestID);
  });

  describe('request', () => {
    let store;
    let res;
    const accessToken = 'Access Token';
    const cookies = {
      get(name) {
        if (name === 'nhso.session') {
          return {
            accessToken,
          };
        }
        return undefined;
      },
    };
    const createRequestApi = () => new NHSOnlineApi({ store, res, cookies });
    const request = ({
      api,
      headers,
      parameters,
      queryParameters,
      url,
      useAccessToken = false,
    } = {}) =>
      (api || createRequestApi()).request({
        headers,
        parameters,
        queryParameters,
        url,
        deferred,
        useAccessToken,
      });

    beforeEach(() => {
      store = {
        dispatch: jest.fn(),
        state: {},
      };
      res = {
        setHeader: jest.fn(),
      };
    });

    describe('will send a header NHSO-Request-ID for an http request', () => {
      let headers;
      beforeEach(() => {
        headers = {};
      });

      describe('when process is server', () => {
        beforeEach(() => {
          process.server = true;
        });

        it('will read value from current request variable', () => {
          // act
          res.locals = {
            nhsoRequestId: mockNhsoRequestID,
          };
          const api = createRequestApi();
          request({ api, headers });

          // assert
          const headersSentInRequest = axios.mock.calls[0][0].headers;
          expect(headersSentInRequest).not.toBeNull();
          const nhsoRequestIdHeader = headersSentInRequest['NHSO-Request-ID'];
          expect(nhsoRequestIdHeader).toBe(mockNhsoRequestID);
          expect(uuid.v4).not.toHaveBeenCalled();
        });
      });

      describe('when process is client', () => {
        beforeEach(() => {
          process.server = false;
        });

        it('will generate a unique id for the http request', () => {
          // act
          request({ headers });

          // assert
          const headersSentInRequest = axios.mock.calls[0][0].headers;
          expect(headersSentInRequest).not.toBeNull();
          const nhsoRequestIdHeader = headersSentInRequest['NHSO-Request-ID'];
          expect(nhsoRequestIdHeader).toBe(mockNhsoRequestID);
          expect(uuid.v4).toHaveBeenCalled();
        });
      });
    });

    describe('use access token', () => {
      let headers;

      beforeEach(() => {
        headers = {};
      });

      describe('set to true', () => {
        beforeEach(() => {
          const api = createRequestApi();
          api.cookie = 'double chocolate fudge';
          request({ headers, useAccessToken: true });
        });

        it('will set the Authorization header', () => {
          expect(headers.Authorization).toBe(`Bearer ${accessToken}`);
        });

        describe('cookie exists', () => {
          beforeEach(() => {
            const api = createRequestApi();
            api.cookie = 'double chocolate fudge';
            request({ api, headers, useAccessToken: true });
          });

          it('will not set the Cookie header', () => {
            expect(headers.Cookie).toBeUndefined();
          });
        });
      });

      describe('set to false', () => {
        beforeEach(() => {
          request({ headers, useAccessToken: false });
        });

        it('will not set the Authorization header', () => {
          expect(headers.Authorization).toBeUndefined();
        });

        describe('cookie exists', () => {
          let api;

          beforeEach(() => {
            api = createRequestApi();
            api.cookie = 'double chocolate fudge';
            request({ api, headers, useAccessToken: false });
          });

          it('will set the Cookie header', () => {
            expect(headers.Cookie).toEqual(api.cookie);
          });
        });
      });
    });

    describe('cookie', () => {
      let headers;
      beforeEach(() => {
        headers = {};
      });

      describe('cookie exists', () => {
        it('will set the cookie in the headers if it is set on the NHSOnlineApi instance', () => {
          const api = createRequestApi();
          api.cookie = 'double chocolate fudge';
          request({ api, headers });
          expect(headers.Cookie).toEqual(api.cookie);
        });

        it('will prefer the received cookie parameter over the instance cookie', () => {
          const api = createRequestApi();
          const cookie = 'shortbread';
          api.cookie = 'chocolate chip';
          request({ api, headers, parameters: { cookie } });
          expect(headers.Cookie).toEqual(cookie);
        });

        it('will not set the Cookie header if no cookie is received', () => {
          request({ headers });
          expect(headers.Cookie).toBeUndefined();
        });
      });
    });

    describe('csrf token', () => {
      let headers;
      beforeEach(() => {
        headers = {};
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
          request({ headers });
          expect(headers['X-CSRF-TOKEN']).toEqual(store.state.session.csrfToken);
        });

        it('will prefer the received parameter over the store value', () => {
          const csrfToken = 'hoo';
          request({ headers, parameters: { csrfToken } });
          expect(headers['X-CSRF-TOKEN']).toEqual(csrfToken);
        });
      });

      describe('token does not exist', () => {
        it('will not set the "X-CSRF-TOKEN header in the request', () => {
          request({ headers });
          expect(headers['X-CSRF-TOKEN']).toBeUndefined();
        });
      });
    });

    let headers;
    beforeEach(() => {
      headers = {};
    });

    describe('query parameters', () => {
      it('will not include a query string when query parameters are empty', () => {
        const queryParameters = {};
        const url = 'http://foo/';
        request({ headers, queryParameters, url });
        expect(axios.mock.calls[0][0].url).toEqual(url);
      });

      it('will not include a query string when query parameters are undefined', () => {
        const queryParameters = undefined;
        const url = 'http://foo/';
        request({ headers, queryParameters, url });
        expect(axios.mock.calls[0][0].url).toEqual(url);
      });

      it('will include a query string when there are query parameters passed', () => {
        const url = 'http://foo/';
        const queryParameters = {
          boo: 'hoo',
          foo: 'bar',
        };
        request({ headers, queryParameters, url });
        expect(axios.mock.calls[0][0].url).toEqual(`${url}?boo=hoo&foo=bar`);
      });

      it('will URI encode the query string values and names', () => {
        const url = 'http://foo/';
        const queryParameters = {
          'works?': '&',
        };
        request({ headers, queryParameters, url });
        expect(axios.mock.calls[0][0].url).toEqual(`${url}?works%3F=%26`);
      });
    });

    it('will dispatch "http/isLoading"', () => {
      request({ headers });
      expect(store.dispatch).toHaveBeenCalledWith('http/isLoading');
    });
  });

  describe('query parameters', () => {
    it('merges parameters of same length successfully', () => {
      const queryParameters = ['1a', '2a', '3a'];
      const parameters = {
        $queryParameters: [
          '1b',
          '2b',
          '3b',
        ],
      };

      const resultParams = new NHSOnlineApi({}).mergeQueryParams(parameters, queryParameters);
      expect(resultParams).toEqual(parameters.$queryParameters);
    });

    it('merges parameters of differing sizes successfully', () => {
      const queryParameters = ['1a', '2a', '3a', '4a'];
      const parameters = {
        $queryParameters: [
          '1b',
          '2b',
          '3b',
        ],
      };

      const resultParams = new NHSOnlineApi({}).mergeQueryParams(parameters, queryParameters);
      expect(resultParams).toEqual(['1b', '2b', '3b', '4a']);
    });

    it('merges parameters of differing sizes successfully', () => {
      const queryParameters = ['1a', '2a', '3a'];
      const parameters = {
        $queryParameters: [
          '1b',
          '2b',
          '3b',
          '4b',
        ],
      };

      const resultParams = new NHSOnlineApi({}).mergeQueryParams(parameters, queryParameters);
      expect(resultParams).toEqual(['1b', '2b', '3b', '4b']);
    });
  });
});
