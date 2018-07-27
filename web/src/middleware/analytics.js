import { isEmpty } from 'lodash/fp';

const APP_ID = 'nhsapp';
const ENGLISH_LANGUAGE = 'en';
const pageNamePrefix = `${APP_ID}:${ENGLISH_LANGUAGE}`;

export default function ({ store, route }) {
  if (process.client) {
    window.digitalData = {};
    window.digitalData = (() => {
      const routePath = route.fullPath;
      const [path] = routePath.split('?');
      const fields = path.split('/').slice(1);
      const pageUrl = window.location.hostname + path;
      const { environment } = window;
      const referringUrl = document.referrer;
      const { domain } = document;
      const urlParams = window.location.href;
      const { userAgent } = navigator;
      const primaryCategory = fields[0] || 'home';
      const subCategory1 = fields[1] || '';
      const subCategory2 = fields[2] || '';

      if (isEmpty(fields)) fields.push('home');
      const pageName = fields.reduce((combined, field) => `${combined}:${field}`, pageNamePrefix);

      return {
        page: {
          pageInfo: {
            pageName,
            destinationURL: pageUrl,
            pageInstanceID: environment,
            referringURL: referringUrl,
            environment: domain,
            urlParams,
          },
          category: {
            primaryCategory,
            subCategory1,
            subCategory2,
          },
          userAgent,
        },
        errors: store.state.errors.apiErrors,
        action: store.state.analytics.action,
        timestamp: store.state.analytics.timestamp,
      };
    })();
  }
}
