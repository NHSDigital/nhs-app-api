import * as dependency from '@/lib/utils';
import WayfinderPage from '@/pages/wayfinder/index';
import { createStore, mount } from '../../helpers';

jest.mock('@/lib/utils');

const setupStore = (hasReferralsInReview,
  hasReferralsNotInReview,
  hasConfirmedAppointments,
  hasUnconfirmedAppointments,
  isNativeApp,
  hasGetters) => {
  let referralsNotInReviewArray = [];
  let referralsInReviewArray = [];
  let confirmedAppointmentsArray = [];
  let unconfirmedAppointmentsArray = [];
  let getters;

  if (hasReferralsNotInReview) {
    referralsNotInReviewArray = [{
      bookingReference: '608119956621',
      deepLinkUrl: '',
      referrerOrganisation: 'Mahogany GP Surgery',
      referredDateTime: '2022-04-10T10:00:00',
      requestedSpecialty: 'Cardiology',
      referralId: '4',
    }];
  }

  if (hasReferralsInReview) {
    referralsInReviewArray = [{
      bookingReference: '608119956620',
      deepLinkUrl: '',
      referrerOrganisation: 'Mahogany GP Surgery',
      referredDateTime: '2022-04-10T10:00:00',
      reviewDate: '2022-04-18T10:00:00',
      requestedSpecialty: 'Cardiology',
      referralId: '3' }];
  }

  if (hasConfirmedAppointments) {
    confirmedAppointmentsArray = [{
      appointmentDateTime: '2022-04-18T10:00:00',
      appointmentId: '1',
      deepLinkUrl: '',
      locationDescription: 'A Clinic, A Town, A Country',
    }];
  }

  if (hasUnconfirmedAppointments) {
    unconfirmedAppointmentsArray = [{
      appointmentId: '2',
      deepLinkUrl: '',
      locationDescription: 'A Clinic, A Town, A Country',
    }];
  }

  if (hasGetters) {
    getters = { 'knownServices/matchOneById': id => ({
      id,
      url: 'www.url.com',
    }),
    'serviceJourneyRules/cdssAdminEnabled': false,
    'serviceJourneyRules/silverIntegrationAppointmentsEnabled': false,
    'serviceJourneyRules/silverIntegrationEnabled': () => (false),
    'session/isProxying': false,
    };
  }

  return createStore({
    state: {
      device: {
        isNativeApp,
      },
      wayfinder: {
        summary: {
          referralsNotInReview: referralsNotInReviewArray,
          referralsInReview: referralsInReviewArray,
          confirmedAppointments: confirmedAppointmentsArray,
          unconfirmedAppointments: unconfirmedAppointmentsArray,
        },
        apiError: undefined,
        hasLoaded: true,
      },
    },
    getters,
  });
};

describe('Summary care response with: ' +
  '1 referral in review,' +
  '1 referral not in review,' +
  '1 confirmed appointment' +
  'and 1 unconfirmed appointment', () => {
  let $store;
  let wrapper;
  let bookOrManageReferralsAndAppointmentsTitle;
  let confirmedAppointmentsTitle;
  let referralsInReviewTitle;
  let noConfirmedAppointmentsText;
  let noReferralsInReviewText;

  const createIndexPage = () => mount(WayfinderPage, {
    $store,
  });

  beforeEach(() => {
    $store = setupStore(true, true, true, true, true, false);
  });

  describe('if page loaded', () => {
    it('title text is shown for book or manage appointments section', () => {
      wrapper = createIndexPage();
      bookOrManageReferralsAndAppointmentsTitle = wrapper.find('#book-Or-Manage-Referrals-And-Appointments-Title');

      expect(bookOrManageReferralsAndAppointmentsTitle.exists()).toBe(true);
    });

    it('title text is shown for confirmed appointments section', () => {
      wrapper = createIndexPage();
      confirmedAppointmentsTitle = wrapper.find('#confirmed-appointments-title');

      expect(confirmedAppointmentsTitle.exists()).toBe(true);
    });

    it('title text is shown for referrals in review section', () => {
      wrapper = createIndexPage();
      referralsInReviewTitle = wrapper.find('#referrals-in-review-title');

      expect(referralsInReviewTitle.exists()).toBe(true);
    });
  });

  describe('if there are confirmed appointments', () => {
    it('hide resulting empty confirmed appointments text', () => {
      wrapper = createIndexPage();
      noConfirmedAppointmentsText = wrapper.find('#no-confirmed-appointments-text');

      expect(noConfirmedAppointmentsText.exists()).toBe(false);
    });
  });

  describe('if there are referrals in review', () => {
    it('hide resulting empty no referrals in review text', () => {
      wrapper = createIndexPage();
      noReferralsInReviewText = wrapper.find('#no-referrals-or-appointments-text');

      expect(noReferralsInReviewText.exists()).toBe(false);
    });
  });
});

describe('Summary care response with: ' +
  '0 referrals in review,' +
  '1 referral not in review,' +
  '0 confirmed appointments' +
  'and 1 unconfirmed appointment', () => {
  let $store;
  let wrapper;
  let bookOrManageReferralsAndAppointmentsTitle;
  let confirmedAppointmentsTitle;
  let referralsInReviewTitle;
  let noConfirmedAppointmentsText;
  let noReferralsInReviewText;

  const createIndexPage = () => mount(WayfinderPage, {
    $store,
  });

  beforeEach(() => {
    $store = setupStore(false, true, false, true, true, false);
  });

  describe('if page loaded', () => {
    it('title text is shown for book or manage appointments section', () => {
      wrapper = createIndexPage();
      bookOrManageReferralsAndAppointmentsTitle = wrapper.find('#book-Or-Manage-Referrals-And-Appointments-Title');

      expect(bookOrManageReferralsAndAppointmentsTitle.exists()).toBe(true);
    });

    it('title text is shown for confirmed appointments section', () => {
      wrapper = createIndexPage();
      confirmedAppointmentsTitle = wrapper.find('#confirmed-appointments-title');

      expect(confirmedAppointmentsTitle.exists()).toBe(true);
    });

    it('title text is shown for referrals in review section', () => {
      wrapper = createIndexPage();
      referralsInReviewTitle = wrapper.find('#referrals-in-review-title');

      expect(referralsInReviewTitle.exists()).toBe(true);
    });
  });

  describe('if there are no confirmed appointments', () => {
    it('show resulting empty confirmed appointments text', () => {
      wrapper = createIndexPage();
      noConfirmedAppointmentsText = wrapper.find('#no-confirmed-appointments-text');

      expect(noConfirmedAppointmentsText.exists()).toBe(true);
    });
  });

  describe('if there are no referrals in review', () => {
    it('show resulting empty no referrals in review text', () => {
      wrapper = createIndexPage();
      noReferralsInReviewText = wrapper.find('#no-referrals-or-appointments-text');

      expect(noReferralsInReviewText.exists()).toBe(true);
    });
  });
});

describe('Summary care response with: ' +
  '0 referrals in review,' +
  '0 referrals not in review,' +
  '0 confirmed appointments' +
  'and 0 unconfirmed appointments', () => {
  let $store;
  let wrapper;
  let youMayHaveOtherReferralsText;
  let contactTheOrganisationText;
  let contactHealthcareProviderText;
  let otherReferralsAppointmentsAndServicesHeader;
  let otherAvailableServicesMenuItems;
  let backButton;

  const createIndexPage = () => mount(WayfinderPage, {
    $store,
  });

  beforeEach(() => {
    $store = setupStore(false, false, false, false, false, true);
    dependency.redirectTo = jest.fn();
  });

  describe('if page loaded', () => {
    it('show you may have other referrals text', () => {
      wrapper = createIndexPage();
      youMayHaveOtherReferralsText = wrapper.find('#you-may-have-other-referrals-text');

      expect(youMayHaveOtherReferralsText.exists()).toBe(true);
    });

    it('show contact the organisation text', () => {
      wrapper = createIndexPage();
      contactTheOrganisationText = wrapper.find('#contact-the-organisation-text');

      expect(contactTheOrganisationText.exists()).toBe(true);
    });

    it('show contact health provider text', () => {
      wrapper = createIndexPage();
      contactHealthcareProviderText = wrapper.find('#contact-the-healthcare-provider-text');

      expect(contactHealthcareProviderText.exists()).toBe(true);
    });

    it('show other referrals appointments and services header', () => {
      wrapper = createIndexPage();
      otherReferralsAppointmentsAndServicesHeader = wrapper.find('#other-referrals-appointments-and-services-header');

      expect(otherReferralsAppointmentsAndServicesHeader.exists()).toBe(true);
    });

    it('show other available services menu items', () => {
      wrapper = createIndexPage();
      otherAvailableServicesMenuItems = wrapper.find('#other-available-services-menu-items');

      expect(otherAvailableServicesMenuItems.exists()).toBe(true);
    });

    it('show back button', () => {
      wrapper = createIndexPage();
      backButton = wrapper.find('#desktopBackLink');

      expect(backButton.exists()).toBe(true);
    });
  });
});

