import AuthReturnLayout from '@/layouts/authReturn';
import i18n from '@/plugins/i18n';
import { AUTH_RETURN_PATH } from '@/router/paths';
import each from 'jest-each';
import { mount, createStore } from '../helpers';


jest.mock('@/lib/utils');

describe('authReturn layout', () => {
  const CONTACT_US_URL = 'https://www.example.com';
  const MY_HEALTH_ONLINE_URL = 'https://111.wales.nhs.uk/contactus/myhealthonline/';
  const NHS_WALES_111_URL = 'https://111.wales.nhs.uk';
  const NHS_111_URL = 'https://111.nhs.uk';
  const serviceDeskReference = 'fooReference';
  let goToUrl;
  let wrapper;

  const mountAuthReturnLayout = ({
    status,
    shallow = false,
    showApiError = true,
    query = {},
  }) => mount(AuthReturnLayout, {
    shallow,
    mocks: {
      correctUrl: jest.fn(),
      goToUrl,
    },
    $store: createStore({
      $env: {
        CONTACT_US_URL,
      },
      getters: {
        'errors/showApiError': showApiError,
      },
      state: {
        appVersion: {
          nativeVersion: true,
        },
        device: { isNativeApp: true },
        errors: {
          pageSettings: { errorOverrideStyles: [] },
          routePath: AUTH_RETURN_PATH,
          apiErrors: [{ status, serviceDeskReference }],
        },
      },
    }),
    $route: {
      query,
    },
    mountOpts: { i18n },
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
      wrapper = mountAuthReturnLayout({ status });
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
          link = container.findAll('a').at(2);
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
          contactUsLink = wrapper.findAll('a').at(1);
        });

        it('will exist', () => {
          expect(contactUsLink.exists()).toBe(true);
        });

        it('url will have error code', () => {
          expect(contactUsLink.attributes('href')).toBe(`${CONTACT_US_URL}?errorcode=${serviceDeskReference}`);
        });
      });

      describe('111 website link', () => {
        let nhs111WebsiteLink;

        beforeEach(() => {
          nhs111WebsiteLink = wrapper.findAll('a').at(0);
        });

        it('will exist', () => {
          expect(nhs111WebsiteLink.exists()).toBe(true);
        });

        it('url will have error code', () => {
          expect(nhs111WebsiteLink.attributes('href')).toBe(NHS_111_URL);
        });
      });
    });
  });

  describe('on error 464', () => {
    beforeEach(() => {
      wrapper = mountAuthReturnLayout({ status: 464 });
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
      wrapper = mountAuthReturnLayout({ status: 465 });
    });

    describe('error container', () => {
      let container;

      beforeEach(() => {
        container = wrapper.find('[data-purpose=error-container]');
      });

      it('will exist', () => {
        expect(container.exists()).toBe(true);
      });

      it('will have one link', () => {
        expect(container.findAll('a').length).toBe(1);
      });

      it('will link to the 111 website', () => {
        const nhs111Link = wrapper.findAll('a').at(0);
        expect(nhs111Link.attributes('href')).toBe(NHS_111_URL);
      });
    });
  });

  describe('metaInfo', () => {
    it('will set language from locale', () => {
      wrapper = mountAuthReturnLayout({ shallow: true });
      const head = wrapper.vm.$options.metaInfo.call(wrapper.vm);
      expect(head.htmlAttrs.lang).toBe('en-GB');
    });

    it('will have no scripts defined', () => {
      wrapper = mountAuthReturnLayout({ shallow: true });
      const head = wrapper.vm.$options.metaInfo.call(wrapper.vm);
      expect(head.script).toBeUndefined();
    });

    describe('title', () => {
      it('will set title to undefined if showError is false', () => {
        wrapper = mountAuthReturnLayout({ shallow: true, showApiError: false });
        const head = wrapper.vm.$options.metaInfo.call(wrapper.vm);
        expect(head.title).toBeUndefined();
      });


      each([
        ['ConsentNotGiven', 'You need to accept NHS login terms of use to continue'],
        ['ExampleErrorDescription', 'Login failed'],
      ])
        .it('will set the correct title if errorDescription is %s',
          (errorDescription, expectedTitle) => {
            const query = { error_description: errorDescription };
            wrapper = mountAuthReturnLayout({ query, shallow: true });

            const head = wrapper.vm.$options.metaInfo.call(wrapper.vm);
            expect(head.title).toBe(`${expectedTitle} - NHS App`);
          });
    });

    describe('nhs login terms not accepted', () => {
      it('has the correct content', () => {
        const query = { error_description: 'ConsentNotGiven' };
        wrapper = mountAuthReturnLayout({ query, shallow: true });

        const termsError = wrapper.find('#termsAndConditionsError');
        const paragraphs = termsError.findAll('p');

        expect(paragraphs.at(0).text()).toBe('You cannot use the NHS app if you have not accepted NHS login terms of use.');
        expect(paragraphs.at(1).text()).toBe('If you need to book an appointment or get a prescription now, contact your GP surgery directly.');
      });

      each([
        ['ConsentNotGiven', true, false],
        ['ExampleErrorDescription', false, true],
      ])
        .it('will show the correct error if the error_description query param is %s',
          (errorDescription, termsAndConditionsErrorVisible, authReturnErrorVisible) => {
            const query = { error_description: errorDescription };
            wrapper = mountAuthReturnLayout({ query, shallow: true });

            expect(wrapper.find('#termsAndConditionsError').exists()).toBe(termsAndConditionsErrorVisible);
            expect(wrapper.find('#authReturnError').exists()).toBe(authReturnErrorVisible);
          });
    });
  });
});
