import AddToCalendarInterrupt from '@/pages/appointments/gp-appointments/add-to-calendar-interrupt';
import i18n from '@/plugins/i18n';
import NativeApp from '@/services/native-app';
import * as dependency from '@/lib/utils';
import { APPOINTMENTS_PATH } from '@/router/paths';
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

  const createAddToCalendarInterruptPage = () => mount(
    AddToCalendarInterrupt,
    {
      $store,
      mountOpts: {
        i18n,
      },
    },
  );

  beforeEach(() => {
    $store = createStore({
      state: {
        device: {
          isNativeApp: true,
        },
        myAppointments: {
          selectedAppointment: {
            startTime: '2020-08-19T12:00:00-05:00',
            endTime: '2020-08-19T12:30:00-05:00',
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
      expect(dateAndTime.text()).toContain('Wednesday 19 August 2020 at 6:00pm');
    });
  });

  describe('heading', () => {
    it('page heading will be displayed', () => {
      wrapper = createAddToCalendarInterruptPage();
      heading = wrapper.find('.nhsuk-heading-xl');

      expect(heading.exists()).toBe(true);
      expect(heading.text()).toContain('If this appointment changes, you\'ll have to update your calendar yourself');
    });
  });

  describe('main paragraph', () => {
    it('main paragraph will be displayed', () => {
      wrapper = createAddToCalendarInterruptPage();
      para = wrapper.find('p');

      expect(para.exists()).toBe(true);
      expect(para.text()).toBe('Your calendar will not update automatically if the appointment is changed or cancelled.');
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

      const expectedCalendarData = '{"subject":"General","body":"1234\\nIAN, Clinic (Mr)","location":"EMISWebCR1 50002","startTimeEpochInSeconds":1597856400,"endTimeEpochInSeconds":1597856400}';

      expect($store.dispatch).toHaveBeenCalledWith('availableAppointments/completeBookingJourney');
      expect($store.dispatch).toHaveBeenCalledWith('availableAppointments/deselect');
      expect(NativeApp.addEventToCalendar).toHaveBeenCalledWith(expectedCalendarData);
      expect(dependency.redirectTo).toHaveBeenCalledWith(wrapper.vm, APPOINTMENTS_PATH);
    });
  });
});
