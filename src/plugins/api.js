/* eslint-disable */
import NHSOnlineApi from '../services/nhsonlineapi';

export default ({ app, store }) => {
  const api = new NHSOnlineApi({
    domain: process.env.API_HOST,
    store,
  });

  if(process.server) {
    api.cookie = app.context.req.headers.cookie;
  }
  app.$http = api;
};
