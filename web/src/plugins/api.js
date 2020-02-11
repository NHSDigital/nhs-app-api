/* eslint-disable no-param-reassign */
import NHSOnlineApiV1 from '@/services/v1nhsonlineapi';
import NHSOnlineApiV2 from '@/services/v2nhsonlineapi';

export default ({ app, store, res, req }) => {
  if (!app) {
    return;
  }

  const api = new NHSOnlineApiV1({
    store,
    res,
    req,
    cookies: app.$cookies,
  });

  const apiV2 = new NHSOnlineApiV2({
    store,
    res,
    req,
    cookies: app.$cookies,
  });

  if (process.server && app.context && app.context.req) {
    api.cookie = app.context.req.headers.cookie;
    apiV2.cookie = app.context.req.headers.cookie;
  }

  app.$http = api;
  app.$httpV2 = apiV2;
};
