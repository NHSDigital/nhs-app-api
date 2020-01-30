import Redirector from '@/pages/redirector/index';
import { APPOINTMENTS, INDEX, INTERSTITIAL_REDIRECTOR, REDIRECT_PARAMETER } from '@/lib/routes';
import { createRouter, mount } from '../../helpers';

describe('redirector page', () => {
  let $route;
  let $router;

  const mountRedirector = () => mount(Redirector, { $router, $route });

  beforeEach(() => {
    $router = createRouter();
  });

  describe('has no redirect param', () => {
    beforeEach(() => {
      $route = {
        ...INTERSTITIAL_REDIRECTOR,
        query: { },
      };
      mountRedirector();
    });

    it('will call router push with INDEX path', () => {
      expect($router.push).toHaveBeenCalledWith(INDEX.path);
    });
  });

  describe('has redirect param for an internal route', () => {
    beforeEach(() => {
      $route = {
        ...INTERSTITIAL_REDIRECTOR,
        query: { [REDIRECT_PARAMETER]: APPOINTMENTS.name },
      };
      mountRedirector();
    });

    it('will call router push with internal route path', () => {
      expect($router.push).toHaveBeenCalledWith(APPOINTMENTS.path);
    });
  });

  describe('has redirect param to redirector route', () => {
    beforeEach(() => {
      $route = {
        ...INTERSTITIAL_REDIRECTOR,
        query: { [REDIRECT_PARAMETER]: INTERSTITIAL_REDIRECTOR.name },
      };
      mountRedirector();
    });

    it('will call router push with INDEX path', () => {
      expect($router.push).toHaveBeenCalledWith(INDEX.path);
    });
  });

  describe('has redirect param external site', () => {
    beforeEach(() => {
      $route = {
        ...INTERSTITIAL_REDIRECTOR,
        query: { [REDIRECT_PARAMETER]: 'http://www.google.com' },
      };
      mountRedirector();
    });

    it('will call router push with INDEX path', () => {
      expect($router.push).toHaveBeenCalledWith(INDEX.path);
    });
  });

  describe('has redirect param not a url', () => {
    beforeEach(() => {
      $route = {
        ...INTERSTITIAL_REDIRECTOR,
        query: { [REDIRECT_PARAMETER]: 'something else' },
      };
      mountRedirector();
    });

    it('will call router push with INDEX path', () => {
      expect($router.push).toHaveBeenCalledWith(INDEX.path);
    });
  });
});
