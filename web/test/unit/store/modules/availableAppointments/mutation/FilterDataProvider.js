/* jshint esversion: 6 */
import assign from 'lodash/fp/assign';

export default class FilterDataProvider {
  constructor(DateProvider) {
    this.dateProvider = DateProvider;
  }

  createDate() {
    return this.dateProvider.create();
  }

  generate() {
    const today = this.createDate();
    today.set({ h: 23, m: 59, s: 59 });

    const startOfTomorrow = this.createDate();
    startOfTomorrow.add(1, 'days');
    startOfTomorrow.set({ h: 0, m: 0, s: 0 });

    const endOfTomorrow = this.createDate();
    endOfTomorrow.add(1, 'days');
    endOfTomorrow.set({ h: 23, m: 59, s: 59 });

    const endOfThisWeek = this.createDate();
    endOfThisWeek.add((today.day() === 0) ? 0 : 7 - today.day(), 'days');
    endOfThisWeek.set({ h: 23, m: 59, s: 59 });

    const startOfNextWeek = this.createDate();
    startOfNextWeek.add((today.day() === 0) ? 1 : 8 - today.day(), 'days');
    startOfNextWeek.set({ h: 0, m: 0, s: 0 });

    const endOfNextWeek = this.createDate();
    endOfNextWeek.add((today.day() === 0) ? 7 : 14 - today.day(), 'days');
    endOfNextWeek.set({ h: 23, m: 59, s: 59 });

    const startOfNext2Week = this.createDate();
    startOfNext2Week.add((today.day() === 0) ? 8 : 15 - today.day(), 'days');
    startOfNext2Week.set({ h: 0, m: 0, s: 0 });

    const slot1 = {
      id: 1,
      ref: 'slot_1',
      type: 'Emergency',
      startTime: today.toISOString(),
      endTime: today.toISOString(),
      location: 'Leeds',
      sessionName: 'General Appointment',
      clinicians: ['Dr House'],
    };

    const slot2 = {
      id: 2,
      ref: 'slot_2',
      type: 'Emergency',
      startTime: today.toISOString(),
      endTime: today.toISOString(),
      location: 'Leeds',
      sessionName: 'General Appointment',
      clinicians: ['Dr House', 'Dr Drake Ramoray'],
    };

    const slot3 = {
      id: 3,
      ref: 'slot_3',
      type: 'Emergency',
      startTime: startOfTomorrow.toISOString(),
      endTime: startOfTomorrow.toISOString(),
      location: 'Leeds',
      sessionName: 'General Appointment',
      clinicians: ['Dr House', 'Dr Drake Ramoray'],
    };

    const slot4 = {
      id: 4,
      ref: 'slot_4',
      type: 'Emergency',
      startTime: endOfTomorrow.toISOString(),
      endTime: endOfTomorrow.toISOString(),
      location: 'Leeds',
      sessionName: 'General Appointment',
      clinicians: ['Dr House', 'Dr Drake Ramoray'],
    };

    const slot5 = {
      id: 5,
      ref: 'slot_5',
      type: 'Emergency',
      startTime: startOfNextWeek.toISOString(),
      endTime: startOfNextWeek.toISOString(),
      location: 'Leeds',
      sessionName: 'General Appointment',
      clinicians: ['Dr House'],
    };

    const slot6 = {
      id: 6,
      ref: 'slot_6',
      type: 'Emergency',
      startTime: endOfNextWeek.toISOString(),
      endTime: endOfNextWeek.toISOString(),
      location: 'Leeds',
      sessionName: 'General Appointment',
      clinicians: ['Dr House'],
    };

    const slot7 = {
      id: 7,
      ref: 'slot_7',
      type: 'Baby immunisations',
      startTime: today.toISOString(),
      endTime: today.toISOString(),
      location: 'Leeds',
      sessionName: 'General Appointment',
      clinicians: ['Dr House'],
    };

    const slot8 = {
      id: 8,
      ref: 'slot_8',
      type: 'Emergency',
      startTime: today.toISOString(),
      endTime: today.toISOString(),
      location: 'London',
      sessionName: 'General Appointment',
      clinicians: ['Dr House'],
    };

    const slot9 = {
      id: 9,
      ref: 'slot_9',
      type: 'Emergency',
      startTime: today.toISOString(),
      endTime: today.toISOString(),
      location: 'Leeds',
      sessionName: 'General Appointment',
      clinicians: ['Dr Who'],
    };

    const slot10 = {
      id: 10,
      ref: 'slot_10',
      type: 'Emergency',
      startTime: startOfNext2Week.toISOString(),
      endTime: startOfNext2Week.toISOString(),
      location: 'Leeds',
      sessionName: 'General Appointment',
      clinicians: ['Dr House'],
    };

    const slot11 = {
      id: 11,
      ref: 'slot_11',
      type: 'Emergency',
      startTime: endOfThisWeek.toISOString(),
      endTime: endOfThisWeek.toISOString(),
      location: 'Leeds',
      sessionName: 'General Appointment',
      clinicians: ['Dr House'],
    };

    const slot12 = {
      id: 12,
      ref: 'slot_12',
      type: 'Emergency',
      startTime: today.clone().subtract(15, 'minutes').toISOString(),
      endTime: today.clone().subtract(15, 'minutes'),
      location: 'Leeds',
      sessionName: 'General Appointment',
      clinicians: ['Dr Drake Ramoray'],
    };

    const slot13 = assign({}, slot1);
    slot13.ref = 'slot_13';
    const slot14 = assign({}, slot2);
    slot14.ref = 'slot_14';
    const slot15 = assign({}, slot4);
    slot15.ref = 'slot_15';
    const slot16 = assign({}, slot6);
    slot16.ref = 'slot_16';
    const slot17 = assign({}, slot10);
    slot17.ref = 'slot_17';
    const slot18 = assign({}, slot11);
    slot18.ref = 'slot_18';

    return {
      slot1,
      slot2,
      slot3,
      slot4,
      slot5,
      slot6,
      slot7,
      slot8,
      slot9,
      slot10,
      slot11,
      slot12,
      slot13,
      slot14,
      slot15,
      slot16,
      slot17,
      slot18,
    };
  }
}
