/* eslint-disable import/prefer-default-export */
import merge from 'deepmerge';
import isString from 'lodash/fp/isString';

export const setCookie = ({ cookies, key, value, options = {} }) => {
  if (cookies) {
    const sanitised = value === '' ? undefined : value;

    if (sanitised) {
      cookies.set(key, sanitised, { ...options, path: '/' });
    } else {
      cookies.remove(key);
    }
  }
};

export const mergeCookie = ({ cookies, key, value, options = {} }) => {
  const cookie = cookies.get(key) || {};
  const mergedCookie = merge(cookie, value);
  cookies.set(key, mergedCookie, { ...options, path: '/' });
};

export const removeCookies = ({ cookies, key = [] }) => {
  let keysToRemove = [];
  if (isString(key)) {
    keysToRemove.push(key);
  } else {
    keysToRemove = key;
  }

  keysToRemove.forEach((keyToRemove) => {
    cookies.remove(keyToRemove);
  });
};
