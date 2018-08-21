/* eslint-disable */
import DateFilterValues from '@/store/modules/availableAppointments/dateFilter/Values';
import { DATE_FORMAT } from '@/store/modules/availableAppointments/mutation/LoadMutation';

export default class FilterMutation {
  constructor(DateProvider, DateMapper) {
    this.dateProvider = DateProvider;
    this.dateMapper = DateMapper;
  }

  execute(data, selectedOptions) {
    let dateRange = null;
    if (selectedOptions.date !== DateFilterValues.ALL) {
      dateRange = {
        from: this.dateMapper.mapToStartDate(selectedOptions.date),
        to: this.dateMapper.mapToEndDate(selectedOptions.date)
      };
    }

    let filteredSlotsMap = this._filter(data, selectedOptions, dateRange);
    if (this._areMandatoryFieldsSelected(selectedOptions) && filteredSlotsMap.size > 0) {
      filteredSlotsMap = this._addDaysWithNoAppointments(filteredSlotsMap, dateRange);
    }

    return Array.from(filteredSlotsMap);
  }

  _addDaysWithNoAppointments(filteredSlotsMap, dateRange = null) {
    if (dateRange === null) {
      return filteredSlotsMap;
    }

    const fromDate = dateRange.from.clone();
    const toDate = dateRange.to.clone();
    const slots = new Map();
    while (fromDate.isSameOrBefore(toDate, 'day')) {
      const date = fromDate.format(DATE_FORMAT);
      if (filteredSlotsMap.has(date)) {
        slots.set(date, filteredSlotsMap.get(date));
      } else {
        slots.set(date, []);
      }

      fromDate.add(1, 'days');
    }

    return slots;
  }

  _areMandatoryFieldsSelected(selectedOptions) {
    if (selectedOptions.type === '' || selectedOptions.location === '') {
      return false;
    }

    return true;
  }

  _filter(data, selectedOptions, dateRange = null) {
    const filteredSlots = new Map();
    if (!this._areMandatoryFieldsSelected(selectedOptions)) {
      return filteredSlots;
    }

    data.forEach((slots, startTime) => {
      const slotTime = this.dateProvider.create(startTime);
      if (dateRange != null
        && (
          slotTime.isBefore(dateRange.from, 'day')
          || dateRange.to.isBefore(slotTime, 'day')
        )
      ) {
        return;
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

    return filteredSlots;
  }
}
