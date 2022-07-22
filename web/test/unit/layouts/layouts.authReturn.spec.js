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
  const SYMPTOM_CHECKER_NORTHERN_IRELAND_URL = 'https://www-nidirect-gov-uk/articles/gp-out-hours-service';
  const COVID_PASS_URL = 'https://www.nhs.uk/conditions/coronavirus-covid-19/covid-pass';
  const COVID_PASS_LETTER_URL = 'http://test/covid-letter';
  const COVID_STATUS_URL = 'http://test/covid-online';
  const serviceDeskReference = 'fooReference';
  let goToUrl;
  let wrapper;

  const mountAuthReturnLayout = ({
    status,
    shallow = false,
    hasConnectionProblem = true,
    query = {},
    skippedLoggedOutPage = false,
  }) => mount(AuthReturnLayout, {
    shallow,
    mocks: {
      correctUrl: jest.fn(),
      goToUrl,
    },
    data: function data() {
      return { hasConnectionProblem };
    },
    $store: createStore({
      $env: {
        CONTACT_US_URL,
        SYMPTOM_CHECKER_URL: 'https://111.nhs.uk',
        SYMPTOM_CHECKER_WALES_URL: 'https://111.wales.nhs.uk',
        MY_HEALTH_ONLINE: 'https://111.wales.nhs.uk/contactus/myhealthonline/',
        SYMPTOM_CHECKER_NORTHERN_IRELAND_URL: 'https://www-nidirect-gov-uk/articles/gp-out-hours-service',
        COVID_PASS_URL: 'https://www.nhs.uk/conditions/coronavirus-covid-19/covid-pass',
        COVID_PASS_LETTER_URL: 'http://test/covid-letter',
        COVID_STATUS_URL: 'http://test/covid-online',
        SKIP_LOGGED_OUT_ENABLED: skippedLoggedOutPage,
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
  ])('on error %i', (status) => {
    beforeEach(() => {
      wrapper = mountAuthReturnLayout({ status });
    });

    describe('shutter screen', () => {
      let screen;

      beforeEach(() => {
        screen = wrapper.find('[data-purpose=shutter-container]');
      });

      it('will exist', () => {
        expect(screen.exists()).toBe(true);
      });

      describe('back to home link', () => {
        let link;

        beforeEach(() => {
          link = screen.findAll('a').at(2);
        });

        it('will exist', () => {
          expect(link.exists()).toBe(true);
        });

        it('will go to /login when clicked', () => {
          link.trigger('click');
          expect(goToUrl).toBeCalledWith('/login');
        });

        it('will retain referrer and redirect_to parameters', () => {
          wrapper = mountAuthReturnLayout({
            status,
            skippedLoggedOutPage: true,
            query: {
              state: 'deep_link?integration_referrer=nhs_uk',
            },
          });
          screen = wrapper.find('[data-purpose=shutter-container]');
          link = screen.findAll('a').at(2);

          link.trigger('click');
          expect(goToUrl).toBeCalledWith('/login?redirect_to=deep_link&referrer=nhs_uk');
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
          expect(contactUsLink.attributes('href'))
            .toBe(`${CONTACT_US_URL}?errorcode=${serviceDeskReference}`);
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

    describe('container', () => {
      it('will have five sub headings', () => {
        expect(wrapper.findAll('h2').length).toBe(4);
      });

      it('will have twelve paragraphs', () => {
        expect(wrapper.findAll('p').length).toBe(12);
      });

      it('will have five links', () => {
        expect(wrapper.findAll('a').length).toBe(5);
      });

      it('will have the title', () => {
        const head = wrapper.vm.$options.metaInfo.call(wrapper.vm);
        expect(head.title).toBe('Cannot access NHS App in your area - NHS App');
      });

      describe('hyperlinks', () => {
        let covidPass;
        let myHealthOnlineLink;
        let nhsWales111Link;
        let niDirectLink;
        let contactUsLink;

        beforeEach(() => {
          covidPass = wrapper.findAll('a').at(0);
          myHealthOnlineLink = wrapper.findAll('a').at(1);
          nhsWales111Link = wrapper.findAll('a').at(2);
          niDirectLink = wrapper.findAll('a').at(3);
          contactUsLink = wrapper.findAll('a').at(4);
        });

        it('will exist', () => {
          expect(covidPass.exists()).toBe(true);
          expect(myHealthOnlineLink.exists()).toBe(true);
          expect(nhsWales111Link.exists()).toBe(true);
          expect(niDirectLink.exists()).toBe(true);
          expect(contactUsLink.exists()).toBe(true);
        });

        it('url href is correct', () => {
          expect(covidPass.attributes('href')).toBe(COVID_PASS_URL);
          expect(myHealthOnlineLink.attributes('href')).toBe(MY_HEALTH_ONLINE_URL);
          expect(nhsWales111Link.attributes('href')).toBe(NHS_WALES_111_URL);
          expect(niDirectLink.attributes('href')).toBe(SYMPTOM_CHECKER_NORTHERN_IRELAND_URL);
          expect(contactUsLink.attributes('href'))
            .toBe(`${CONTACT_US_URL}?errorcode=${serviceDeskReference}&odscode=`);
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

      it('will have a title', () => {
        expect(container.findAll('h1').length).toBe(1);
      });

      it('will have seven paragraphs', () => {
        expect(container.findAll('p').length).toBe(7);
      });

      it('will have three links', () => {
        expect(container.findAll('a').length).toBe(3);
      });

      it('will have a digital covid link', () => {
        const link = wrapper.findAll('a').at(0);
        expect(link.attributes('href')).toBe(COVID_STATUS_URL);
      });

      it('will have a letter covid link', () => {
        const link = wrapper.findAll('a').at(1);
        expect(link.attributes('href')).toBe(COVID_PASS_LETTER_URL);
      });

      it('will link to the 111 website', () => {
        const nhs111Link = wrapper.findAll('a').at(2);
        expect(nhs111Link.attributes('href')).toBe(NHS_111_URL);
      });
    });
  });

  describe('on error 468', () => {
    beforeEach(() => {
      wrapper = mountAuthReturnLayout({ status: 468 });
    });

    describe('shutter screen', () => {
      let screen;

      beforeEach(() => {
        screen = wrapper.find('[data-purpose=shutter-container]');
      });

      it('will exist', () => {
        expect(screen.exists()).toBe(true);
      });

      it('will have seven paragraphs', () => {
        expect(screen.findAll('p').length).toBe(7);
      });

      it('will have three links', () => {
        expect(screen.findAll('a').length).toBe(3);
      });

      describe('hyperlinks', () => {
        let covidPassLink;
        let nhs111Link;
        let contactUsLink;
        beforeEach(() => {
          covidPassLink = wrapper.findAll('a').at(0);
          nhs111Link = wrapper.findAll('a').at(1);
          contactUsLink = wrapper.findAll('a').at(2);
        });

        it('will exist', () => {
          expect(nhs111Link.exists()).toBe(true);
          expect(covidPassLink.exists()).toBe(true);
          expect(contactUsLink.exists()).toBe(true);
        });

        it('url href is correct', () => {
          expect(covidPassLink.attributes('href')).toBe(COVID_PASS_URL);
          expect(nhs111Link.attributes('href')).toBe(NHS_111_URL);
          expect(contactUsLink.attributes('href'))
            .toBe(`${CONTACT_US_URL}?errorcode=${serviceDeskReference}&odscode=`);
        });
      });
    });
  });

  describe('on error 469', () => {
    beforeEach(() => {
      wrapper = mountAuthReturnLayout({ status: 469 });
    });

    describe('shutter screen', () => {
      let screen;

      beforeEach(() => {
        screen = wrapper.find('[data-purpose=shutter-container]');
      });

      it('will exist', () => {
        expect(screen.exists()).toBe(true);
      });

      it('will have four paragraphs', () => {
        expect(screen.findAll('p').length).toBe(4);
      });

      it('will have two links', () => {
        expect(screen.findAll('a').length).toBe(2);
      });

      describe('hyperlinks', () => {
        let contactUsLink;
        let nhs111Link;
        beforeEach(() => {
          nhs111Link = wrapper.findAll('a').at(0);
          contactUsLink = wrapper.findAll('a').at(1);
        });

        it('will exist', () => {
          expect(nhs111Link.attributes('href')).toBe(NHS_111_URL);
          expect(contactUsLink.exists()).toBe(true);
        });

        it('url href is correct', () => {
          expect(contactUsLink.attributes('href'))
            .toBe(`${CONTACT_US_URL}?errorcode=${serviceDeskReference}&odscode=`);
        });
      });
    });
  });

  describe('on error 999', () => {
    beforeEach(() => {
      wrapper = mountAuthReturnLayout({ status: 999 });
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

        it('will retain referrer and redirect_to parameters', () => {
          wrapper = mountAuthReturnLayout({
            status: 999,
            skippedLoggedOutPage: true,
            query: {
              state: 'deep_link?integration_referrer=nhs_uk',
            },
          });
          container = wrapper.find('[data-purpose=error-container]');
          link = container.findAll('a').at(2);

          link.trigger('click');
          expect(goToUrl).toBeCalledWith('/login?redirect_to=deep_link&referrer=nhs_uk');
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
          expect(contactUsLink.attributes('href'))
            .toBe(`${CONTACT_US_URL}?errorcode=${serviceDeskReference}`);
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
      it('will set title to undefined if hasConnectionProblem is false', () => {
        wrapper = mountAuthReturnLayout({ shallow: true, hasConnectionProblem: false });
        const head = wrapper.vm.$options.metaInfo.call(wrapper.vm);
        expect(head.title).toBeUndefined();
      });

      it('will set title to Cannot login... if under 13', () => {
        wrapper = mountAuthReturnLayout({ status: 465, shallow: true });
        const head = wrapper.vm.$options.metaInfo.call(wrapper.vm);
        expect(head.title).toBe('Cannot log in due to age restriction - NHS App');
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

    describe.each([
      [400, 'Cannot log in'],
      [468, 'Cannot log in'],
      [500, 'The service is unavailable'],
      [403, 'Cannot log in'],
      [469, 'Cannot log in'],
      [502, 'Cannot log in'],
      [504, 'Cannot log in'],
      [464, 'Cannot access NHS App in your area'],
    ])('on error %i the heading content of h1 should be %s', (status, headingTitle) => {
      beforeEach(() => {
        wrapper = mountAuthReturnLayout({ status });
      });
      describe('will check the heading h1 content', () => {
        let screen;
        beforeEach(() => {
          screen = wrapper.find('[data-purpose=shutter]');
        });

        it('will check the heading content', () => {
          const heading1 = screen.find('h1');
          expect(heading1.text()).toContain(headingTitle);
        });
      });
    });
  });
});
