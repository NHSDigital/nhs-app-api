import crypto from 'crypto';
import querystring from 'querystring';
import { setCookie } from '@/lib/cookie-manager';

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
const createVerifier = () => base64URLEncode(crypto.randomBytes(32));
const createChallenge = verifier => base64URLEncode(sha256(verifier));

const camelToUnderscore = key => key.replace(/([A-Z])/g, '_$1').toLowerCase();

const generateFidoUrl = (redirectData) => {
  const originalData = redirectData;
  const newData = {};
  Object.keys(originalData).forEach((key) => {
    if (key !== 'authoriseUrl') {
      newData[camelToUnderscore(key)] = originalData[key];
    }
  });
  return `${originalData.authoriseUrl}?${querystring.stringify(newData)}`;
};

const generateCIDUrl = (redirectData) => {
  const newData = {
    [camelToUnderscore('scope')]: redirectData.scope,
    [camelToUnderscore('clientId')]: redirectData.clientId,
    [camelToUnderscore('codeChallenge')]: redirectData.codeChallenge,
    [camelToUnderscore('codeChallengeMethod')]: redirectData.codeChallengeMethod,
    [camelToUnderscore('redirectUri')]: redirectData.redirectUri,
    [camelToUnderscore('state')]: redirectData.state,
    [camelToUnderscore('responseType')]: redirectData.responseType,
    [camelToUnderscore('vtr')]: redirectData.vtr,
  };

  return `${redirectData.authoriseUrl}?${querystring.stringify(newData)}`;
};

class AuthorisationService {
  constructor(environment) {
    this.nativeCidRedirectUri = environment.NATIVE_CID_REDIRECT_URI;
    this.webCidRedirectUri = environment.CID_REDIRECT_URI;
    this.cidClientId = environment.CID_CLIENT_ID;
    this.cidAuthEndpoint = environment.CID_AUTH_ENDPOINT_URL;
    this.cidP5VectorOfTrustEnabled = environment.CID_P5_VECTOR_OF_TRUST_ENABLED;
    this.secureCookies = environment.SECURE_COOKIES;
  }

  generateLoginUrl({ isNativeApp, redirectTo, cookies, fidoAuthResponse }) {
    return this.generateAuthUrl({
      isNativeApp,
      redirectTo,
      cookies,
      fidoAuthResponse,
      p5VectorOfTrust: this.cidP5VectorOfTrustEnabled,
    });
  }

  generateUpliftUrl({ isNativeApp, cookies }) {
    const { loginUrl } = this.generateAuthUrl({
      isNativeApp,
      cookies,
      p5VectorOfTrust: false,
    });

    return { upliftUrl: loginUrl };
  }

  generateAuthUrl({
    isNativeApp,
    redirectTo,
    cookies,
    fidoAuthResponse,
    p5VectorOfTrust }) {
    const verifier = createVerifier();
    const challenge = createChallenge(verifier);
    const myState = this.newState(this.cryptoGenerateRandom);
    const redirectUri = this.getRedirectUri(isNativeApp);
    const request = {
      scope: 'openid profile email nhs_app_credentials gp_integration_credentials profile_extended',
      clientId: this.cidClientId,
      redirectUri,
      responseType: 'code',
      state: redirectTo || myState,
      codeVerifier: verifier,
      codeChallenge: challenge,
      codeChallengeMethod: 'S256',
      authoriseUrl: this.cidAuthEndpoint,
      fidoAuthResponse,
      vtr: p5VectorOfTrust ? '["P5.Cp.Cd", "P5.Cp.Ck", "P5.Cm", "P9.Cp.Cd", "P9.Cp.Ck", "P9.Cm"]' : '["P9.Cp.Cd", "P9.Cp.Ck", "P9.Cm"]',
    };

    setCookie({
      key: 'nhso.auth',
      value: {
        redirectUri,
        codeVerifier: verifier,
      },
      cookies,
      secure: this.secureCookies,
    });

    let responseUrl;
    if (fidoAuthResponse === undefined) {
      responseUrl = generateCIDUrl(request);
    } else {
      responseUrl = generateFidoUrl(request);
    }

    return { loginUrl: responseUrl, request };
  }

  getRedirectUri(isNativeApp) {
    return isNativeApp ? this.nativeCidRedirectUri : this.webCidRedirectUri;
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
}

export default AuthorisationService;
