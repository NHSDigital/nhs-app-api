/* eslint-disable no-underscore-dangle */
import isEmpty from 'lodash/fp/isEmpty';
import moment from 'moment';
import { PATIENT_ID_REGEX_PATTERN, APPOINTMENTS_PATH, TERMSANDCONDITIONS_PATH } from '@/router/paths';

const APP_ID = 'nhs:app';
const pageNamePrefix = `${APP_ID}`;
const patientIdRegex = new RegExp(`^${PATIENT_ID_REGEX_PATTERN}$`, 'i');

const getFields = (path) => {
  let fields = path.split('/').slice(1);
  if (fields.length > 0 && fields[0] === 'patient') {
    fields = fields.slice(1); // remove patient
  }
  if (fields.length > 0 && fields[0].match(patientIdRegex)) {
    fields = fields.slice(1); // remove patient guid
  }
  return fields;
};


export default function (app, store, to, router) {
/* eslint-disable-next-line no-param-reassign */
  window.digitalData = {};
  window.digitalData = (() => {
    const routePath = to.fullPath;
    const [path] = routePath.split('?');
    const fields = getFields(path);
    const pageUrl = window.location.hostname + path;
    const { domain } = document;
    const { userAgent } = navigator;
    const primaryCategory = fields[0] || 'home';
    const subCategory1 = fields[1] || '';
    const subCategory2 = fields[2] || '';
    const subCategory3 = fields[3] || '';
    const unqPageIdentifier = subCategory3 || subCategory2 || subCategory1 || primaryCategory;

    if (isEmpty(fields) || isEmpty(fields[0])) fields[0] = 'home';
    const pageName = fields.reduce((combined, field) => `${combined}:${field}`, pageNamePrefix);

    window.digitalData = {
      page: {
        pageInfo: {
          pageName,
          destinationURL: pageUrl,
          type: 'dynamic',
          referrer: router.currentRoute.path,
          referringURL: window.location.hostname,
          environment: domain,
          urlParams: window.location.origin + router.currentRoute.fullPath,
        },
        category: {
          primaryCategory,
          subCategory1,
          subCategory2,
          subCategory3,
          unqPageIdentifier,
        },
        userAgent,
      },
      error: store.state.analytics.error,
      action: store.state.analytics.action,
      timestamp: store.state.analytics.timestamp,
      environment: store.$env.ANALYTICS_ENVIRONMENT,
      userType: '',
      user: {
        gpOdsCode: store.state.session.gpOdsCode,
        medicalRecordType: store.state.myRecord.medicalRecordType,
        appointmentDateFilterDropdownValue:
          store.state.availableAppointments.selectedOptions.date,
        gpOnlineProduct: '',
        gpBookingSlot: '',
      },
    };

    if (!routePath.includes(APPOINTMENTS_PATH)) {
      window.digitalData.user.appointmentDateFilterDropdownValue = '';
    }

    if (store.state.availableAppointments.selectedSlot &&
        store.state.availableAppointments.selectedSlot.startTime) {
      window.digitalData.user.gpBookingSlot = moment(store.state.availableAppointments.selectedSlot.startTime).format('dddd | HH:mm:ss');
    }

    if (store.state.myRecord.record && store.state.myRecord.record.supplier) {
      window.digitalData.user.gpOnlineProduct = store.state.myRecord.record.supplier;
    }

    try {
      // eslint-disable-next-line no-underscore-dangle
      if (!routePath.includes(TERMSANDCONDITIONS_PATH)
        && store.state.termsAndConditions.analyticsCookieAccepted
        && window._satellite) {
        window._satellite.track('page_view');
      }
    } catch (ex) {
      // Put track call in try-catch, as it likely called under error, so no internet connection.
      // eslint-disable-next-line no-empty
      try { store.dispatch(); } catch (exception) { }
    }
    return window.digitalData;
  })();

  const $analytics = {
    trackButtonClick: (target) => {
      const action = {
        type: 'page_view',
        senderType: 'button',
        target,
      };
      store.dispatch('analytics/track', action);
    },
    validationError: (messages) => {
      const error = {
        type: 'validation_error',
        messages,
      };
      store.dispatch('analytics/trackError', error);
    },
    logicError: (messages) => {
      const error = {
        type: 'logic_error',
        messages,
      };
      store.dispatch('analytics/trackError', error);
    },
  };

  Object.assign(app, { $analytics });
}
