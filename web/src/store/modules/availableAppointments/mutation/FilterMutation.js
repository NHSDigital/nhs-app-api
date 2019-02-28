/* eslint-disable */
import DateFilterValues from '@/store/modules/availableAppointments/dateFilter/Values';
import _ from 'lodash';
import { sortBy } from 'lodash/fp';

export const DATE_FORMAT = 'YYYY-MM-DD';

export default class FilterMutation {
  constructor(DateProvider, DateMapper) {
    this.dateProvider = DateProvider;
    this.dateMapper = DateMapper;
  }

  execute(slots, selectedOptions) {
    let dateRange = null;
    if (selectedOptions.date !== DateFilterValues.ALL) {
      dateRange = {
        from: this.dateMapper.mapToStartDate(selectedOptions.date),
        to: this.dateMapper.mapToEndDate(selectedOptions.date)
      };
    }

    if (!this._areMandatoryFieldsSelected(selectedOptions)) {
      return [];
    }

    let filteredSlotsMap = this._filter(slots, selectedOptions, dateRange);
    if (filteredSlotsMap.size > 0) {
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

  _filter(slots, selectedOptions, dateRange = null) {
    const sortedSlots = this._sort(slots);
    const filteredSlots = new Map();

    _.each(sortedSlots, (slot) => {
      const slotTime = this.dateProvider.create(slot.startTime);
      const day = slotTime.format(DATE_FORMAT);
      if (dateRange != null
        && (
          slotTime.isBefore(dateRange.from, 'day')
          || dateRange.to.isBefore(slotTime, 'day')
        )
      ) {
        return;
      }

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

      if (filteredSlots.has(day)) {
        const slotCollection = filteredSlots.get(day);
        const lastSlot = slotCollection[slotCollection.length -1];
        if (lastSlot.sessionName.toLowerCase() === slot.sessionName.toLowerCase()
          && selectedOptions.clinician === ''
          && this.dateProvider.create(lastSlot.startTime).isSame(this.dateProvider.create(slot.startTime), 'minute')) {
          return;
        }

        slotCollection.push(slot);
        filteredSlots.set(day, slotCollection);

      } else {
        filteredSlots.set(day, [slot]);
      }

    });

    return filteredSlots;
  }

  _sort(slots) {
    const sort = sortBy(slot => [
      slot.startTime,
    ]);

    return sort(slots);
  }
}
