import each from 'jest-each';
import GPAppointments from '@/pages/appointments/gp-appointments';
import { mount } from '../../helpers';

const createAppointmentsPage = ({
  status,
  userSessionCreateReferenceCode,
} = {}) => mount(GPAppointments, {
  $store: {
    state: {
      device: {},
      myAppointments: {
        error: {
          status,
        },
      },
      session: {
        userSessionCreateReferenceCode,
      },
    },
    dispatch: jest.fn(),
  },
  methods: {
    reload: jest.fn(),
  },
});

describe('index.vue', () => {
  let wrapper;
  describe('errors', () => {
    each([
      400,
      403,
      500,
      502,
      504,
    ]).it('will display an error dialog for status code: %s', (status) => {
      wrapper = createAppointmentsPage({ status });
      expect(wrapper.find(`#error-dialog-${status}`).exists()).toBe(true);
    });
  });

  it('will return the session serviceDeskReference code', () => {
    wrapper = createAppointmentsPage({ userSessionCreateReferenceCode: 'code123' });
    expect(wrapper.vm.hasReferenceCode).toBe('code123');
  });
});
