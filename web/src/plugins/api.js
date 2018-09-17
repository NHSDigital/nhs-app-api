/* eslint-disable */
import NHSOnlineApi from '../services/nhsonlineapi';
import urlResolution from '../middleware/urlResolution';

export default ({ app, store, res }) => {
  urlResolution({ env: app.$env, req: app.context.req });
  const api = new NHSOnlineApi({
    domain: process.server ? app.$env.API_HOST_SERVER : app.$env.API_HOST,
    store,
    res,
    cookies: app.$cookies,
  });

  if(process.server) {
    api.cookie = app.context.req.headers.cookie;
  }
  app.$http = api;
};
