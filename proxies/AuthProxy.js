import crypto from 'crypto';
import { AuthorizationServiceConfiguration, AuthorizationRequest, RedirectRequestHandler } from '@openid/appauth';
import Proxy from './Proxy';

const base64URLEncode = value =>
  value.toString('base64').replace(/\+/g, '-').replace(/\//g, '_').replace(/=/g, '');
const sha256 = value => crypto.createHash('sha256').update(value).digest();
const createChallenge = verifier => base64URLEncode(sha256(verifier));
const authorizationHandler = new RedirectRequestHandler();

class AuthProxy extends Proxy {

  constructor(environmentConfig) {
    super('', {});
    this.cidClientId = environmentConfig.CID_CLIENT_ID;
    this.redirectUri = environmentConfig.CID_REDIRECT_URI;
    this.configuration =
      new AuthorizationServiceConfiguration(environmentConfig.CID_AUTH_ENDPOINT, null, null);
  }

  login(verifier) {
    const challenge = createChallenge(verifier);
    const request = new AuthorizationRequest(
      this.cidClientId,
      this.redirectUri,
      undefined,
      undefined,
      undefined,
      { code_challenge: challenge },
    );

    return authorizationHandler.performAuthorizationRequest(this.configuration, request);
  }
}

export default AuthProxy;
