import WayfinderPage from '@/pages/wayfinder/index';
import { createStore, mount } from '../../helpers';

jest.mock('@/lib/utils', () => ({
  ...jest.requireActual('@/lib/utils'),
  redirectTo: jest.fn(),
}));

let $store;
let wrapper;
let bookOrManageReferralsAndAppointmentsTitle;
let confirmedAppointmentsTitle;
let referralsInReviewNotOverdueTitle;
let noReferralsOrAppointmentsText;
let noConfirmedAppointmentsText;
let noReferralsInReviewNotOverdueText;
let referralsOrAppointmentsHelpLink;
let confirmedAppointmentsHelpLink;
let referralsInReviewHelpLink;
let otherAvailableServicesMenuItems;
let ersJumpOffMenuItem;
let backButton;
let doesNotMeetMinimumAgeMessage;
let noOtherServicesShowingMessage;

const setupStore = (
  hasActionableReferralsAndAppointments,
  hasConfirmedAppointments,
  hasReferralsInReviewNotOverdue,
  isNativeApp,
  hasApiError,
  hasApiErrorForUnderAge,
) => {
  let actionableReferralsAndAppointments = [];
  let confirmedAppointments = [];
  let referralsInReviewNotOverdue = [];
  let apiError = null;

  if (hasActionableReferralsAndAppointments) {
    actionableReferralsAndAppointments = [{
      referralId: '4',
      status: 'bookable',
      referrerOrganisation: 'Mahogany GP Surgery',
      referredDateTime: '2022-04-10T10:00:00',
      serviceSpecialty: 'Cardiology',
      deepLinkUrl: 'http://stubs.local.bitraft.io:8080/referral/bookable/4',
    }, {
      referralId: '17',
      status: 'inReview',
      referrerOrganisation: 'Mahogany GP Surgery',
      referredDateTime: '2022-04-10T10:00:00',
      reviewDueDate: '2022-04-18T10:00:00',
      serviceSpecialty: 'Haematology',
      deepLinkUrl: 'http://stubs.local.bitraft.io:8080/referral/overdue/17',
    }, {
      appointmentId: '2',
      locationDescription: 'A Clinic, A Town, A Country',
      deepLinkUrl: '',
    }];
  }

  if (hasConfirmedAppointments) {
    confirmedAppointments = [{
      appointmentId: '1',
      appointmentDateTime: '2022-04-18T10:00:00',
      locationDescription: 'A Clinic, A Town, A Country',
      deepLinkUrl: '',
    }];
  }

  if (hasReferralsInReviewNotOverdue) {
    referralsInReviewNotOverdue = [{
      referralId: '3',
      status: 'inReview',
      referrerOrganisation: 'Mahogany GP Surgery',
      referredDateTime: '2022-04-10T10:00:00',
      reviewDueDate: '2300-04-18T10:00:00',
      serviceSpecialty: 'Cardiology',
      deepLinkUrl: 'http://stubs.local.bitraft.io:8080/referral/inreview/3',
    }];
  }

  if (hasApiError) {
    apiError = {
      status: 500,
      serviceDeskReference: '521211',
    };
  }

  if (hasApiErrorForUnderAge) {
    apiError = {
      status: 470,
    };
  }

  return createStore({
    state: {
      device: {
        isNativeApp,
      },
      wayfinder: {
        summary: {
          actionableReferralsAndAppointments,
          confirmedAppointments,
          referralsInReviewNotOverdue,
        },
        apiError,
        hasLoaded: true,
      },
    },
    getters: {
      'knownServices/matchOneById': id => ({ id, url: 'www.url.com' }),
      'serviceJourneyRules/silverIntegrationAppointmentsEnabled': false,
      'serviceJourneyRules/silverIntegrationEnabled': () => true,
      'session/isProxying': false,
    },
  });
};

const mountPage = () => mount(
  WayfinderPage, {
    $store,
  },
);

describe('Summary care response with each type of summary item', () => {
  beforeEach(() => {
    $store = setupStore(true, true, true, true);
    wrapper = mountPage();
  });

  describe('if page loaded', () => {
    it('title text is shown for book or manage appointments section', () => {
      bookOrManageReferralsAndAppointmentsTitle = wrapper.find('#book-Or-Manage-Referrals-And-Appointments-Title');

      expect(bookOrManageReferralsAndAppointmentsTitle.exists()).toBe(true);
    });

    it('title text is shown for confirmed appointments section', () => {
      confirmedAppointmentsTitle = wrapper.find('#confirmed-appointments-title');

      expect(confirmedAppointmentsTitle.exists()).toBe(true);
    });

    it('title text is shown for referrals in review section', () => {
      referralsInReviewNotOverdueTitle = wrapper.find('#referrals-in-review-title');

      expect(referralsInReviewNotOverdueTitle.exists()).toBe(true);
    });
  });

  it('Missing or incorrect referrals or appointments link is visible', () => {
    referralsOrAppointmentsHelpLink = wrapper.find('#btn_missingOrIncorrectReferralsOrAppointments');

    expect(referralsOrAppointmentsHelpLink.text()).toEqual('Missing or incorrect referrals or appointments');
  });

  it('Missing or incorrect confirmed appointments link is visible', () => {
    confirmedAppointmentsHelpLink = wrapper.find('#btn_missingOrIncorrectConfirmedAppointments');

    expect(confirmedAppointmentsHelpLink.text()).toEqual('Missing or incorrect confirmed appointments');
  });

  it('Missing or incorrect referrals in review link is visible', () => {
    referralsInReviewHelpLink = wrapper.find('#btn_missingOrIncorrectReferralsInReview');

    expect(referralsInReviewHelpLink.text()).toEqual('Missing or incorrect referrals in review');
  });

  it('show other available services menu items without eRS jump-off', () => {
    otherAvailableServicesMenuItems = wrapper.find('#other-available-services-menu-items');
    ersJumpOffMenuItem = otherAvailableServicesMenuItems.find('#btn_manage_your_referral');

    expect(ersJumpOffMenuItem.exists()).toBe(false);
    expect(otherAvailableServicesMenuItems.exists()).toBe(true);
  });

  describe('if there are confirmed appointments', () => {
    it('hide resulting empty confirmed appointments text', () => {
      noConfirmedAppointmentsText = wrapper.find('#no-confirmed-appointments-text');

      expect(noConfirmedAppointmentsText.exists()).toBe(false);
    });
  });

  describe('if there are referrals in review', () => {
    it('hide resulting empty no referrals in review text', () => {
      noReferralsInReviewNotOverdueText = wrapper.find('#no-referrals-in-review-text');

      expect(noReferralsInReviewNotOverdueText.exists()).toBe(false);
    });
  });
});

describe('Summary care response with only actionable items', () => {
  beforeEach(() => {
    $store = setupStore(true, false, false, true);
    wrapper = mountPage();
  });

  describe('if page loaded', () => {
    it('title text is shown for book or manage appointments section', () => {
      bookOrManageReferralsAndAppointmentsTitle = wrapper.find('#book-Or-Manage-Referrals-And-Appointments-Title');

      expect(bookOrManageReferralsAndAppointmentsTitle.exists()).toBe(true);
    });

    it('title text is shown for confirmed appointments section', () => {
      confirmedAppointmentsTitle = wrapper.find('#confirmed-appointments-title');

      expect(confirmedAppointmentsTitle.exists()).toBe(true);
    });

    it('title text is shown for referrals in review section', () => {
      referralsInReviewNotOverdueTitle = wrapper.find('#referrals-in-review-title');

      expect(referralsInReviewNotOverdueTitle.exists()).toBe(true);
    });
  });

  it('Missing or incorrect referrals or appointments link is visible', () => {
    referralsOrAppointmentsHelpLink = wrapper.find('#btn_missingOrIncorrectReferralsOrAppointments');

    expect(referralsOrAppointmentsHelpLink.text()).toEqual('Missing or incorrect referrals or appointments');
  });

  it('Missing or incorrect confirmed appointments link is visible', () => {
    confirmedAppointmentsHelpLink = wrapper.find('#btn_missingOrIncorrectConfirmedAppointments');

    expect(confirmedAppointmentsHelpLink.text()).toEqual('Missing or incorrect confirmed appointments');
  });

  it('show other available services menu items without eRS jump-off', () => {
    otherAvailableServicesMenuItems = wrapper.find('#other-available-services-menu-items');
    ersJumpOffMenuItem = otherAvailableServicesMenuItems.find('#btn_manage_your_referral');

    expect(ersJumpOffMenuItem.exists()).toBe(false);
    expect(otherAvailableServicesMenuItems.exists()).toBe(true);
  });

  it('Missing or incorrect referrals in review link is visible', () => {
    referralsInReviewHelpLink = wrapper.find('#btn_missingOrIncorrectReferralsInReview');

    expect(referralsInReviewHelpLink.text()).toEqual('Missing or incorrect referrals in review');
  });

  describe('if there are no confirmed appointments', () => {
    it('show resulting empty confirmed appointments text', () => {
      noConfirmedAppointmentsText = wrapper.find('#no-confirmed-appointments-text');

      expect(noConfirmedAppointmentsText.exists()).toBe(true);
    });
  });

  describe('if there are no referrals in review', () => {
    it('show resulting empty no referrals in review text', () => {
      noReferralsInReviewNotOverdueText = wrapper.find('#no-referrals-in-review-text');

      expect(noReferralsInReviewNotOverdueText.text()).toBe('You have no referrals being reviewed.');
    });
  });
});

describe('Summary care response with no summary items', () => {
  beforeEach(() => {
    $store = setupStore(false, false, false, false);
    wrapper = mountPage();
  });

  describe('if page loaded', () => {
    it('show no referrals or appointments text', () => {
      noReferralsOrAppointmentsText = wrapper.find('#no-referrals-or-appointments-text');

      expect(noReferralsOrAppointmentsText.text()).toBe('You have no referrals or appointments to view or manage.');
    });

    describe('if there are no confirmed appointments', () => {
      it('show resulting empty confirmed appointments text', () => {
        noConfirmedAppointmentsText = wrapper.find('#no-confirmed-appointments-text');

        expect(noConfirmedAppointmentsText.exists()).toBe(true);
      });
    });

    describe('if there are no referrals in review', () => {
      it('show resulting empty no referrals in review text', () => {
        noReferralsInReviewNotOverdueText = wrapper.find('#no-referrals-in-review-text');

        expect(noReferralsInReviewNotOverdueText.text()).toBe('You have no referrals being reviewed.');
      });
    });

    it('Missing or incorrect referrals or appointments link is visible', () => {
      referralsOrAppointmentsHelpLink = wrapper.find('#btn_missingOrIncorrectReferralsOrAppointments');

      expect(referralsOrAppointmentsHelpLink.text()).toEqual('Missing or incorrect referrals or appointments');
    });

    it('Missing or incorrect confirmed appointments link is visible', () => {
      confirmedAppointmentsHelpLink = wrapper.find('#btn_missingOrIncorrectConfirmedAppointments');

      expect(confirmedAppointmentsHelpLink.text()).toEqual('Missing or incorrect confirmed appointments');
    });

    it('Missing or incorrect referrals in review link is visible', () => {
      referralsInReviewHelpLink = wrapper.find('#btn_missingOrIncorrectReferralsInReview');

      expect(referralsInReviewHelpLink.text()).toEqual('Missing or incorrect referrals in review');
    });

    it('show other available services menu items without eRS jump-off', () => {
      otherAvailableServicesMenuItems = wrapper.find('#other-available-services-menu-items');
      ersJumpOffMenuItem = otherAvailableServicesMenuItems.find('#btn_manage_your_referral');

      expect(ersJumpOffMenuItem.exists()).toBe(false);
      expect(otherAvailableServicesMenuItems.exists()).toBe(true);
    });

    it('show back button', () => {
      backButton = wrapper.find('#desktopBackLink');

      expect(backButton.exists()).toBe(true);
    });
  });
});

describe('Summary care response with API error', () => {
  beforeEach(() => {
    $store = setupStore(false, false, false, false, true);
    wrapper = mountPage();
  });

  describe('if page errored', () => {
    it('show other available services menu items with eRS jump-off', () => {
      otherAvailableServicesMenuItems = wrapper.find('#other-available-services-menu-items');
      ersJumpOffMenuItem = otherAvailableServicesMenuItems.find('#btn_manage_your_referral');

      expect(ersJumpOffMenuItem.exists()).toBe(true);
      expect(otherAvailableServicesMenuItems.exists()).toBe(true);
    });
  });
});

describe('Summary care response with API error for underage', () => {
  beforeEach(() => {
    $store = setupStore(false, false, false, false, false, true);
    wrapper = mountPage();
  });

  describe('if page errored', () => {
    it('show error message for minimum age', () => {
      doesNotMeetMinimumAgeMessage = wrapper.find('#doesNotMeetMinimumAge');

      expect(doesNotMeetMinimumAgeMessage.exists()).toBe(true);
      expect(doesNotMeetMinimumAgeMessage.text()).toEqual('If you\'re aged 15 or under you may be able to access your referrals and appointments using other services.');
    });

    it('show message for no other services showing', () => {
      noOtherServicesShowingMessage = wrapper.find('#noOtherServicesShowing');

      expect(noOtherServicesShowingMessage.exists()).toBe(true);
      expect(noOtherServicesShowingMessage.text()).toEqual('If no other services are showing, you\'ll need to contact the relevant organisation or healthcare provider for more information.');
    });
  });
});
