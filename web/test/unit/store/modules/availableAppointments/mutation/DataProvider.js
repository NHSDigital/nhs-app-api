/* jshint esversion: 6 */

export default class DataProvider {
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
      type: 'Emergency',
      startTime: today.toISOString(),
      endTime: today.toISOString(),
      location: 'Leeds',
      clinicians: ['Dr House'],
    };

    const slot2 = {
      id: 2,
      type: 'Emergency',
      startTime: today.toISOString(),
      endTime: today.toISOString(),
      location: 'Leeds',
      clinicians: ['Dr House', 'Dr Drake Ramoray'],
    };

    const slot3 = {
      id: 3,
      type: 'Emergency',
      startTime: startOfTomorrow.toISOString(),
      endTime: startOfTomorrow.toISOString(),
      location: 'Leeds',
      clinicians: ['Dr House', 'Dr Drake Ramoray'],
    };

    const slot4 = {
      id: 4,
      type: 'Emergency',
      startTime: endOfTomorrow.toISOString(),
      endTime: endOfTomorrow.toISOString(),
      location: 'Leeds',
      clinicians: ['Dr House', 'Dr Drake Ramoray'],
    };

    const slot5 = {
      id: 5,
      type: 'Emergency',
      startTime: startOfNextWeek.toISOString(),
      endTime: startOfNextWeek.toISOString(),
      location: 'Leeds',
      clinicians: ['Dr House'],
    };

    const slot6 = {
      id: 6,
      type: 'Emergency',
      startTime: endOfNextWeek.toISOString(),
      endTime: endOfNextWeek.toISOString(),
      location: 'Leeds',
      clinicians: ['Dr House'],
    };

    const slot7 = {
      id: 7,
      type: 'Baby immunisations',
      startTime: today.toISOString(),
      endTime: today.toISOString(),
      location: 'Leeds',
      clinicians: ['Dr House'],
    };

    const slot8 = {
      id: 8,
      type: 'Emergency',
      startTime: today.toISOString(),
      endTime: today.toISOString(),
      location: 'London',
      clinicians: ['Dr House'],
    };

    const slot9 = {
      id: 9,
      type: 'Emergency',
      startTime: today.toISOString(),
      endTime: today.toISOString(),
      location: 'Leeds',
      clinicians: ['Dr Who'],
    };

    const slot10 = {
      id: 10,
      type: 'Emergency',
      startTime: startOfNext2Week.toISOString(),
      endTime: startOfNext2Week.toISOString(),
      location: 'Leeds',
      clinicians: ['Dr House'],
    };

    const slot11 = {
      id: 11,
      type: 'Emergency',
      startTime: endOfThisWeek.toISOString(),
      endTime: endOfThisWeek.toISOString(),
      location: 'Leeds',
      clinicians: ['Dr House'],
    };

    return { slot1, slot2, slot3, slot4, slot5, slot6, slot7, slot8, slot9, slot10, slot11 };
  }
}
