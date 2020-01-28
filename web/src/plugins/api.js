/* eslint-disable */
import NHSOnlineApi from '@/services/v1nhsonlineapi';

export default ({ app, store, res, req }) => {

  if (!app) {
    return;
  }

  const api = new NHSOnlineApi({
    store,
    res,
    req,
    cookies: app.$cookies,
  });

  if(process.server && app.context && app.context.req) {
    api.cookie = app.context.req.headers.cookie;
  }

  app.$http = api;
};
