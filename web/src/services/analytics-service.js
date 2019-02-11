/* eslint-disable no-underscore-dangle */
import { isEmpty } from 'lodash/fp';

const APP_ID = 'nhs:app';
const pageNamePrefix = `${APP_ID}`;


export default function (app, store, route) {
  /* eslint-disable-next-line no-param-reassign */
  if (process.client) {
    window.digitalData = {};
    window.digitalData = (() => {
      const routePath = route.fullPath;
      const [path] = routePath.split('?');
      const fields = path.split('/').slice(1);
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
            referrer: app.router.currentRoute.path,
            referringURL: window.location.hostname,
            environment: domain,
            urlParams: window.location.origin + app.router.currentRoute.fullPath,
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
        environment: app.$env.ANALYTICS_ENVIRONMENT,
        userType: '',
        user: {
          gpOdsCode: store.state.session.gpOdsCode,
          medicalRecordType: store.state.myRecord.medicalRecordType,
          appointmentDateFilterDropdownValue:
            store.state.availableAppointments.selectedOptions.date,
        },
      };
      try {
        // eslint-disable-next-line no-underscore-dangle
        window._satellite.track('page_view');
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
}
