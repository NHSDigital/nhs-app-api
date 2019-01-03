/* eslint-disable import/prefer-default-export */
import isString from 'lodash/fp/isString';

const defaultStyleResolver = styleName => `${styleName}-desktop`;

/**
 * Used to generate the correct style classname for styles based on if the device is native.
 * @param context - with the vuex store
 * @param value styles to exchange. This can either be a string or an array.
 * @param styleResolver - function used to derive the class name.
 * @returns {string[]} array of resolved classnames for either native or desktop
 */
export const getDynamicStyleName = (context, value, styleResolver = defaultStyleResolver) =>
  (isString(value) ? [value] : value)
    .map(styleName =>
      (context.$store.state.device.isNativeApp ? styleName : styleResolver(styleName)));

/**
 * Used to generate the correct style class for styles based on if the device is native.
 * @param context - with the vuex store
 * @param value styles to exchange. This can either be a string or an array.
 * @param styleResolver - function used to derive the class name.
 * @returns {*[]} array of resolved classnames for either native or desktop
 */
export const getDynamicStyle = (context, value, styleResolver = defaultStyleResolver) =>
  getDynamicStyleName(context, value, styleResolver)
    .map(className => context.$style[className]);

/**
 * Used as a function factory to swap classnames that match the keys for the value stored in the
 * object.
 *
 * @param mapping
 * @returns {function(*=): (*|string)}
 */
export const exchangeStyle = mapping =>
  styleName => (mapping[styleName] || defaultStyleResolver(styleName));
