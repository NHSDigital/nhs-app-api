import crypto from 'crypto';

const base64URLEncode = value =>
  value
    .toString('base64')
    .replace(/\+/g, '-')
    .replace(/\//g, '_')
    .replace(/=/g, '');
const sha256 = value =>
  crypto
    .createHash('sha256')
    .update(value)
    .digest();
const createChallenge = verifier => base64URLEncode(sha256(verifier));

class AuthorizationService {
  static createVerifier() {
    return base64URLEncode(crypto.randomBytes(32));
  }

  static getRedirectUri(state) {
    const device = state.device.source;
    if (device === 'android' || device === 'ios') {
      return process.env.NATIVE_CID_REDIRECT_URI;
    }

    return process.env.CID_REDIRECT_URI;
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
    const CHARSET =
      'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
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

  buildLoginObject(verifier, store) {
    const challenge = createChallenge(verifier);
    const myState = this.newState(this.cryptoGenerateRandom);
    const redirectUri = AuthorizationService.getRedirectUri(store.state);
    const clientId = process.env.CID_CLIENT_ID;
    const request = {
      scope: 'openid',
      client_id: clientId,
      redirect_uri: redirectUri,
      response_type: 'code',
      state: myState,
      code_challenge: challenge,
      code_challenge_method: 'S256',
      baseUrl: process.env.CID_AUTH_ENDPOINT,
      registerUrl: process.env.CID_REGISTER_ENDPOINT,
    };
    return request;
  }

  performLogin(verifier, redirectUri) {
    const challenge = createChallenge(verifier);
    const myState = this.newState(this.cryptoGenerateRandom);
    const clientId = process.env.CID_CLIENT_ID;

    const request = {
      scope: 'openid',
      client_id: clientId,
      redirect_uri: redirectUri,
      response_type: 'code',
      code_challenge: challenge,
      code_challenge_method: 'S256',
      state: myState,
    };

    const baseUrl = process.env.CID_AUTH_ENDPOINT;

    const query = this.stringify(request);
    if (process.client) {
      const url = `${baseUrl}?${query}`;
      window.location = url;
    }
  }

  performRegistration(verifier, redirectUri) {
    const challenge = createChallenge(verifier);
    const myState = this.newState(this.cryptoGenerateRandom);
    const clientId = process.env.CID_CLIENT_ID;

    const request = {
      redirect_uri: redirectUri,
      client_id: clientId,
      response_type: 'code',
      state: myState,
      code_challenge: challenge,
      code_challenge_method: 'S256',
    };

    const query = this.stringify(request);
    const baseUrl = process.env.CID_REGISTER_ENDPOINT;
    const url = `${baseUrl}?${query}`;
    if (process.client) {
      window.location = url;
    }
  }
}

export default AuthorizationService;
