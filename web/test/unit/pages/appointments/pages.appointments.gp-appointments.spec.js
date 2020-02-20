import each from 'jest-each';
import GPAppointments from '@/pages/appointments/gp-appointments';
import { createStore, mount } from '../../helpers';

const createAppointmentsPage = ({ $store }) => mount(GPAppointments, {
  $store,
  methods: {
    reload: jest.fn(),
  },
});

describe('index.vue', () => {
  let state;
  let wrapper;

  beforeEach(() => {
    state = {
      device: {},
      myAppointments: {
        error: null,
      },
    };
    const $store = createStore({ state });
    wrapper = createAppointmentsPage({ $store });
  });

  describe('errors', () => {
    each([
      400,
      403,
      500,
      502,
      504,
    ]).it('will display an error dialog for status code: %s', (status) => {
      state.myAppointments.error = { status };
      expect(wrapper.find(`#error-dialog-${status}`).exists()).toBe(true);
    });
  });
});
