/* eslint-disable */
import DateFilterValues from '@/store/modules/availableAppointments/dateFilter/Values';

export default class FilterMutation {
  constructor(DateProvider, DateMapper) {
    this.dateProvider = DateProvider;
    this.dateMapper = DateMapper;
  }

  execute(data, selectedOptions) {
    const filteredSlots = new Map();
    if (selectedOptions.type === '' || selectedOptions.location === '') {
      return Array.from(filteredSlots);
    }

    data.forEach((slots, startTime) => {
      const slotTime = this.dateProvider.create(startTime);
      if (selectedOptions.date !== DateFilterValues.ALL
        && (
          slotTime.isBefore(this.dateMapper.mapToStartDate(selectedOptions.date), 'day')
          || this.dateMapper.mapToEndDate(selectedOptions.date).isBefore(slotTime, 'day')
        )
      ) {
        return Array.from(filteredSlots);
      }

      slots.forEach((slot) => {
        if (selectedOptions.type !== '' && selectedOptions.type !== slot.type) {
          return;
        }

        if (selectedOptions.location !== '' && selectedOptions.location !== slot.location) {
          return;
        }

        if (selectedOptions.clinician !== ''
          && Array.isArray(slot.clinicians)
          && slot.clinicians.indexOf(selectedOptions.clinician) === -1) {
          return;
        }

        if (filteredSlots.has(startTime)) {
          const slotCollection = filteredSlots.get(startTime);
          slotCollection.push(slot);
          filteredSlots.set(startTime, slotCollection);
        } else {
          filteredSlots.set(startTime, [slot]);
        }
      });
    });

    return Array.from(filteredSlots);
  }
}
