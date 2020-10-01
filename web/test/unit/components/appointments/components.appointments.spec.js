import each from 'jest-each';
import Appointment from '@/components/appointments/Appointment';
import { createStore, mount } from '../../helpers';

const createAppointmentComponent = (
  propsData,
  addToCalendarEnabled = false,
  isNativeApp = false,
) => mount(Appointment, {
  propsData,
  $store: createStore({
    state: {
      device: {
        isNativeApp,
      },
    },
    $env: {
      ADD_APPOINTMENT_TO_CALENDAR_ENABLED: addToCalendarEnabled,
    },
  }),
});

describe('Appointment.vue', () => {
  each([
    [true, true, 'true', true, true],
    [false, true, 'true', true, false],
    [true, false, 'true', true, false],
    [true, true, 'false', true, false],
    [true, true, 'true', false, false],
  ]).it('will show the add event to calendar link when all conditions are met', (
    showAddToCalendarLinkProp,
    nativeAppFunctionExists,
    addToCalendarEnabled,
    isNativeApp,
    expectedVisible,
  ) => {
    // arrange
    const appointment = {
      startTime: '2020-06-25T12:40:00+01:00',
      endTime: '2020-06-25T12:50:00+01:00',
    };
    const props = {
      appointment,
      showAddToCalendarLink: showAddToCalendarLinkProp,
    };

    window.nativeApp = nativeAppFunctionExists ? { addEventToCalendar: jest.fn() } : undefined;

    // act
    const component = createAppointmentComponent(
      props, addToCalendarEnabled, isNativeApp,
    );

    // assert
    expect(component.find('#add-event-to-calendar-link').exists()).toBe(expectedVisible);
  });
});
