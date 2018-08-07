/* eslint-disable */
import NHSOnlineApi from '../services/nhsonlineapi';

export default ({ app, store, res }) => {
  const api = new NHSOnlineApi({
    domain: process.server ? process.env.API_HOST_SERVER : process.env.API_HOST,
    store,
    res,
    cookies: app.$cookies,
  });

  if(process.server) {
    api.cookie = app.context.req.headers.cookie;
  }
  app.$http = api;
};
