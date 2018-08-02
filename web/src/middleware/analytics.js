import { isEmpty } from 'lodash/fp';

const APP_ID = 'nhsapp';
const ENGLISH_LANGUAGE = 'en';
const pageNamePrefix = `${APP_ID}:${ENGLISH_LANGUAGE}`;

export default function ({ store, app, route }) {
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

      return {
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
        errors: store.state.errors.apiErrors,
        action: store.state.analytics.action,
        timestamp: store.state.analytics.timestamp,
        environment: '',
        userType: '',
        user: {
          gpOdsCode: store.state.session.gpOdsCode,
        },
      };
    })();
  }
}
