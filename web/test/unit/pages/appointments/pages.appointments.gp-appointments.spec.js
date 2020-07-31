import each from 'jest-each';
import GPAppointments from '@/pages/appointments/gp-appointments';
import { mount } from '../../helpers';

const createAppointmentsPage = ({
  userSessionCreateReferenceCode,
  error = undefined,
} = {}) => mount(GPAppointments, {
  $store: {
    app: {
      $env: {},
    },
    state: {
      device: {},
      myAppointments: {
        error,
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
      wrapper = createAppointmentsPage({ error: { status } });
      expect(wrapper.find(`#error-dialog-${status}`).exists()).toBe(true);
    });
  });

  it('will return the session serviceDeskReference code', () => {
    wrapper = createAppointmentsPage({ userSessionCreateReferenceCode: 'code123' });
    expect(wrapper.vm.hasReferenceCode).toBe('code123');
  });

  it('will return the gp session response error serviceDeskReference code instead of session serviceDeskReference', () => {
    wrapper = createAppointmentsPage({ error: { status: 599, serviceDeskReference: 'show me instead' }, userSessionCreateReferenceCode: 'code123' });
    expect(wrapper.vm.hasReferenceCode).toBe('show me instead');
  });

  it('will return the gp session response error serviceDeskReference code when session serviceDeskReference is not present', () => {
    wrapper = createAppointmentsPage({ error: { status: 502, serviceDeskReference: 'show me please' } });
    expect(wrapper.vm.hasReferenceCode).toBe('show me please');
  });
});
