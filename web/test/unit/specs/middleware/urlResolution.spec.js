import urlResolution from '@/middleware/urlResolution';

describe('middleware/urlResolution', () => {
  let input;

  beforeEach(() => {
    input = {
      env: {
        URI_FORMAT_API_CLIENT: 'http://api.{host}',
        URI_FORMAT_API_SERVER: 'http://api.{host}:8082',
        URI_FORMAT_CID_REDIRECT_WEB: 'http://web.{host}/auth-return',
        URI_FORMAT_CID_REDIRECT_NATIVE: 'nhsapp://web.{host}/auth-return',
      },
      req: {
        connection: {},
        headers: {},
      },
    };
  });

  describe('server', () => {
    beforeEach(() => {
      process.server = true;
      input.req.headers.host = 'foo.bar';
    });

    describe('x-origin header exists', () => {
      describe('x-origin has a single value', () => {
        beforeEach(() => {
          input.req.headers['x-origin'] = 'www.some.web.host';
          urlResolution(input);
        });

        it('will correctly set the API_HOST from the `x-origin` header', () => {
          expect(input.env.API_HOST).toBe('http://api.some.web.host');
        });

        it('will correctly set the API_SERVER_HOST from the `x-origin` header', () => {
          expect(input.env.API_HOST_SERVER).toBe('http://api.some.web.host:8082');
        });

        it('will correctly set the CID_REDIRECT_URI from the `x-origin` header', () => {
          expect(input.env.CID_REDIRECT_URI).toBe('http://web.some.web.host/auth-return');
        });

        it('will correctly set the CID_REDIRECT_URI_NATIVE from the `x-origin` header', () => {
          expect(input.env.NATIVE_CID_REDIRECT_URI).toBe('nhsapp://web.some.web.host/auth-return');
        });
      });

      describe('x-origin has comma separated values', () => {
        beforeEach(() => {
          input.req.headers['x-origin'] = 'www.first.web.host, www.some.web.host';
          urlResolution(input);
        });

        it('will correctly set the API_HOST from the first `x-origin` header value', () => {
          expect(input.env.API_HOST).toBe('http://api.first.web.host');
        });

        it('will correctly set the API_SERVER_HOST from the first `x-origin` header value', () => {
          expect(input.env.API_HOST_SERVER).toBe('http://api.first.web.host:8082');
        });

        it('will correctly set the CID_REDIRECT_URI from the first `x-origin` header value', () => {
          expect(input.env.CID_REDIRECT_URI).toBe('http://web.first.web.host/auth-return');
        });

        it('will correctly set the CID_REDIRECT_URI_NATIVE from the first `x-origin` header value', () => {
          expect(input.env.NATIVE_CID_REDIRECT_URI).toBe('nhsapp://web.first.web.host/auth-return');
        });
      });
    });

    describe('x-origin header does not exist', () => {
      beforeEach(() => {
        input.req.headers['x-origin'] = undefined;
        urlResolution(input);
      });

      it('will correctly set the API_HOST from the `Host` header', () => {
        expect(input.env.API_HOST).toBe('http://api.foo.bar');
      });

      it('will correctly set the API_SERVER_HOST from the `Host` header', () => {
        expect(input.env.API_HOST_SERVER).toBe('http://api.foo.bar:8082');
      });

      it('will correctly set the CID_REDIRECT_URI from the `Host` header', () => {
        expect(input.env.CID_REDIRECT_URI).toBe('http://web.foo.bar/auth-return');
      });

      it('will correctly set the CID_REDIRECT_URI_NATIVE from the `Host` header', () => {
        expect(input.env.NATIVE_CID_REDIRECT_URI).toBe('nhsapp://web.foo.bar/auth-return');
      });
    });
  });
});
