import WayfinderPage from '@/pages/appointments/hospital-referrals-appointments/hospital-referrals-appointments';
import { createStore, mount } from '../../../helpers';

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
let waitTimesJumpOffMenuItem;
let waitingListsMenuItems;

const setupStore = (
  hasActionableReferralsAndAppointments,
  hasConfirmedAppointments,
  hasReferralsInReviewNotOverdue,
  isNativeApp,
  hasApiError,
  hasApiErrorForUnderAge,
  isWaitTimesEnabled,
) => {
  let actionableReferralsAndAppointments = [];
  let confirmedAppointments = [];
  let referralsInReviewNotOverdue = [];
  let apiError = null;

  if (hasActionableReferralsAndAppointments) {
    actionableReferralsAndAppointments = [{
      referralId: '4',
      status: 'bookable',
      serviceSpecialty: 'Cardiology',
      deepLinkUrl: 'http://stubs.local.bitraft.io:8080/referral/bookable/4',
    }, {
      referralId: '17',
      status: 'inReview',
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
    $env: {
      SECONDARY_CARE_WAIT_TIMES_ENABLED: isWaitTimesEnabled,
    },
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

const mountPageWithWaitTimesEnabled = () => mount(
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
      expect(bookOrManageReferralsAndAppointmentsTitle.text()).toEqual('You have 3 referrals or appointments you need to action');
    });

    it('title text is shown for confirmed appointments section', () => {
      confirmedAppointmentsTitle = wrapper.find('#confirmed-appointments-title');

      expect(confirmedAppointmentsTitle.exists()).toBe(true);
      expect(confirmedAppointmentsTitle.text()).toEqual('You have 1 upcoming appointment');
    });

    it('title text is shown for referrals in review section', () => {
      referralsInReviewNotOverdueTitle = wrapper.find('#referrals-in-review-title');

      expect(referralsInReviewNotOverdueTitle.exists()).toBe(true);
      expect(referralsInReviewNotOverdueTitle.text()).toEqual('You have 1 referral being reviewed');
    });
  });

  it('Missing, incorrect or cancelled referrals or appointments link is visible', () => {
    referralsOrAppointmentsHelpLink = wrapper.find('#wayfinder-help-jump-off-link-referrals-or-appointments');

    expect(referralsOrAppointmentsHelpLink.text()).toEqual('What to do if a referral or appointment is missing, incorrect or has not been cancelled');
  });

  it('Missing, incorrect or cancelled confirmed appointments link is visible', () => {
    confirmedAppointmentsHelpLink = wrapper.find('#wayfinder-help-jump-off-link-appointments');

    expect(confirmedAppointmentsHelpLink.text()).toEqual('What to do if an appointment is missing, incorrect or has not been changed or cancelled');
  });

  it('Missing, incorrect or cancelled referrals in review link is visible', () => {
    referralsInReviewHelpLink = wrapper.find('#wayfinder-help-jump-off-link-referrals');

    expect(referralsInReviewHelpLink.text()).toEqual('What to do if a referral being reviewed by a clinic is missing or incorrect');
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
      expect(bookOrManageReferralsAndAppointmentsTitle.text()).toEqual('You have 3 referrals or appointments you need to action');
    });

    it('title text is shown for confirmed appointments section', () => {
      confirmedAppointmentsTitle = wrapper.find('#confirmed-appointments-title');

      expect(confirmedAppointmentsTitle.exists()).toBe(true);
      expect(confirmedAppointmentsTitle.text()).toEqual('You have 0 upcoming appointments');
    });

    it('title text is shown for referrals in review section', () => {
      referralsInReviewNotOverdueTitle = wrapper.find('#referrals-in-review-title');

      expect(referralsInReviewNotOverdueTitle.exists()).toBe(true);
      expect(referralsInReviewNotOverdueTitle.text()).toEqual('You have 0 referrals being reviewed');
    });
  });

  it('Missing, incorrect or cancelled referrals or appointments link is visible', () => {
    referralsOrAppointmentsHelpLink = wrapper.find('#wayfinder-help-jump-off-link-referrals-or-appointments');

    expect(referralsOrAppointmentsHelpLink.text()).toEqual('What to do if a referral or appointment is missing, incorrect or has not been cancelled');
  });

  it('Missing, incorrect or cancelled confirmed appointments link is visible', () => {
    confirmedAppointmentsHelpLink = wrapper.find('#wayfinder-help-jump-off-link-appointments');

    expect(confirmedAppointmentsHelpLink.text()).toEqual('What to do if an appointment is missing, incorrect or has not been changed or cancelled');
  });

  it('show other available services menu items without eRS jump-off', () => {
    otherAvailableServicesMenuItems = wrapper.find('#other-available-services-menu-items');
    ersJumpOffMenuItem = otherAvailableServicesMenuItems.find('#btn_manage_your_referral');

    expect(ersJumpOffMenuItem.exists()).toBe(false);
    expect(otherAvailableServicesMenuItems.exists()).toBe(true);
  });

  it('Missing, incorrect or cancelled referrals in review link is visible', () => {
    referralsInReviewHelpLink = wrapper.find('#wayfinder-help-jump-off-link-referrals');

    expect(referralsInReviewHelpLink.text()).toEqual('What to do if a referral being reviewed by a clinic is missing or incorrect');
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
    it('title text is shown for book or manage appointments section', () => {
      bookOrManageReferralsAndAppointmentsTitle = wrapper.find('#book-Or-Manage-Referrals-And-Appointments-Title');

      expect(bookOrManageReferralsAndAppointmentsTitle.exists()).toBe(true);
      expect(bookOrManageReferralsAndAppointmentsTitle.text()).toEqual('You have 0 referrals or appointments you need to action');
    });

    it('title text is shown for confirmed appointments section', () => {
      confirmedAppointmentsTitle = wrapper.find('#confirmed-appointments-title');

      expect(confirmedAppointmentsTitle.exists()).toBe(true);
      expect(confirmedAppointmentsTitle.text()).toEqual('You have 0 upcoming appointments');
    });

    it('title text is shown for referrals in review section', () => {
      referralsInReviewNotOverdueTitle = wrapper.find('#referrals-in-review-title');

      expect(referralsInReviewNotOverdueTitle.exists()).toBe(true);
      expect(referralsInReviewNotOverdueTitle.text()).toEqual('You have 0 referrals being reviewed');
    });

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

    it('Missing, incorrect or cancelled referrals or appointments link is visible', () => {
      referralsOrAppointmentsHelpLink = wrapper.find('#wayfinder-help-jump-off-link-referrals-or-appointments');

      expect(referralsOrAppointmentsHelpLink.text()).toEqual('What to do if a referral or appointment is missing, incorrect or has not been cancelled');
    });

    it('Missing, incorrect or cancelled confirmed appointments link is visible', () => {
      confirmedAppointmentsHelpLink = wrapper.find('#wayfinder-help-jump-off-link-appointments');

      expect(confirmedAppointmentsHelpLink.text()).toEqual('What to do if an appointment is missing, incorrect or has not been changed or cancelled');
    });

    it('Missing, incorrect or cancelled referrals in review link is visible', () => {
      referralsInReviewHelpLink = wrapper.find('#wayfinder-help-jump-off-link-referrals');

      expect(referralsInReviewHelpLink.text()).toEqual('What to do if a referral being reviewed by a clinic is missing or incorrect');
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

describe('Secondary care with wait times enabled', () => {
  beforeEach(() => {
    $store = setupStore(false, false, false, false, false, false, true);
    wrapper = mountPageWithWaitTimesEnabled();
  });
  it('show wait time menu items', () => {
    waitingListsMenuItems = wrapper.find('#wait-times-menu-item');
    waitTimesJumpOffMenuItem = waitingListsMenuItems.find('#btn_wait_times');

    expect(waitTimesJumpOffMenuItem.exists()).toBe(true);
    expect(waitingListsMenuItems.exists()).toBe(true);
  });
});


describe('Secondary care with wait times disabled', () => {
  beforeEach(() => {
    $store = setupStore(false, false, false, false, false, false, false);
    wrapper = mountPage();
  });
  it('do not show wait time menu items', () => {
    waitTimesJumpOffMenuItem = wrapper.find('#wait-times-menu-item');
    expect(waitTimesJumpOffMenuItem.exists()).toBe(false);
  });
});
