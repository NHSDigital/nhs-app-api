import crypto from 'crypto';

const base64URLEncode = value => value
  .toString('base64')
  .replace(/\+/g, '-')
  .replace(/\//g, '_')
  .replace(/=/g, '');
const sha256 = value => crypto
  .createHash('sha256')
  .update(value)
  .digest();
const createVerifier = () => base64URLEncode(crypto.randomBytes(32));
const createChallenge = verifier => base64URLEncode(sha256(verifier));

class AuthorisationService {
  constructor(environment) {
    this.nativeCidRedirectUri = environment.NATIVE_CID_REDIRECT_URI;
    this.webCidRedirectUri = environment.CID_REDIRECT_URI;
    this.cidClientId = environment.CID_CLIENT_ID;
    this.cidAuthEndpoint = environment.CID_AUTH_ENDPOINT;
  }

  generateLoginValues(state) {
    const verifier = createVerifier();
    const challenge = createChallenge(verifier);
    const myState = this.newState(this.cryptoGenerateRandom);

    const request = {
      scope: 'openid',
      clientId: this.cidClientId,
      redirectUri: this.getRedirectUri(state),
      responseType: 'code',
      state: myState,
      codeVerifier: verifier,
      codeChallenge: challenge,
      codeChallengeMethod: 'S256',
      authoriseUrl: this.cidAuthEndpoint,
    };

    return request;
  }

  getRedirectUri(state) {
    const device = state.device.source;
    if (device === 'android' || device === 'ios') {
      return this.nativeCidRedirectUri;
    }

    return this.webCidRedirectUri;
  }

  /* eslint-disable class-methods-use-this */
  newState(randomGenerator) {
    return randomGenerator(10);
  }

  /* eslint-disable class-methods-use-this */
  cryptoGenerateRandom() {
    const buffer = new Uint8Array(1);
    // fall back to Math.random() if nothing else is available
    for (let i = 0; i < 1; i += 1) {
      buffer[i] = Math.random();
    }
    const CHARSET = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    const state = [];
    for (let i = 0; i < buffer.byteLength; i += 1) {
      const index = (buffer[i] % CHARSET.length) || 0;
      state.push(CHARSET[index]);
    }
    return state.join('');
  }

  /* eslint-disable class-methods-use-this */
  stringify(input) {
    const encoded = [];
    /* eslint-disable no-restricted-syntax */
    for (const key in input) {
      /* eslint-disable no-prototype-builtins */
      if (input.hasOwnProperty(key) && input[key]) {
        encoded.push(`${encodeURIComponent(key)}=${encodeURIComponent(input[key])}`);
      }
    }
    return encoded.join('&');
  }
}

export default AuthorisationService;
