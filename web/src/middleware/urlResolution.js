/* eslint-disable no-param-reassign */
import get from 'lodash/fp/get';

const applyUriFormat = (host, format) => format.replace('{host}', host);
const getWindowLocation = get('location.href');
const getHost = () => getWindowLocation(window);

const resolveBaseHost = () => getHost()
  .replace(/^https?:\/\//, '')
  .replace(/^(?:web|www)([.-]?)/, '$1')
  .replace(/(?:[:/?].*$)/, '');

const resolveUrlFormat = (formatName, env) => {
  if (env[formatName]) return env[formatName];
  throw new Error(`Cannot resolve URL format: ${formatName} from environment variables.`);
};

export const resolveApiClient = ({ env }) => {
  env.API_HOST = env.API_HOST || applyUriFormat(
    resolveBaseHost(),
    resolveUrlFormat('URI_FORMAT_API_CLIENT', env),
  );

  return env.API_HOST;
};

export const resolveCidWeb = ({ env }) => {
  env.CID_REDIRECT_URI = env.CID_REDIRECT_URI || applyUriFormat(
    resolveBaseHost(),
    resolveUrlFormat('URI_FORMAT_CID_REDIRECT_WEB', env),
  );

  return env.CID_REDIRECT_URI;
};

export const resolveCidNative = ({ env }) => {
  env.NATIVE_CID_REDIRECT_URI = env.NATIVE_CID_REDIRECT_URI || applyUriFormat(
    resolveBaseHost(),
    resolveUrlFormat('URI_FORMAT_CID_REDIRECT_NATIVE', env),
  );
  return env.NATIVE_CID_REDIRECT_URI;
};

export default ({ store, next }) => {
  const env = store.$env;
  resolveApiClient({ env });
  resolveCidWeb({ env });
  resolveCidNative({ env });
  return next();
};
