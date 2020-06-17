import { capitalize, get, isEqual } from 'lodash/fp';
import { getType as lookupMimeType } from 'mime';
import Mime from 'mime/Mime';
import moment from 'moment-timezone';
import 'url-polyfill';
import { INDEX_PATH, EMPTY_PATH, INDEX_PATH_PARAM } from '@/router/paths';
import NativeCallbacks from '@/services/native-app';
import { EventBus, FOCUS_NHSAPP_ROOT } from '@/services/event-bus';

const protocol = 'http://';
const secureProtocol = 'https://';

const customMimeTypes = new Mime({
  'image/bmp': ['dib'],
});

export const datePart = (value, dateFormat) => {
  switch (dateFormat) {
    case 'Unknown':
    case 'YearMonthDay':
      return value ? moment.utc(value).format('D MMMM YYYY') : '';
    case 'Year':
      return value ? moment.utc(value).format('YYYY') : '';
    case 'YearMonth':
      return value ? moment.utc(value).format('MMMM YYYY') : '';
    case 'YearMonthDayTime':
      return value ? moment.utc(value).format('D MMMM YYYY h:mm a') : '';
    default:
      return value ? moment.utc(value).format('D MMMM YYYY') : '';
  }
};

export const mimeType = type => lookupMimeType(type) || customMimeTypes.getType(type) || 'application/octet-stream';

export const isEmptyArray = array => (Array.isArray(array) && array.length === 0);

export const isFalsy = value => !(value && value !== 'false');

export const isTruthy = value => !isFalsy(value);

export const isBlankString = value => typeof value !== 'string' || value.trim() === '';

export const isNumber = value => typeof value === 'number';

const get12HourTimeFormat = (dateTime, $t, capitaliseOutput = false) => {
  let localeValue;

  if (moment.isMoment(dateTime)) {
    const hours = dateTime.hours();
    const minutes = dateTime.minutes();

    if (minutes === 0) {
      if (hours === 12) {
        localeValue = $t('messageDateTimeFormats.midday');
      } else if (hours === 0) {
        localeValue = $t('messageDateTimeFormats.midnight');
      }
    }

    if (capitaliseOutput) {
      localeValue = capitalize(localeValue);
    }
  }

  return localeValue
    ? `[${localeValue}]`
    : 'h:mma';
};

export const formatInboxMessageTime = (inboxMessageTime, $t) => {
  const inboxMessageMoment = moment(inboxMessageTime);

  const formatConfig = {
    sameDay: get12HourTimeFormat(inboxMessageMoment, $t, true),
    lastDay: `[${$t('messageDateTimeFormats.yesterday')}]`,
    lastWeek: 'dddd',
    sameElse: 'D MMMM YYYY',
  };

  return inboxMessageMoment.calendar(moment.tz('Europe/London'), formatConfig);
};

export const formatIndividualMessageTime = (messageTime, $t) => {
  const messageMoment = moment(messageTime);

  const localeParams = {
    dateFormat: 'D MMMM YYYY',
    timeFormat: get12HourTimeFormat(messageMoment, $t),
  };

  const formatConfig = {
    sameDay: $t('messageDateTimeFormats.sentAtTimeTodayFormat', localeParams),
    lastDay: $t('messageDateTimeFormats.sentAtTimeYesterdayFormat', localeParams),
    lastWeek: $t('messageDateTimeFormats.sentDateAndTimeFormat', localeParams),
    sameElse: $t('messageDateTimeFormats.sentDateAndTimeFormat', localeParams),
  };

  return messageMoment.calendar(moment.tz('Europe/London'), formatConfig);
};

export const key = {
  ArrowDown: 'ArrowDown',
  ArrowLeft: 'ArrowLeft',
  Enter: 'Enter',
  Tab: 'Tab',
};

export const navigateBack = (self) => {
  self.$router.goBack();
};

export const readableBytes = (bytes) => {
  if (Number.isNaN(Number(bytes)) || bytes < 0) {
    return bytes;
  }

  if (bytes < 1000) {
    const convertedBytes = Math.round(bytes / 1);
    if (convertedBytes === bytes) {
      return `${convertedBytes}B`;
    }
    return readableBytes(convertedBytes);
  }

  if (bytes < 1000000) {
    const convertedBytes = Math.round(bytes / 1000);
    if (convertedBytes * 1000 === bytes) {
      return `${convertedBytes}KB`;
    }
    return readableBytes(convertedBytes * 1000);
  }

  return `${Number(parseFloat(bytes / 1000000).toFixed(2))}MB`;
};

export const createRouteByNameObject = ({ name, query, params, store }) => {
  const newParams = { ...params };
  const isLoggedIn = store.getters['session/isLoggedIn']();
  if (isLoggedIn && store.getters['linkedAccounts/isPatientIdNotEmpty']) {
    newParams.patientId = store.getters['linkedAccounts/getPatientId'];
  } else {
    delete newParams.patientId;
  }
  return { name, query, params: newParams };
};

export const redirectByName = ({ $router, $store }, name, query) => {
  const { currentRoute } = $router;
  const { params } = currentRoute;
  if (get('currentRoute.name')($router) === name) {
    const localQuery = !query || isEqual($router.currentRoute.query, query)
      ? {
        ...currentRoute.query,
        ts: moment().unix(),
      }
      : query;

    $router.push(createRouteByNameObject({ name, query: localQuery, params, store: $store }));
  } else if (!query) {
    $router.push(createRouteByNameObject({ name, params, store: $store }));
  } else {
    $router.push(createRouteByNameObject({ name, query, params, store: $store }));
  }
};

const getTrimmedPath = (path) => {
  if ((path !== INDEX_PATH) && path.length > 1 && path[0] === EMPTY_PATH) {
    return path.substr(1);
  }
  return path;
};

const definePatientIdPathParams = (store) => {
  const retObj = {
    patientId: '',
    paramString: `${INDEX_PATH_PARAM}${EMPTY_PATH}`,
  };

  if (store.getters['linkedAccounts/isPatientIdNotEmpty']) {
    retObj.patientId = store.getters['linkedAccounts/getPatientId'];
    retObj.paramString = INDEX_PATH_PARAM;
  }
  return retObj;
};

const getPathWithPatientIdPrefix = ({ trimmedPath, store }) => {
  const { patientId, paramString } = definePatientIdPathParams(store);
  let replacedPatientIdPath = INDEX_PATH.replace(paramString, patientId);

  if (trimmedPath !== INDEX_PATH && trimmedPath !== EMPTY_PATH) {
    replacedPatientIdPath = `${replacedPatientIdPath}${trimmedPath}`;
  }
  return replacedPatientIdPath;
};

// TODO add more unit tests here
export const checkIfPathShouldHavePatientPrefix = ({ path, store }) => {
  if (store.app.isNhsAppPath(path)) {
    return path;
  }
  const trimmedPath = getTrimmedPath(path);
  const completePath = getPathWithPatientIdPrefix({ trimmedPath, store });
  return store.app.isNhsAppPath(completePath) ? trimmedPath : undefined;
};

export const createRoutePathObject = ({ path, query, store }) => {
  let routeObject = {};
  if (query) {
    routeObject = { query };
  }
  const isLoggedIn = store.getters['session/isLoggedIn']();
  if (!isLoggedIn) {
    routeObject.path = path;
  } else {
    routeObject.path = getPathWithPatientIdPrefix({ trimmedPath: getTrimmedPath(path), store });
  }
  return routeObject;
};

export const redirectTo = ({ $router, $store }, path, query) => {
  const currentRoute = get('currentRoute.path')($router);
  if (currentRoute !== undefined && currentRoute.endsWith(path)) {
    const localQuery = !query || isEqual($router.currentRoute.query, query)
      ? {
        ...$router.currentRoute.query,
        ts: moment().unix(),
      }
      : query;

    $router.push(createRoutePathObject({ path, query: localQuery, store: $store }));
  } else if (!query) {
    $router.push(createRoutePathObject({ path, store: $store }));
  } else {
    $router.push(createRoutePathObject({ path, query, store: $store }));
  }
};

export const stripHtml = content => (content || '').replace(/<[^>]*>?/g, '');

export const displayedURL = (originalURL) => {
  if (!originalURL) {
    return '';
  }
  let displayUrl = originalURL;
  if (originalURL.includes(protocol)) {
    displayUrl = originalURL.replace(protocol, '');
  } else if (originalURL.includes(secureProtocol)) {
    displayUrl = originalURL.replace(secureProtocol, '');
  }
  return displayUrl;
};

export const hrefForURL = (originalURL) => {
  if (!originalURL) {
    return '';
  }
  if (originalURL.includes(secureProtocol) ||
    originalURL.includes(protocol)) {
    return originalURL;
  }
  return `//${originalURL}`;
};

export const getPathAndQuery = (url) => {
  try {
    const theUrl = new URL(url);
    return theUrl.pathname + theUrl.search;
  } catch (e) {
    return '';
  }
};

export const getThirdPartyJumpOff = (thirdPartyLocales, redirectPath) => {
  if (thirdPartyLocales.jumpOffs !== undefined) {
    for (let i = 0; i < thirdPartyLocales.jumpOffs.length; i += 1) {
      if (thirdPartyLocales.jumpOffs[i].path === decodeURIComponent(redirectPath)) {
        return thirdPartyLocales.jumpOffs[i];
      }
    }
  }
  return '';
};

export const getThirdPartyLocaleText = (thirdPartyLocales, redirectPath, feature, property) => {
  const jumpOff = getThirdPartyJumpOff(thirdPartyLocales, redirectPath);
  if (jumpOff === '') {
    return '';
  }
  return jumpOff[feature][property];
};

export const resetPageFocus = (store) => {
  if (store.state.device.isNativeApp) {
    NativeCallbacks.resetPageFocus();
  }

  EventBus.$emit(FOCUS_NHSAPP_ROOT);
  window.scrollTo(0, 0);
};
