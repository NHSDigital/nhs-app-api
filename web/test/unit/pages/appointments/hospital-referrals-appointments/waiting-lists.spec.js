import WaitingListPage from '@/pages/appointments/hospital-referrals-appointments/waiting-lists';
import { createStore, mount } from '../../../helpers';

jest.mock('@/lib/utils', () => ({
  ...jest.requireActual('@/lib/utils'),
  redirectTo: jest.fn(),
}));

let $store;
let wrapper;
let waitTimesTitle;
let waitTimesText;
let jumpOffLink;
let technicalProblemMessage;
let informationCurrentlyUnavailableMessage;
let cannotViewTryAgainMessage;

const setupStore = (
  waitTimesCount,
  hasApiError,
  isNativeApp,
) => {
  let waitTimes = [];
  let apiError = null;

  if (waitTimesCount === 1) {
    waitTimes = [{
      estimatedWaitToTreatmentDisplayDate: 'February 2023',
      plannedWaitTime: '40',
      referredDate: '2022-04-18T10:00:00',
      providerName: 'Oak GP Surgery',
      speciality: 'Neurology',
    }];
  } else if (waitTimesCount > 1) {
    waitTimes = [{
      estimatedWaitToTreatmentDisplayDate: 'February 2023',
      plannedWaitTime: '40',
      referredDate: '2022-04-18T10:00:00',
      providerName: 'Oak GP Surgery',
      speciality: 'Neurology',
    }, {
      estimatedWaitToTreatmentDisplayDate: 'February 2023',
      plannedWaitTime: '60',
      referredDate: '2022-07-18T10:00:00',
      providerName: 'Oak GP Surgery',
      speciality: 'Orthopedic',
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
        waitTimes,
        apiError,
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
  WaitingListPage, {
    $store,
  },
);

describe('Wait times page', () => {
  beforeEach(() => {
    $store = setupStore(0, false, true);
    wrapper = mountPage();

    waitTimesText = wrapper.find('#wait-times-content');
  });

  describe('if page loaded', () => {
    it('title text and something wrong text shown for wait times', () => {
      waitTimesTitle = wrapper.find('#wait-times-title');
      const isSomethingWrongElement = wrapper.find('#something-wrong');

      expect(waitTimesTitle.exists()).toBe(true);
      expect(isSomethingWrongElement.text()).toEqual('Is something wrong?');
    });

    it('missing appointment jump-off is visible and text correct', () => {
      jumpOffLink = wrapper.find('#wayfinder-help-jump-off-link-wait-times');

      expect(jumpOffLink.exists()).toBe(true);
      expect(jumpOffLink.text()).toEqual('What to do if something is missing, incorrect or has not been changed or cancelled');
    });
  });

  describe('waiting list with no wait times', () => {
    it('show zero waiting list text', () => {
      expect(waitTimesText.text()).toEqual('You\'re on 0 waiting lists');
    });
  });
});

describe('waiting list with single wait time', () => {
  beforeEach(() => {
    $store = setupStore(1);
    wrapper = mountPage();

    waitTimesText = wrapper.find('#wait-times-content');
  });
  it('show single wait time text', () => {
    expect(waitTimesText.text()).toEqual('You\'re on a waiting list');
  });
});

describe('waiting list with more than one wait times', () => {
  beforeEach(() => {
    $store = setupStore(2);
    wrapper = mountPage();

    waitTimesText = wrapper.find('#wait-times-content');
  });
  it('show multiple wait time text', () => {
    expect(waitTimesText.text()).toEqual('You\'re on 2 waiting lists');
  });
});

describe('WaitTimes with API error', () => {
  beforeEach(() => {
    $store = setupStore(0, true);
    wrapper = mountPage();
  });

  describe('if page errored', () => {
    it('show error message for technical problem', () => {
      technicalProblemMessage = wrapper.find('#technicalProblem');

      expect(technicalProblemMessage.exists()).toBe(true);
      expect(technicalProblemMessage.text()).toEqual('There is a technical problem.');
    });

    it('show message for information currently unavailable', () => {
      informationCurrentlyUnavailableMessage = wrapper.find('#informationCurrentlyUnavailable');

      expect(informationCurrentlyUnavailableMessage.exists()).toBe(true);
      expect(informationCurrentlyUnavailableMessage.text()).toEqual('Information on waiting times is currently unavailable.');
    });

    it('show message cannot view try again', () => {
      cannotViewTryAgainMessage = wrapper.find('#cannotViewTryAgain');

      expect(cannotViewTryAgainMessage.exists()).toBe(true);
      expect(cannotViewTryAgainMessage.text()).toEqual('Try again later.');
    });
  });
});
