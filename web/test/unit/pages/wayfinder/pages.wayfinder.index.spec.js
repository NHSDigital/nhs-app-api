import * as dependency from '@/lib/utils';
import WayfinderPage from '@/pages/wayfinder/index';
import { createStore, mount } from '../../helpers';

jest.mock('@/lib/utils');

const setupStore = (
  hasReferralsInReview,
  hasReferralsNotInReview,
  hasConfirmedAppointments,
  hasUnconfirmedAppointments,
  isNativeApp,
  hasApiError,
) => {
  let referralsNotInReviewArray = [];
  let referralsInReviewArray = [];
  let confirmedAppointmentsArray = [];
  let unconfirmedAppointmentsArray = [];
  let apiError = null;

  if (hasReferralsNotInReview) {
    referralsNotInReviewArray = [{
      deepLinkUrl: '',
      referrerOrganisation: 'Mahogany GP Surgery',
      referredDateTime: '2022-04-10T10:00:00',
      requestedSpecialty: 'Cardiology',
      referralId: '4',
    }];
  }

  if (hasReferralsInReview) {
    referralsInReviewArray = [{
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

  if (hasApiError) {
    apiError = {
      status: 500,
      serviceDeskReference: '521211',
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
        apiError,
        hasLoaded: true,
      },
    },
    getters: {
      'knownServices/matchOneById': id => ({ id, url: 'www.url.com' }),
      'serviceJourneyRules/cdssAdminEnabled': false,
      'serviceJourneyRules/silverIntegrationAppointmentsEnabled': false,
      'serviceJourneyRules/silverIntegrationEnabled': () => true,
      'session/isProxying': false,
    },
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
  let ReferralsOrAppointmentsLink;
  let ConfirmedAppointmentsLink;
  let ReferralsInReviewLink;
  let otherAvailableServicesMenuItems;
  let ersJumpOffMenuItem;

  const createIndexPage = () => mount(WayfinderPage, {
    $store,
  });

  beforeEach(() => {
    $store = setupStore(true, true, true, true, true);
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

  it('Missing or incorrect referrals or appointments link is visible', () => {
    wrapper = createIndexPage();
    ReferralsOrAppointmentsLink = wrapper.find('#btn_missingOrIncorrectReferralsOrAppointments');

    expect(ReferralsOrAppointmentsLink.exists()).toBe(true);
    expect(ReferralsOrAppointmentsLink.text())
      .toEqual('Missing or incorrect referrals or appointments');
  });

  it('Missing or incorrect confirmed appointments link is visible', () => {
    wrapper = createIndexPage();
    ConfirmedAppointmentsLink = wrapper.find('#btn_missingOrIncorrectConfirmedAppointments');

    expect(ConfirmedAppointmentsLink.exists()).toBe(true);
    expect(ConfirmedAppointmentsLink.text())
      .toEqual('Missing or incorrect confirmed appointments');
  });

  it('Missing or incorrect referrals in review link is visible', () => {
    wrapper = createIndexPage();
    ReferralsInReviewLink = wrapper.find('#btn_missingOrIncorrectReferralsInReview');

    expect(ReferralsInReviewLink.exists()).toBe(true);
    expect(ReferralsInReviewLink.text())
      .toEqual('Missing or incorrect referrals in review');
  });

  it('show other available services menu items', () => {
    wrapper = createIndexPage();
    otherAvailableServicesMenuItems = wrapper.find('#other-available-services-menu-items');
    ersJumpOffMenuItem = otherAvailableServicesMenuItems.find('#btn_manage_your_referral');

    expect(ersJumpOffMenuItem.exists()).toBe(false);
    expect(otherAvailableServicesMenuItems.exists()).toBe(true);
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
      noReferralsInReviewText = wrapper.find('#no-referrals-in-review-text');

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
  let ReferralsOrAppointmentsLink;
  let ConfirmedAppointmentsLink;
  let ReferralsInReviewLink;
  let otherAvailableServicesMenuItems;
  let ersJumpOffMenuItem;

  const createIndexPage = () => mount(WayfinderPage, {
    $store,
  });

  beforeEach(() => {
    $store = setupStore(false, true, false, true, true);
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

  it('Missing or incorrect referrals or appointments link is visible', () => {
    wrapper = createIndexPage();
    ReferralsOrAppointmentsLink = wrapper.find('#btn_missingOrIncorrectReferralsOrAppointments');

    expect(ReferralsOrAppointmentsLink.exists()).toBe(true);
    expect(ReferralsOrAppointmentsLink.text())
      .toEqual('Missing or incorrect referrals or appointments');
  });

  it('Missing or incorrect confirmed appointments link is visible', () => {
    wrapper = createIndexPage();
    ConfirmedAppointmentsLink = wrapper.find('#btn_missingOrIncorrectConfirmedAppointments');

    expect(ConfirmedAppointmentsLink.exists()).toBe(true);
    expect(ConfirmedAppointmentsLink.text())
      .toEqual('Missing or incorrect confirmed appointments');
  });

  it('show other available services menu items', () => {
    wrapper = createIndexPage();
    otherAvailableServicesMenuItems = wrapper.find('#other-available-services-menu-items');
    ersJumpOffMenuItem = otherAvailableServicesMenuItems.find('#btn_manage_your_referral');

    expect(ersJumpOffMenuItem.exists()).toBe(false);
    expect(otherAvailableServicesMenuItems.exists()).toBe(true);
  });

  it('Missing or incorrect referrals in review link is visible', () => {
    wrapper = createIndexPage();
    ReferralsInReviewLink = wrapper.find('#btn_missingOrIncorrectReferralsInReview');

    expect(ReferralsInReviewLink.exists()).toBe(true);
    expect(ReferralsInReviewLink.text())
      .toEqual('Missing or incorrect referrals in review');
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
      noReferralsInReviewText = wrapper.find('#no-referrals-in-review-text');

      expect(noReferralsInReviewText.exists()).toBe(true);
      expect(noReferralsInReviewText.text()).toBe('You have no referrals being reviewed.');
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
  let noReferralsOrAppointmentsText;
  let noConfirmedAppointmentsText;
  let noReferralsInReviewText;
  let backButton;
  let ReferralsOrAppointmentsLink;
  let ConfirmedAppointmentsLink;
  let ReferralsInReviewLink;
  let otherAvailableServicesMenuItems;
  let ersJumpOffMenuItem;

  const createIndexPage = () => mount(WayfinderPage, {
    $store,
  });

  beforeEach(() => {
    $store = setupStore(false, false, false, false, false);
    dependency.redirectTo = jest.fn();
  });

  describe('if page loaded', () => {
    it('show no referrals or appointments text', () => {
      wrapper = createIndexPage();
      noReferralsOrAppointmentsText = wrapper.find('#no-referrals-or-appointments-text');

      expect(noReferralsOrAppointmentsText.exists()).toBe(true);
      expect(noReferralsOrAppointmentsText.text()).toBe('You have no referrals or appointments to view or manage.');
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
        noReferralsInReviewText = wrapper.find('#no-referrals-in-review-text');

        expect(noReferralsInReviewText.exists()).toBe(true);
        expect(noReferralsInReviewText.text()).toBe('You have no referrals being reviewed.');
      });
    });

    it('Missing or incorrect referrals or appointments link is visible', () => {
      wrapper = createIndexPage();
      ReferralsOrAppointmentsLink = wrapper.find('#btn_missingOrIncorrectReferralsOrAppointments');

      expect(ReferralsOrAppointmentsLink.exists()).toBe(true);
      expect(ReferralsOrAppointmentsLink.text())
        .toEqual('Missing or incorrect referrals or appointments');
    });

    it('Missing or incorrect confirmed appointments link is visible', () => {
      wrapper = createIndexPage();
      ConfirmedAppointmentsLink = wrapper.find('#btn_missingOrIncorrectConfirmedAppointments');

      expect(ConfirmedAppointmentsLink.exists()).toBe(true);
      expect(ConfirmedAppointmentsLink.text())
        .toEqual('Missing or incorrect confirmed appointments');
    });

    it('Missing or incorrect referrals in review link is visible', () => {
      wrapper = createIndexPage();
      ReferralsInReviewLink = wrapper.find('#btn_missingOrIncorrectReferralsInReview');

      expect(ReferralsInReviewLink.exists()).toBe(true);
      expect(ReferralsInReviewLink.text())
        .toEqual('Missing or incorrect referrals in review');
    });

    it('show other available services menu items', () => {
      wrapper = createIndexPage();
      otherAvailableServicesMenuItems = wrapper.find('#other-available-services-menu-items');
      ersJumpOffMenuItem = otherAvailableServicesMenuItems.find('#btn_manage_your_referral');

      expect(ersJumpOffMenuItem.exists()).toBe(false);
      expect(otherAvailableServicesMenuItems.exists()).toBe(true);
    });

    it('show back button', () => {
      wrapper = createIndexPage();
      backButton = wrapper.find('#desktopBackLink');

      expect(backButton.exists()).toBe(true);
    });
  });
});

describe('If summary care response returns with error', () => {
  let $store;
  let wrapper;
  let otherAvailableServicesMenuItems;
  let ersJumpOffMenuItem;

  const createIndexPage = () => mount(WayfinderPage, {
    $store,
  });

  beforeEach(() => {
    $store = setupStore(false, false, false, false, false, true);
    dependency.redirectTo = jest.fn();
  });

  describe('if page errored', () => {
    it('show other available services menu items', () => {
      wrapper = createIndexPage();
      otherAvailableServicesMenuItems = wrapper.find('#other-available-services-menu-items');
      ersJumpOffMenuItem = otherAvailableServicesMenuItems.find('#btn_manage_your_referral');

      expect(ersJumpOffMenuItem.exists()).toBe(true);
      expect(otherAvailableServicesMenuItems.exists()).toBe(true);
    });
  });
});
