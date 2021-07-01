import AuthorisationService from '@/services/authorisation-service';

describe('Authorisation Service', () => {
  let environment;
  let authorisationService;

  const createService = () => new AuthorisationService(environment);

  beforeEach(() => {
    environment = {
      CID_REDIRECT_URI: 'mock cid redirect uri',
      CID_CLIENT_ID: 'mock cid client ID',
      CID_AUTH_ENDPOINT: 'mock cid auth endpoint',
      CID_P5_VECTOR_OF_TRUST_ENABLED: false,
    };
  });

  describe('generate login url', () => {
    let request;
    const cookies = {
      a: undefined,
      b: undefined,
      set: (a, b) => {
        cookies.a = a;
        cookies.b = b;
      },
    };
    const fidoAuthResponse = 'mock auth response';

    beforeEach(() => {
      authorisationService = createService();
      ({ request } = authorisationService.generateLoginUrl(
        {
          cookies,
          fidoAuthResponse,
        },
      ));
    });

    it('puts the correct redirect URI in the cookie', () => {
      expect(cookies.b.redirectUri).toEqual(`${environment.CID_REDIRECT_URI}/auth-return`);
    });

    it('adds a verifier to the cookie in the request', () => {
      expect(cookies.b.codeVerifier).toBeDefined();
    });

    it('uses the correct auth response in the request', () => {
      expect(request.fidoAuthResponse).toEqual(fidoAuthResponse);
    });

    it('uses the correct authorisation URL in the request', () => {
      expect(request.authoriseUrl).toEqual(environment.CID_AUTH_ENDPOINT_URL);
    });

    it('has a challenge in the request', () => {
      expect(request.codeChallenge).toBeDefined();
    });

    it('has default state in the request', () => {
      expect(request.state).toBe('A');
    });

    describe('Url has a redirect url', () => {
      beforeEach(() => {
        ({ request } = authorisationService.generateLoginUrl(
          {
            cookies,
            redirectTo: 'url',
            fidoAuthResponse,
          },
        ));
      });

      it('has redirect param value in state in the request', () => {
        expect(request.state).toBe('url');
      });

      describe('P5 vector of trust is enabled', () => {
        let loginUrl;

        beforeEach(() => {
          environment.CID_P5_VECTOR_OF_TRUST_ENABLED = true;
          authorisationService = createService();
          ({ loginUrl } = authorisationService.generateLoginUrl({
            cookies,
          }));
        });

        it('will have P5 vector of trust', () => {
          expect(loginUrl).toContain(`vtr=${encodeURIComponent('["P5.Cp.Cd", "P5.Cp.Ck", "P5.Cm", "P9.Cp.Cd", "P9.Cp.Ck", "P9.Cm"]')}`);
        });
      });

      describe('P5 vector of trust is disabled', () => {
        let loginUrl;

        beforeEach(() => {
          environment.CID_P5_VECTOR_OF_TRUST_ENABLED = false;
          authorisationService = createService();
          ({ loginUrl } = authorisationService.generateLoginUrl({
            cookies,
          }));
        });

        it('will have P9 vector of trust', () => {
          expect(loginUrl).toContain(`vtr=${encodeURIComponent('["P9.Cp.Cd", "P9.Cp.Ck", "P9.Cm"]')}`);
        });
      });
    });

    describe('generate uplift url', () => {
      let upliftUrl;

      beforeEach(() => {
        authorisationService = new AuthorisationService(environment);
        ({ upliftUrl } = authorisationService.generateUpliftUrl({
          cookies: { set: jest.fn() },
        }));
      });

      it('will have P9 vector of trust', () => {
        expect(upliftUrl).toContain(`vtr=${encodeURIComponent('["P9.Cp.Cd", "P9.Cp.Ck", "P9.Cm"]')}`);
      });
    });

    describe('stringify', () => {
      beforeEach(() => {
        authorisationService = new AuthorisationService(environment);
      });

      it('encodes a uri correctly', () => {
        const encoded = authorisationService.stringify('test');
        expect(encoded).toEqual('0=t&1=e&2=s&3=t');
      });
    });
  });
});
