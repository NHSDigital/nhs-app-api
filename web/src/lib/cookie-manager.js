/* eslint-disable import/prefer-default-export */
import merge from 'deepmerge';
import isString from 'lodash/fp/isString';

export const setCookie = ({
  cookies,
  key,
  value,
  expires = null,
  path = '/',
  domain = null,
  secure = 'false',
  sameSite = 'Lax',
}) => {
  if (cookies) {
    if (value === undefined || value === '') {
      cookies.remove(key);
    } else {
      const isSecure = typeof secure === 'boolean' ? secure : secure === 'true';
      cookies.set(key, value, expires, path, domain, isSecure, sameSite);
    }
  }
};

export const mergeCookie = ({
  cookies,
  key,
  value,
  expires = null,
  path = '/',
  domain = null,
  secure = 'false',
  sameSite = 'Lax',
}) => {
  const cookie = cookies.get(key) || {};
  const mergedCookie = merge(cookie, value);
  const isSecure = typeof secure === 'boolean' ? secure : secure === 'true';
  cookies.set(key, mergedCookie, expires, path, domain, isSecure, sameSite);
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
