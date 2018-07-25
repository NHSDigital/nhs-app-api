export default function ({ store, route }) {
  if (process.client) {
    window.digitalData = {};
    window.digitalData = (() => {
      const routePath = route.fullPath;
      const clrpath = routePath.split('?');
      const fldrelmnt = clrpath[0].split('/');

      const pageUrl = window.location.hostname + clrpath[0];
      const { environment } = window;
      const referringUrl = document.referrer;
      const { domain } = document;
      const urlParams = window.location.href;

      const { userAgent } = navigator;

      function getCategory(index) {
        if (fldrelmnt[index] && fldrelmnt[index] !== ' ') {
          return fldrelmnt[index];
        }
        return '';
      }

      const primaryCategory = getCategory(1) || 'home';
      const subCategory1 = getCategory(2) || '';
      const subCategory2 = getCategory(3) || '';
      let pageName = 'bw:en';
      for (let i = 1; i < fldrelmnt.length; i += 1) {
        const pageNamePart = getCategory(i);
        if (pageNamePart) {
          pageName = `${pageName}:${pageNamePart}`;
        }
      }
      if (pageName === 'bw:en') {
        pageName = `${pageName}:home`;
      }

      const dataObject = {
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
      };

      return dataObject;
    })();
  }
}
