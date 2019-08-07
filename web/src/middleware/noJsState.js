/* eslint-disable no-param-reassign */
import flow from 'lodash/fp/flow';
import get from 'lodash/fp/get';
import isString from 'lodash/fp/isString';
import mapKeys from 'lodash/fp/mapKeys';
import set from 'lodash/fp/set';
import qs from 'qs';

const NO_JS_PARAM_PREFIX = 'nojs.';

const getQueryString = ({ url }) => {
  const index = url.indexOf('?');
  return index === -1 ? undefined : url.substr(index + 1);
};

const parseJson = (json, mustParse = true) => {
  if (!json) return {};
  if (!isString(json)) return json;
  try {
    return JSON.parse(json);
  } catch (e) {
    return mustParse ? {} : json;
  }
};

const mergeNamed = (body) => {
  const named = {};
  let altered = Object.keys(body).reduce((accumulator, key) => {
    if (key.indexOf(NO_JS_PARAM_PREFIX) === 0) {
      named[key.substr(NO_JS_PARAM_PREFIX.length)] = parseJson(body[key], false);
    } else {
      accumulator[key] = body[key];
    }

    return accumulator;
  }, {});

  mapKeys((key) => {
    altered = set(key)(named[key], altered);
  })(named);

  return altered;
};

const parseBody = req => mergeNamed({
  ...parseJson(get('body.nojs')(req)),
  ...get('body')(req),
});

const parseQs = req => flow([
  getQueryString,
  qs.parse,
  get('nojs'),
  parseJson,
])(req);

export default ({ req, store }) => {
  if (process.client) return;
  const qsNoJs = parseQs(req);
  const bodyNoJs = parseBody(req);
  const noJs = { ...qsNoJs, ...bodyNoJs };

  if (noJs) {
    const merged = { ...store.state, ...noJs };
    mapKeys((key) => {
      store.state[key] = merged[key];
    })(merged);
  }
};
