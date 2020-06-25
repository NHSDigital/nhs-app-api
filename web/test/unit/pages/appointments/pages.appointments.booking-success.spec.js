import * as dependency from '@/lib/utils';
import BookingSuccessPage from '@/pages/appointments/gp-appointments/booking-success';
import { createStore, mount } from '../../helpers';

jest.mock('@/lib/utils');

describe('booking-success.vue', () => {
  let $store;
  let wrapper;
  let appointmentDetailsCard;
  let backLink;
  let switchProfileButton;

  const createBookingSuccessPage = () => mount(BookingSuccessPage, {
    $store,
    stubs: {
      'page-title': '<div></div>',
    },
  });

  beforeEach(() => {
    $store = createStore({
      state: {
        device: {
          isNativeApp: true,
        },
        availableAppointments: {
          selectedSlot: {
            id: 193645,
            startTime: '2020-06-25T12:40:00+01:00',
            endTime: '2020-06-25T12:50:00+01:00',
            location: 'EMISWebCR1 50002',
            clinicians: ['IAN, Clinic (Mr)'],
            type: 'General',
            sessionName: '1234',
            channel: 0,
            ref: 'slot_0',
          },
        },
      },
      getters: {
        'session/isProxying': false,
      },
    });

    dependency.redirectTo = jest.fn();
  });


  describe('proxy mode is false', () => {
    it('appointment details card will exist', () => {
      wrapper = createBookingSuccessPage();
      appointmentDetailsCard = wrapper.find('#appointmentDetails');
      switchProfileButton = wrapper.find('#switchProfileButton');
      backLink = wrapper.find('#genericBackLink');

      expect(appointmentDetailsCard.exists()).toBe(true);
      expect(switchProfileButton.exists()).toBe(false);
      expect(backLink.exists()).toBe(true);
    });
  });

  describe('proxy mode is true', () => {
    it('appointment details card will not exist', () => {
      $store.getters['session/isProxying'] = true;
      wrapper = createBookingSuccessPage();
      appointmentDetailsCard = wrapper.find('#appointmentDetails');
      switchProfileButton = wrapper.find('#switchProfileButton');
      backLink = wrapper.find('#genericBackLink');

      expect(appointmentDetailsCard.exists()).toBe(false);
      expect(switchProfileButton.exists()).toBe(true);
      expect(backLink.exists()).toBe(false);
    });
  });
});
