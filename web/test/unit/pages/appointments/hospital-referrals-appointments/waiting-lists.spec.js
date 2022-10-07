import WaitingListPage from '@/pages/appointments/hospital-referrals-appointments/waiting-lists';
import { createStore, mount } from '../../../helpers';

jest.mock('@/lib/utils', () => ({
  ...jest.requireActual('@/lib/utils'),
  redirectTo: jest.fn(),
}));

let $store;
let wrapper;
let waitTimesTitle;
let somethingWrongText;
let waitTimesText;

const setupStore = (
  waitTimesCount,
) => {
  let waitTimes = [];

  if (waitTimesCount === 1) {
    waitTimes = [{
      plannedWaitTime: '40',
      referredDate: '2022-04-18T10:00:00',
      providerName: 'Oak GP Surgery',
      speciality: 'Neurology',
    }];
  } else if (waitTimesCount > 1) {
    waitTimes = [{
      plannedWaitTime: '40',
      referredDate: '2022-04-18T10:00:00',
      providerName: 'Oak GP Surgery',
      speciality: 'Neurology',
    }, {
      plannedWaitTime: '60',
      referredDate: '2022-07-18T10:00:00',
      providerName: 'Oak GP Surgery',
      speciality: 'Orthopedic',
    }];
  }

  return createStore({
    state: {
      wayfinder: {
        waitTimes: {
          waitTimes,
        },
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
    $store = setupStore(0);
    wrapper = mountPage();
  });

  describe('if page loaded', () => {
    it('title text is shown for book or wait times', () => {
      waitTimesTitle = wrapper.find('#wait-Times-Title');

      expect(waitTimesTitle.exists()).toBe(true);
    });
  });

  it('is something wrong text is visible', () => {
    somethingWrongText = wrapper.find('#something-Wrong');

    expect(somethingWrongText.text()).toEqual('Is something wrong?');
  });

  describe('waiting list with no wait times', () => {
    it('show zero waiting list text', () => {
      waitTimesText = wrapper.find('#wait-Times-Content');
      expect(waitTimesText.text()).toEqual('You\'re on 0 waiting lists');
    });
  });

  describe('waiting list with single wait time', () => {
    beforeEach(() => {
      $store = setupStore(1);
      wrapper = mountPage();
    });
    it('show single wait time text', () => {
      waitTimesText = wrapper.find('#wait-Times-Content');
      expect(waitTimesText.text()).toEqual('You\'re on a waiting list');
    });
  });

  describe('waiting list with more than one wait times', () => {
    beforeEach(() => {
      $store = setupStore(2);
      wrapper = mountPage();
    });
    it('show multiple wait time text', () => {
      waitTimesText = wrapper.find('#wait-Times-Content');
      expect(waitTimesText.text()).toEqual('You\'re on 2 waiting lists');
    });
  });
});
