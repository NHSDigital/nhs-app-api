import crypto from 'crypto';
import { AuthorizationServiceConfiguration, AuthorizationRequest, RedirectRequestHandler } from '@openid/appauth';

const base64URLEncode = value =>
  value.toString('base64').replace(/\+/g, '-').replace(/\//g, '_').replace(/=/g, '');
const sha256 = value => crypto.createHash('sha256').update(value).digest();
const createChallenge = verifier => base64URLEncode(sha256(verifier));
const authorizationHandler = new RedirectRequestHandler();

export default class AuthorizationService {
  constructor(environmentConfig) {
    this.cidClientId = environmentConfig.CID_CLIENT_ID;
    this.redirectUri = environmentConfig.CID_REDIRECT_URI;
    this.configuration =
      new AuthorizationServiceConfiguration(environmentConfig.CID_AUTH_ENDPOINT, null, null);
  }

  // This rule is disabled because we need `createVerifier` to be at class level to expose it
  // to the code that imports the `AuthorizationService`.
  // eslint-disable-next-line class-methods-use-this
  createVerifier() {
    return base64URLEncode(crypto.randomBytes(32));
  }

  performLogin(verifier) {
    const challenge = createChallenge(verifier);
    const request = new AuthorizationRequest(
      this.cidClientId,
      this.redirectUri,
      undefined,
      undefined,
      undefined,
      {
        code_challenge: challenge,
        code_challenge_method: 'S256',
      },
    );

    return authorizationHandler.performAuthorizationRequest(this.configuration, request);
  }
}

