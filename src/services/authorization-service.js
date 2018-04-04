import crypto from 'crypto';
import { AuthorizationServiceConfiguration, AuthorizationRequest, RedirectRequestHandler } from '@openid/appauth';

const authorizationHandler = new RedirectRequestHandler();
const base64URLEncode = value =>
  value.toString('base64').replace(/\+/g, '-').replace(/\//g, '_').replace(/=/g, '');
const configuration =
  new AuthorizationServiceConfiguration(process.env.CID_AUTH_ENDPOINT, null, null);
const sha256 = value => crypto.createHash('sha256').update(value).digest();

const createChallenge = verifier => base64URLEncode(sha256(verifier));

export const createVerifier = () => base64URLEncode(crypto.randomBytes(32));
export const performLogin = (redirectUri, verifier) => {
  const clientId = process.env.CID_CLIENT_IDs;
  const challenge = createChallenge(verifier);
  const request = new AuthorizationRequest(
    clientId,
    redirectUri,
    undefined,
    undefined,
    undefined,
    { code_challenge: challenge },
  );

  authorizationHandler.performAuthorizationRequest(configuration, request);
};
