/* eslint-disable no-shadow */
/* eslint-disable no-param-reassign */
/* eslint-disable no-unused-vars */
/* eslint-disable import/extensions */
import moment from 'moment-timezone';
import { assign, find, get, map, mapKeys, sortBy } from 'lodash/fp';
import {
  INIT,
  LOAD,
  SELECT,
  DESELECT,
  CLEAR,
  FILTER,
  SET_SELECTED_OPTIONS,
} from './mutation-types';

const clearState = (state) => {
  state.slots = new Map();
  state.filteredSlots = [];
  state.hasLoaded = false;
  state.filtersOptions = {
    types: [],
    locations: [],
    clinicians: [],
    dates: [],
  };
  state.selectedOptions = {
    type: '',
    location: '',
    clinician: '',
    date: 'this_week',
  };
};

const sortSlots = sortBy(slot => [
  slot.startTime,
]);

const createDate = (date = new Date()) => moment.tz(date, 'Europe/London');

const getDate = (rangeInDays = 0) => {
  const date = createDate();
  date.add(rangeInDays, 'days');
  date.set({ h: 0, m: 0, s: 0 });

  return date;
};

const mapToEndDate = (value) => {
  let rangeInDays;
  switch (value) {
    case 'today':
      rangeInDays = 0;
      break;
    case 'tomorrow':
      rangeInDays = 1;
      break;
    case 'next_week':
      rangeInDays = 14 - getDate().day();
      break;
    default:
      rangeInDays = 7 - getDate().day();
      break;
  }

  const date = getDate(rangeInDays);
  date.set({ h: 23, m: 59, s: 59 });

  return date;
};

const mapToStartDate = (value) => {
  let rangeInDays;
  switch (value) {
    case 'tomorrow':
      rangeInDays = 1;
      break;
    case 'next_week':
      rangeInDays = 8 - getDate().day();
      break;
    default:
      rangeInDays = 0;
      break;
  }

  return getDate(rangeInDays);
};

export default {
  [SELECT](state, slot) {
    slot.isSelected = true;
    state.selectedSlot = slot;
  },
  [DESELECT](state) {
    if (state.selectedSlot) {
      state.selectedSlot.isSelected = false;
    }

    state.selectedSlot = null;
  },
  [CLEAR](state) {
    clearState(state);
  },
  [INIT](state) {
    clearState(state);
    state.selectedSlot = null;
    state.booked = false;
  },
  [SET_SELECTED_OPTIONS](state, selectedOptions) {
    state.selectedOptions = selectedOptions;
  },
  [LOAD](state, data) {
    const sortedSlots = sortSlots(data.slots);
    const slots = new Map();
    const filters = new Map();
    const types = [];
    const locations = [];
    const clinicians = [];

    sortedSlots.forEach((slot) => {
      slot.isSelected = false;
      const startDate = createDate(slot.startTime).format('YYYY-MM-DD');
      if (slots.has(startDate)) {
        const slotCollection = slots.get(startDate);
        slotCollection.push(slot);
        slots.set(startDate, slotCollection);
      } else {
        slots.set(startDate, [slot]);
      }

      if (!filters.has(slot.type)) {
        filters.set(slot.type, true);
        types.push({ value: slot.type, name: slot.type, translate: false });
      }

      if (!filters.has(slot.location)) {
        filters.set(slot.location, true);
        locations.push({ value: slot.location, name: slot.location, translate: false });
      }

      slot.clinicians.forEach((clinician) => {
        if (!filters.has(clinician)) {
          filters.set(clinician, true);
          clinicians.push({ value: clinician, name: clinician, translate: false });
        }
      });
    });

    if (locations.length === 1) {
      state.selectedOptions.location = locations[0].value;
    }

    const dates = [
      { value: 'today', name: 'appointments.booking.filters.date.options.today', translate: true },
      { value: 'tomorrow', name: 'appointments.booking.filters.date.options.tomorrow', translate: true },
      { value: 'this_week', name: 'appointments.booking.filters.date.options.this_week', translate: true },
      { value: 'next_week', name: 'appointments.booking.filters.date.options.next_week', translate: true },
      { value: 'all', name: 'appointments.booking.filters.date.options.all', translate: true },
    ];

    types.sort();
    types.unshift({ value: '', name: 'appointments.booking.filters.type.default_option', translate: true });

    locations.sort();
    locations.unshift({ value: '', name: 'appointments.booking.filters.location.default_option', translate: true });

    clinicians.sort();
    clinicians.unshift({ value: '', name: 'appointments.booking.filters.clinician.default_option', translate: true });

    state.hasLoaded = true;
    state.slots = slots;
    state.filtersOptions = {
      types,
      locations,
      clinicians,
      dates,
    };
  },
  [FILTER](state) {
    const filteredSlots = new Map();
    if (state.selectedOptions.type === '' || state.selectedOptions.location === '') {
      state.filteredSlots = Array.from(filteredSlots);
      return;
    }

    state.slots.forEach((slots, startTime) => {
      const slotTime = createDate(startTime);
      if (state.selectedOptions.date !== 'all'
        && (
          slotTime.isBefore(mapToStartDate(state.selectedOptions.date), 'day')
          || mapToEndDate(state.selectedOptions.date).isBefore(slotTime, 'day')
        )
      ) {
        state.filteredSlots = Array.from(filteredSlots);
        return;
      }

      slots.forEach((slot) => {
        if (state.selectedOptions.type !== '' && state.selectedOptions.type !== slot.type) {
          return;
        }

        if (state.selectedOptions.location !== '' && state.selectedOptions.location !== slot.location) {
          return;
        }

        if (state.selectedOptions.clinician !== ''
          && Array.isArray(slot.clinicians)
          && slot.clinicians.indexOf(state.selectedOptions.clinician) === -1) {
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

    state.filteredSlots = Array.from(filteredSlots);
  },
};
