import urlResolution from '@/middleware/urlResolution';

describe('middleware/urlResolution', () => {
  let context;

  beforeEach(() => {
    context = {
      store: {
        $env: {
          URI_FORMAT_API_CLIENT: 'http://api{host}',
          URI_FORMAT_CID_REDIRECT_WEB: 'http://web{host}/auth-return',
          URI_FORMAT_CID_REDIRECT_NATIVE: 'nhsapp://web{host}/auth-return',
        },
      },
      next: jest.fn(),
    };
  });

  describe('urlResolution', () => {
    let href;

    describe('window location is http://web.local.bitraft.io:3000/', () => {
      beforeEach(() => {
        href = 'http://web.local.bitraft.io:3000/';
        delete window.location;
        window.location = {
          href,
        };
        urlResolution(context);
      });

      it('will correctly set the API_HOST', () => {
        expect(context.store.$env.API_HOST).toBe('http://api.local.bitraft.io');
      });

      it('will correctly set the CID_REDIRECT_URI', () => {
        expect(context.store.$env.CID_REDIRECT_URI).toBe('http://web.local.bitraft.io/auth-return');
      });

      it('will correctly set the NATIVE_CID_REDIRECT_URI', () => {
        expect(context.store.$env.NATIVE_CID_REDIRECT_URI).toBe('nhsapp://web.local.bitraft.io/auth-return');
      });
    });
  });
});
