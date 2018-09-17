/* eslint-disable no-param-reassign */
/* eslint-disable no-confusing-arrow */
const isHttps = env => !env.DISABLE_WEB_HTTPS;

const resolveApiHost = (host, port) => host
  .replace(/^(https?:\/\/)(web|www)/, '$1api')
  .replace(/:\d+$/, `:${port}`);

const resolveProtocol = env => isHttps(env) ? 'https' : 'http';

const resolveClientHost = href => href.replace(/^(https?:\/\/)([^/?]+).*$/, '$1$2');

const resolveRedirectUri = ({ protocol, suffix, webHost }) => {
  const host = protocol ? webHost.replace(/^https?:\/\//, `${protocol}://`) : webHost;
  return `${host}/${suffix}`;
};

const resolveServerHost = ({ env, req }) => `${resolveProtocol(env)}://${req.headers.host}`;

export default ({ env, req }) => {
  const apiPort = process.server ? env.API_PORT_SERVER : env.API_PORT;
  const webHost = process.server ?
    resolveServerHost({ env, req }) : resolveClientHost(window.location.href);
  const apiHost = resolveApiHost(webHost, apiPort);

  env.API_HOST = apiHost;
  env.API_HOST_SERVER = env.API_HOST_SERVER || apiHost;
  env.WEB_HOST = webHost;
  env.NATIVE_CID_REDIRECT_URI = resolveRedirectUri({ protocol: 'nhsapp', suffix: 'auth-return', webHost });
  env.CID_REDIRECT_URI = resolveRedirectUri({
    protocol: resolveProtocol(env),
    suffix: 'auth-return',
    webHost });
};
