import { allRoutes } from '@/router';

describe('Each route name', () => {
  it('should be unique.', (done) => {
    const routeNames = Object.keys(allRoutes).map(key => allRoutes[key].name);
    const nestedRoutes = allRoutes.find(x => x.name === '').children;
    const nestedRouteNames = Object.keys(nestedRoutes).map(key => nestedRoutes[key].name);

    expect(routeNames.length).toBeTruthy();
    expect(nestedRouteNames.length).toBeTruthy();
    const allRoutesNames = [...routeNames, ...nestedRouteNames];
    const foundMap = {};
    allRoutesNames.forEach((name) => {
      if (foundMap[name]) {
        done.fail(`[${name}] is duplicated and should be unique.`);
      } else {
        foundMap[name] = true;
      }
    });
    done();
  });
});
