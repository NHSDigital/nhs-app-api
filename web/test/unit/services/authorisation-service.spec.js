import AuthorisationService from '@/services/authorisation-service';
import Sources from '@/lib/sources';

const environment = {
  NATIVE_CID_REDIRECT_URI: 'mock native cid redirect uri',
  CID_REDIRECT_URI: 'mock cid redirect uri',
  CID_CLIENT_ID: 'mock cid client ID',
  CID_AUTH_ENDPOINT: 'mock cid auth endpoint',
};


describe('login values generation', () => {
  const authorisationService = new AuthorisationService(environment);

  const source = Sources.Android;
  const cookies = {
    a: undefined,
    b: undefined,
    set: (a, b) => {
      cookies.a = a;
      cookies.b = b;
    },
  };
  const fidoAuthResponse = 'mock auth response';

  const { request } = authorisationService.generateLoginUrl({ source, cookies, fidoAuthResponse });

  it('uses the correct source in the request', () => {
    const redirectUri = authorisationService.getRedirectUri(source);
    expect(request.redirectUri).toEqual(redirectUri);
  });

  it('puts the correct redirect URI in the cookie', () => {
    expect(cookies.b.redirectUri).toEqual(environment.NATIVE_CID_REDIRECT_URI);
  });

  it('adds a verifier to the cookie in the request', () => {
    expect(cookies.b.codeVerifier).toBeDefined();
  });

  it('uses the correct auth response in the request', () => {
    expect(request.fidoAuthResponse).toEqual(fidoAuthResponse);
  });

  it('uses the correct authorisation URL in the request', () => {
    expect(request.authoriseUrl).toEqual(environment.CID_AUTH_ENDPOINT);
  });

  it('has a challenge in the request', () => {
    expect(request.codeChallenge).toBeDefined();
  });
});

describe('redirect URI', () => {
  const authorisationService = new AuthorisationService(environment);

  it('uses the correct URI for Web', () => {
    const uri = authorisationService.getRedirectUri(Sources.Web);
    expect(uri).toEqual(environment.CID_REDIRECT_URI);
  });

  it('uses the correct URI for IOS', () => {
    const uri = authorisationService.getRedirectUri(Sources.iOS);
    expect(uri).toEqual(environment.NATIVE_CID_REDIRECT_URI);
  });

  it('uses the correct URI for android', () => {
    const uri = authorisationService.getRedirectUri(Sources.Android);
    expect(uri).toEqual(environment.NATIVE_CID_REDIRECT_URI);
  });
});

describe('stringify', () => {
  const authorisationService = new AuthorisationService(environment);

  it('encodes a uri correctly', () => {
    const encoded = authorisationService.stringify('test');
    expect(encoded).toEqual('0=t&1=e&2=s&3=t');
  });
});
