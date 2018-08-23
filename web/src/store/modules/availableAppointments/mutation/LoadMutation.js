/* eslint-disable */
import _ from 'lodash';
import DateFilterValues from '@/store/modules/availableAppointments/dateFilter/Values';

export default class LoadMutation {
  constructor(DateProvider) {
    this.dateProvider = DateProvider;
  }

  execute(data) {
    const filters = new Map();
    const types = [];
    const locations = [];
    const clinicians = [];
    let defaultLocationSelectedOption = '';

    _.each(data.slots, (slot, index) => {
      slot.ref = `slot_${index}`;

      if (!filters.has(slot.type)) {
        filters.set(slot.type, true);
        types.push({ value: slot.type, name: slot.type, translate: false });
      }

      if (!filters.has(slot.location)) {
        filters.set(slot.location, true);
        locations.push({ value: slot.location, name: slot.location, translate: false });
      }

      _.each(slot.clinicians, (clinician) => {
        if (!filters.has(clinician)) {
          filters.set(clinician, true);
          clinicians.push({ value: clinician, name: clinician, translate: false });
        }
      });
    });

    if (locations.length === 1) {
      defaultLocationSelectedOption = locations[0].value;
    }

    const dates = [
      { value: DateFilterValues.TODAY, name: 'appointments.booking.filters.date.options.today', translate: true },
      { value: DateFilterValues.TOMORROW, name: 'appointments.booking.filters.date.options.tomorrow', translate: true },
      { value: DateFilterValues.THIS_WEEK, name: 'appointments.booking.filters.date.options.this_week', translate: true },
      { value: DateFilterValues.NEXT_WEEK, name: 'appointments.booking.filters.date.options.next_week', translate: true },
      { value: DateFilterValues.ALL, name: 'appointments.booking.filters.date.options.all', translate: true },
    ];

    types.sort();
    types.unshift({ value: '', name: 'appointments.booking.filters.type.default_option', translate: true });

    locations.sort();
    locations.unshift({ value: '', name: 'appointments.booking.filters.location.default_option', translate: true });

    clinicians.sort();
    clinicians.unshift({ value: '', name: 'appointments.booking.filters.clinician.default_option', translate: true });

    return {
      slots: data.slots,
      filtersOptions: {
        types,
        locations,
        clinicians,
        dates,
      },
      defaultLocationSelectedOption
    };
  }
}
