import each from 'jest-each';
import proofLevel from '@/lib/proofLevel';
import { allRoutes } from '@/router';
import values from 'lodash/fp/values';
import {
  MY_RECORD_NAME,
  NOT_FOUND_NAME,
  CHECKYOURSYMPTOMS_NAME,
} from '@/router/names';

// '' is the base route name for /patient/:patientId routes
// MY_RECORD, SYMPTOMS, CHECKYOURSYMPTOMS and NOT_FOUND are redirect routes
const excludedRouteNames = ['', MY_RECORD_NAME, NOT_FOUND_NAME, CHECKYOURSYMPTOMS_NAME];
const filteredRoutes = allRoutes.reduce((agg, route) => {
  agg.push(route);
  if (route.children) {
    agg.push(...route.children);
  }
  return agg;
}, []).filter(route => !excludedRouteNames.includes(route.name));

function assertRouteExists(routes, route) {
  const names = values(routes).map(x => x.name);
  expect(names).toEqual(
    expect.arrayContaining([route.name]),
  );
  const paths = values(routes).map(x => x.path);
  expect(paths).toEqual(
    expect.arrayContaining([route.path]),
  );
}

function checkForValidProofLevelOrisAnonymous(route, routes) {
  if (route.meta.isAnonymous) {
    expect(route.meta.proofLevel)
      .toBeUndefined();
    expect(route.meta.upliftRoute)
      .toBeUndefined();
  } else {
    switch (route.meta.proofLevel) {
      case proofLevel.P9:
        expect(route.meta.upliftRoute)
          .toBeDefined();
        assertRouteExists(routes, route.meta.upliftRoute);
        break;
      case proofLevel.P5:
        expect(route.meta.upliftRoute)
          .toBeUndefined();
        break;
      default:
        fail(`route must have a valid proof level set to one of (${values(proofLevel)
          .join(', ')})`);
    }
  }
}

describe('routes - proof level usage', () => {
  each(values(filteredRoutes).map(x => [x.name, x]))
    .it('`%s` will have proof level setup correctly or be Anonymous', (_, route) => {
      checkForValidProofLevelOrisAnonymous(route, filteredRoutes);
    });
});
