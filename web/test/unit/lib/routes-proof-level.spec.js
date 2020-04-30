import each from 'jest-each';
import proofLevel from '@/lib/proofLevel';
import routes from '@/lib/routes';
import values from 'lodash/fp/values';

function assertRouteExists(allRoutes, path) {
  const paths = values(allRoutes).map(x => x.path);
  expect(paths).toEqual(
    expect.arrayContaining([path]),
  );
}

describe('routes - proof level usage', () => {
  each(values(routes).map(x => [x.name, x]))
    .it('`%s` will have proof level setup correctly', (_, route) => {
      if (route.isAnonymous) {
        expect(route.proofLevel).toBeUndefined();
        expect(route.upliftPath).toBeUndefined();
      } else {
        switch (route.proofLevel) {
          case proofLevel.P9:
            expect(route.upliftPath).toBeDefined();
            assertRouteExists(routes, route.upliftPath);
            break;
          case proofLevel.P5:
            expect(route.upliftPath).toBeUndefined();
            break;
          default:
            fail(`route must have a valid proof level set to one of (${values(proofLevel).join(', ')})`);
        }
      }
    });
});

