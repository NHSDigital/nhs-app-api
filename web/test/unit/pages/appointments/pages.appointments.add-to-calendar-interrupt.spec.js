import * as dependency from '@/lib/utils';
import AddToCalendarInterrupt from '@/pages/appointments/gp-appointments/add-to-calendar-interrupt';
import { APPOINTMENTS_PATH } from '@/router/paths';
import NativeApp from '@/services/native-app';
import { createStore, mount } from '../../helpers';

jest.mock('@/lib/utils');
jest.mock('@/services/native-app');

describe('add-to-calendar-interrupt.vue', () => {
  let $store;
  let wrapper;

  let dateAndTime;
  let heading;
  let para;
  let addToCalendarButton;

  const createAddToCalendarInterruptPage = () => mount(AddToCalendarInterrupt, { $store });

  beforeEach(() => {
    $store = createStore({
      state: {
        device: {
          isNativeApp: true,
        },
        myAppointments: {
          selectedAppointment: {
            startTime: '2020-06-25T12:40:00+01:00',
            endTime: '2020-06-25T12:50:00+01:00',
            location: 'EMISWebCR1 50002',
            clinicians: ['IAN, Clinic (Mr)'],
            type: 'General',
            sessionName: '1234',
          },
        },
      },
    });
  });

  describe('date and time', () => {
    it('appointment date and time will be displayed', () => {
      wrapper = createAddToCalendarInterruptPage();
      dateAndTime = wrapper.find('.nhsuk-caption-m');

      expect(dateAndTime.exists()).toBe(true);
      expect(dateAndTime.text()).toContain('Thursday 25 June 2020 at 12:40pm');
    });
  });

  describe('heading', () => {
    it('page heading will be displayed', () => {
      wrapper = createAddToCalendarInterruptPage();
      heading = wrapper.find('.nhsuk-heading-xl');

      expect(heading.exists()).toBe(true);
      expect(heading.text()).toContain('translate_appointments.addToCalendar.paragraph1');
    });
  });

  describe('main paragraph', () => {
    it('main paragraph will be displayed', () => {
      wrapper = createAddToCalendarInterruptPage();
      para = wrapper.find('p');

      expect(para.exists()).toBe(true);
      expect(para.text()).toContain('translate_appointments.addToCalendar.paragraph2');
    });
  });

  describe('add to calendar button', () => {
    beforeEach(() => {
      dependency.redirectTo = jest.fn();
      wrapper = createAddToCalendarInterruptPage();
      addToCalendarButton = wrapper.find('#addToCalendarButton');
    });

    it('add to calendar button will exist', () => {
      expect(addToCalendarButton.exists()).toBe(true);
    });

    it('calls addEventToCalendar when clicked', () => {
      addToCalendarButton.trigger('click');

      expect($store.dispatch).toHaveBeenCalledWith('availableAppointments/completeBookingJourney');
      expect($store.dispatch).toHaveBeenCalledWith('availableAppointments/deselect');
      expect(NativeApp.addEventToCalendar).toHaveBeenCalled();
      expect(dependency.redirectTo).toHaveBeenCalledWith(wrapper.vm, APPOINTMENTS_PATH);
    });
  });
});
