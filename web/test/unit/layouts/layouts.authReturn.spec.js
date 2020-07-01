import AuthReturnLayout from '@/layouts/authReturn';
import { mount } from '../helpers';

describe('authReturn layout', () => {
  const CONTACT_US_URL = 'https://www.example.com';
  const MY_HEALTH_ONLINE_URL = 'https://www.myhealthonline-inps2.wales.nhs.uk';
  const NHS_WALES_111_URL = 'https://111.wales.nhs.uk';
  const NHS_111_URL = 'https://111.nhs.uk';
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

      it('will have four sub headings', () => {
        expect(container.findAll('h2').length).toBe(4);
      });

      it('will have eight paragraphs', () => {
        expect(container.findAll('p').length).toBe(8);
      });

      it('will have four links', () => {
        expect(container.findAll('a').length).toBe(4);
      });

      describe('hyperlinks', () => {
        let myHealthOnlineLink;
        let nhsWales111Link;
        let nhs111Link;
        let contactUsLink;

        beforeEach(() => {
          myHealthOnlineLink = wrapper.findAll('a').at(0);
          nhsWales111Link = wrapper.findAll('a').at(1);
          nhs111Link = wrapper.findAll('a').at(2);
          contactUsLink = wrapper.findAll('a').at(3);
        });

        it('will exist', () => {
          expect(myHealthOnlineLink.exists()).toBe(true);
          expect(nhsWales111Link.exists()).toBe(true);
          expect(nhs111Link.exists()).toBe(true);
          expect(contactUsLink.exists()).toBe(true);
        });

        it('url href is correct', () => {
          expect(myHealthOnlineLink.attributes('href')).toBe(MY_HEALTH_ONLINE_URL);
          expect(nhsWales111Link.attributes('href')).toBe(NHS_WALES_111_URL);
          expect(nhs111Link.attributes('href')).toBe(NHS_111_URL);
          expect(contactUsLink.attributes('href'))
            .toBe(`${CONTACT_US_URL}?errorcode=${serviceDeskReference}`);
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
