import { assign, find, get, map, mapKeys, sortBy } from 'lodash/fp';

export const SLOT_SELECTED = 'SLOT_SELECTED';
export const SLOTS_LOADED = 'SLOTS_LOADED';
export const INIT_APPOINTMENTS = 'INIT_APPOINTMENTS';

const findById = (id, collection) => find(item => item.id === id)(collection);
const findByIds = (ids, collection) => map(id => findById(id, collection))(ids);
const sortSlots = sortBy(slot => [
  slot.startTime,
  get('clinicians[0].displayName')(slot),
]);

const initialState = {
  appointmentSessions: [],
  clinicians: [],
  locations: [],
  slots: [],
  hasLoaded: false,
  hasErrored: false,
  selectedSlotId: undefined,
};
export const state = () => initialState;

export const actions = {
  getAppointmentsSlotsParameters: () => {
    const getToDate = () => {
      const rangeInDays = 14;

      const date = new Date();
      date.setDate(date.getDate() + rangeInDays);
      date.setUTCHours(0, 0, 0, 0);

      return date;
    };
    return {
      fromDate: new Date().toISOString(),
      toDate: getToDate().toISOString(),
    };
  },
  init({ commit }) {
    commit(INIT_APPOINTMENTS);
  },
  load({ commit }) {
    const getFromDate = () => new Date();
    const getToDate = () => {
      const rangeInDays = 14;

      const date = getFromDate();
      date.setDate(date.getDate() + rangeInDays);
      date.setUTCHours(0, 0, 0, 0);

      return date;
    };
    const appointmentSlotParmas = {
      fromDate: getFromDate().toISOString(),
      toDate: getToDate().toISOString(),
    };
    return this.app.$http
      .getV1PatientAppointmentSlots(appointmentSlotParmas)
      .then((data) => {
        commit(SLOTS_LOADED, data);
      });
  },
  select({ commit }, slotId) {
    commit(SLOT_SELECTED, slotId);
  },
};
/* eslint-disable no-shadow */
/* eslint-disable no-param-reassign */
/* eslint-disable no-unused-vars */
export const mutations = {
  [SLOT_SELECTED](state, slotId) {
    state.selectedSlotId = slotId;
    state.slots = state.slots.map((slot) => {
      slot.selected = slot.id === slotId;
      return slot;
    });
  },
  [INIT_APPOINTMENTS](state) {
    state = initialState;
  },
  [SLOTS_LOADED](state, data) {
    mapKeys((key) => {
      state[key] = data[key];
    })(data);

    state.slots = map((slot) => {
      const result = assign({}, slot);
      const location = findById(slot.locationId, state.locations);
      if (location) {
        result.location = location;
      }

      const appointmentSession = findById(slot.locationId, state.locations);
      if (appointmentSession) {
        result.appointmentSession = appointmentSession;
      }

      const clinicians = findByIds(slot.clinicianIds, state.clinicians);
      if (clinicians && clinicians.length > 0) {
        result.clinicians = clinicians;
      }

      result.selected = false;
      return result;
    })(state.slots);

    state.slots = sortSlots(state.slots);
    state.hasLoaded = true;
  },
};
/* eslint-disable no-shadow */
export const getters = {
  findById(id, collection) {
    find(item => item.id === id)(collection);
  },
  findByIds(ids, collection) {
    map(id => findById(id, collection))(ids);
  },
  slots(state) {
    return state.slots.map((slot) => {
      const result = assign({}, slot);

      result.location = findById(slot.locationId, state.locations);
      result.appointmentSession = findById(
        slot.appointmentSessionId,
        state.appointmentSessions,
      );
      result.clinicians = findByIds(slot.clinicianIds, state.clinicians);

      return result;
    });
  },
  isSelected(state) {
    return id => id === state.selectedSlotId;
  },
  currentSlot(state) {
    return state.slots.find(slot => slot.id === state.selectedSlotId);
  },
};
