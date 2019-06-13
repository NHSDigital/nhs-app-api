/* eslint-disable no-param-reassign */
import get from 'lodash/fp/get';

const applyUriFormat = (host, format) => format.replace('{host}', host);
const getHostHeader = get('headers.host');
const getOriginHeader = get('headers.x-origin');
const getWindowLocation = get('location.href');

export const getHost = (req) => {
  if (!req) {
    return getWindowLocation(window);
  }

  const host = getOriginHeader(req) || getHostHeader(req);
  if (host) {
    return host.split(',')
      .map(x => x.trim())[0];
  }
  return undefined;
};

const resolveBaseHost = req => getHost(req)
  .replace(/^https?:\/\//, '')
  .replace(/^(?:web|www)([.-]?)/, '$1')
  .replace(/(?:[:/?].*$)/, '');

const resolveUrlFormat = (formatName, env) => {
  if (env[formatName]) return env[formatName];
  throw new Error(`Cannot resolve URL format: ${formatName} from environment variables.`);
};

export const resolveApiClient = ({ req, env }) => {
  env.API_HOST = env.API_HOST || applyUriFormat(
    resolveBaseHost(req),
    resolveUrlFormat('URI_FORMAT_API_CLIENT', env),
  );

  return env.API_HOST;
};

export const resolveCidWeb = ({ req, env }) => {
  env.CID_REDIRECT_URI = env.CID_REDIRECT_URI || applyUriFormat(
    resolveBaseHost(req),
    resolveUrlFormat('URI_FORMAT_CID_REDIRECT_WEB', env),
  );

  return env.CID_REDIRECT_URI;
};

export const resolveCidNative = ({ req, env }) => {
  env.NATIVE_CID_REDIRECT_URI = /* env.NATIVE_CID_REDIRECT_URI || */ applyUriFormat(
    resolveBaseHost(req),
    resolveUrlFormat('URI_FORMAT_CID_REDIRECT_NATIVE', env),
  );
  return env.NATIVE_CID_REDIRECT_URI;
};

export default ({ env, req }) => {
  // const { env, req } = input;
  resolveApiClient({ env, req });
  resolveCidWeb({ env, req });
  resolveCidNative({ env, req });
};
