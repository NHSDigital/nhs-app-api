import AuthReturnLayout from '@/layouts/authReturn';
import { mount } from '../helpers';

describe('authReturn layout', () => {
  const CONTACT_US_URL = 'https://www.example.com';
  const serviceDeskReference = 'fooReference';
  let goToUrl;
  let wrapper;

  const mountAuthReturnLayout = status => mount(AuthReturnLayout, {
    $env: {
      CONTACT_US_URL,
    },
    mocks: {
      correctUrl: jest.fn(),
      goToUrl,
    },
    state: {
      device: {
        isNativeApp: true,
      },
      errors: {
        pageSettings: { errorOverrideStyles: [] },
        routePath: '/auth-return',
        apiErrors: [{ status, serviceDeskReference }],
      },
    },
  });

  beforeEach(() => {
    goToUrl = jest.fn();
  });

  describe.each([
    400,
    403,
    502,
    504,
    999,
  ])('on error %i', (status) => {
    beforeEach(() => {
      wrapper = mountAuthReturnLayout(status);
    });

    describe('error container', () => {
      let container;

      beforeEach(() => {
        container = wrapper.find('[data-purpose=error-container]');
      });

      it('will exist', () => {
        expect(container.exists()).toBe(true);
      });

      describe('back to home link', () => {
        let link;

        beforeEach(() => {
          link = container.findAll('a').at(1);
        });

        it('will exist', () => {
          expect(link.exists()).toBe(true);
        });

        it('will go to /login when clicked', () => {
          link.trigger('click');
          expect(goToUrl).toBeCalledWith('/login');
        });
      });

      describe('contact us link', () => {
        let contactUsLink;

        beforeEach(() => {
          contactUsLink = wrapper.find('a');
        });

        it('will exist', () => {
          expect(contactUsLink.exists()).toBe(true);
        });

        it('url will have error code', () => {
          expect(contactUsLink.attributes('href')).toBe(`${CONTACT_US_URL}?errorcode=${serviceDeskReference}`);
        });
      });
    });
  });

  describe('on error 464', () => {
    beforeEach(() => {
      wrapper = mountAuthReturnLayout(464);
    });

    describe('error container', () => {
      let container;

      beforeEach(() => {
        container = wrapper.find('[data-purpose=error-container]');
      });

      it('will exist', () => {
        expect(container.exists()).toBe(true);
      });

      it('will have one link only', () => {
        expect(container.findAll('a').length).toBe(1);
      });

      describe('contact us link', () => {
        let contactUsLink;

        beforeEach(() => {
          contactUsLink = wrapper.find('a');
        });

        it('will exist', () => {
          expect(contactUsLink.exists()).toBe(true);
        });

        it('url will have error code', () => {
          expect(contactUsLink.attributes('href')).toBe(`${CONTACT_US_URL}?errorcode=${serviceDeskReference}`);
        });
      });
    });
  });

  describe('on error 465', () => {
    beforeEach(() => {
      wrapper = mountAuthReturnLayout(465);
    });

    describe('error container', () => {
      let container;

      beforeEach(() => {
        container = wrapper.find('[data-purpose=error-container]');
      });

      it('will exist', () => {
        expect(container.exists()).toBe(true);
      });

      it('will not have any links', () => {
        expect(container.find('a').exists()).toBe(false);
      });
    });
  });
});
