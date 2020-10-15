/* eslint-disable global-require, import/no-dynamic-require, no-param-reassign */
import get from 'lodash/fp/get';
import isArray from 'lodash/fp/isArray';
import isEmpty from 'lodash/fp/isEmpty';
import keys from 'lodash/fp/keys';
import map from 'lodash/fp/map';
import glob from 'glob';
import { CHECKYOURSYMPTOMS_NAME } from '@/router/names';
import { allRoutes } from '@/router';

// CHECKYOURSYMPTOMS are used as a redirect so will have no meta
const excludedRouteNames = [
  CHECKYOURSYMPTOMS_NAME,
];

const importRoute = (path) => {
  try {
    const routes = get('default', require(path));
    return { routes, path };
  } catch (error) {
    return { error, path };
  }
};

const processResults = (results) => {
  const noRoutes = 'No error but routes did not load';
  const routesNotArray = 'No error but routes did not export default array';
  const missingCrumb = 'Missing meta crumb';
  const missingHelpUrl = 'Missing help url';

  return results.reduce((agg, result) => {
    if (result.error) {
      agg[result.error] = agg[result.error] || [];
      agg[result.error].push(result.path);
      return agg;
    }

    if (!result.routes) {
      agg[noRoutes] = agg[noRoutes] || [];
      agg[noRoutes].push(result.path);
      return agg;
    }

    if (!isArray(result.routes)) {
      agg[routesNotArray] = agg[routesNotArray] || [];
      agg[routesNotArray].push(result.path);
      return agg;
    }

    result.routes.filter(route => !excludedRouteNames.includes(route.name)).forEach((route) => {
      if (!get('meta.crumb', route)) {
        agg[missingCrumb] = agg[missingCrumb] || [];
        agg[missingCrumb].push(`${result.path}: Route name: ${get('name', route)}`);
      }
      if (!get('meta.helpUrl', route)) {
        agg[missingHelpUrl] = agg[missingHelpUrl] || [];
        agg[missingHelpUrl].push(`${result.path}: Route name: ${get('name', route)}`);
      }
    });

    return agg;
  }, {});
};

const createFailureMessage = (failures) => {
  let messages = [];
  messages.push('Analysis of routes configuration failed: ');
  messages.push('  In the list below, the following errors can be fixed like so:');
  messages.push('');
  messages.push('  No error but routes did not export default array:');
  messages.push('    In the relevant file under @/router/routes ensure you have exported a ');
  messages.push('    default array containing the routes from this module:');
  messages.push('      export default [');
  messages.push('        // Individual route objects go here');
  messages.push('        INDEX, APPOINTMENTS, // ...');
  messages.push('      ];');
  messages.push('');
  messages.push('  Missing meta crumb | Missing help url:');
  messages.push('    In the individual route you have defined, ensure there is a meta object with a crumb object, and help url:');
  messages.push('      const GP_APPOINTMENTS = {');
  messages.push('        name: \'appointments-gp-appointments\',');
  messages.push('        // ... ,');
  messages.push('        meta: {');
  messages.push('          crumb: breadcrumbs.GP_APPOINTMENTS_CRUMB,');
  messages.push('          helpUrl: baseNhsAppHelpUrl,');
  messages.push('        },');
  messages.push('      }');
  messages.push('');
  messages.push('    If there is no crumb required for the route, default it to {}:');
  messages.push('      const GP_APPOINTMENTS = {');
  messages.push('        name: \'appointments-gp-appointments\',');
  messages.push('        // ... ,');
  messages.push('        meta: {');
  messages.push('          crumb: {},');
  messages.push('        },');
  messages.push('      }');
  messages.push('');
  messages.push('  Failures:');

  messages = [
    ...messages,
    ...map(key => `  ${key}:\n    ${failures[key].join('\n    ')}\n`)(keys(failures)),
  ];

  return messages.join('\n');
};

describe('verify routes', () => {
  it('will have a meta and crumb defined for each route', (done) => {
    glob('src/router/routes/**/*.js', (err, files) => {
      const paths = map(file => `../${file}`, files);
      const results = map(importRoute)(paths);
      const failures = processResults(results);
      if (!isEmpty(failures)) {
        done.fail(createFailureMessage(failures));
      }

      done();
    });
  });

  describe('checkEachRouteHasValidHelpUrlAttribute', () => {
    it('should have a valid helpUrl attribute', (done) => {
      const routes = allRoutes.find(x => x.name === '').children;
      const expression = /https?:\/\/(www\.)?[-a-zA-Z0-9@:%._+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_+.~#?&//=]*)?/gi;
      const regex = new RegExp(expression);

      expect(routes.length).toBeTruthy();
      routes.forEach((route) => {
        if (route.meta) {
          if (route.meta.helpUrl === undefined ||
            route.meta.helpUrl === null ||
            route.meta.helpUrl === '' ||
            !route.meta.helpUrl.match(regex)) {
            done.fail(`${route.name} needs a valid populated helpUrl attribute.`);
          }
        }
      });
      done();
    });
  });
});
