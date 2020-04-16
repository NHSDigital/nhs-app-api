import each from 'jest-each';
import values from 'lodash/fp/values';
import { routes } from '@/lib/routes';

describe('routes - proof level usage', () => {
  each(values(routes).map(x => [x.name, x]))
    .it('`%s` will have proof level setup correctly', (_, route) => {
      if (route.isAnonymous) {
        expect(route.proofLevel).toBeUndefined();
        expect(route.upliftPath).toBeUndefined();
      } else {
        switch (route.proofLevel) {
          case 9:
            expect(route.upliftPath).toBeDefined();
            break;
          case 5:
            expect(route.upliftPath).toBeUndefined();
            break;
          default:
            fail('route must have a valid proof level set to either 9 or 5');
        }
      }
    });
});
