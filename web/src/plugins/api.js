/* eslint-disable */
import NHSOnlineApi from '../services/nhsonlineapi';

export default ({ app, store, res, req }) => {
  const api = new NHSOnlineApi({
    store,
    res,
    req,
    cookies: app.$cookies,
  });

  if(process.server) {
    api.cookie = app.context.req.headers.cookie;
  }
  app.$http = api;
};
