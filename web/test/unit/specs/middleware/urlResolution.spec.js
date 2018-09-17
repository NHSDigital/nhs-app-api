import urlResolution from '@/middleware/urlResolution';

describe('middleware/urlResolution', () => {
  const input = {
    env: {
      DISABLE_WEB_HTTPS: '',
    },
    req: {
      connection: {},
      headers: {
        host: 'foo.bar',
      },
    },
  };

  describe('server', () => {
    describe('local', () => {
      beforeEach(() => {
        input.env.API_PORT_SERVER = '8082';
        input.req.headers.host = 'web.local.bitraft.io:3000';
        input.env.DISABLE_WEB_HTTPS = 'true';
        process.server = true;
        urlResolution(input);
      });

      it('will set the API_HOST to api.local.bitraft.io', () => {
        expect(input.env.API_HOST).toBe('http://api.local.bitraft.io:8082');
      });

      it('will set the CID_REDIRECT_URI correctly', () => {
        expect(input.env.CID_REDIRECT_URI).toBe('http://web.local.bitraft.io:3000/auth-return');
      });
    });

    describe('scratch', () => {
      beforeEach(() => {
        input.req.headers.host = 'www-scratchc.dev.nonlive.nhsapp.service.nhs.uk';
        input.env.DISABLE_WEB_HTTPS = '';
        process.server = true;
        urlResolution(input);
      });

      it('will set the API_HOST to api-scratchc.dev.nonlive.nhsapp.service.nhs.uk', () => {
        expect(input.env.API_HOST).toBe('https://api-scratchc.dev.nonlive.nhsapp.service.nhs.uk');
      });

      it('will set the CID_REDIRECT_URI correctly', () => {
        expect(input.env.CID_REDIRECT_URI).toBe('https://www-scratchc.dev.nonlive.nhsapp.service.nhs.uk/auth-return');
      });
    });

    describe('production', () => {
      beforeEach(() => {
        input.req.headers.host = 'www-blue.production.nhsapp.service.nhs.uk';
        input.env.DISABLE_WEB_HTTPS = '';
        process.server = true;
        urlResolution(input);
      });

      it('will set the API_HOST to api-blue.production.nhsapp.service.nhs.uk', () => {
        expect(input.env.API_HOST).toBe('https://api-blue.production.nhsapp.service.nhs.uk');
      });

      it('will set the CID_REDIRECT_URI correctly', () => {
        expect(input.env.CID_REDIRECT_URI).toBe('https://www-blue.production.nhsapp.service.nhs.uk/auth-return');
      });
    });
    describe('staging', () => {
      beforeEach(() => {
        input.req.headers.host = 'www-green.staging.nhsapp.service.nhs.uk';
        input.env.DISABLE_WEB_HTTPS = '';
        process.server = true;
        urlResolution(input);
      });

      it('will set the API_HOST to api-green.staging.nhsapp.service.nhs.uk', () => {
        expect(input.env.API_HOST).toBe('https://api-green.staging.nhsapp.service.nhs.uk');
      });

      it('will set the CID_REDIRECT_URI correctly', () => {
        expect(input.env.CID_REDIRECT_URI).toBe('https://www-green.staging.nhsapp.service.nhs.uk/auth-return');
      });
    });
  });

  describe('client', () => {
    beforeEach(() => {
      process.server = false;
      urlResolution(input);
    });

    it('will set the API_HOST on the environment', () => {
      expect(input.env.API_HOST).not.toBeUndefined();
    });
  });
});
