import each from 'jest-each';
import GPAppointments from '@/pages/appointments/gp-appointments';
import { mount, createStore } from '../../helpers';


describe('index.vue', () => {
  let wrapper;
  let $store;

  const mountPage = ({ status, userSessionCreateReferenceCode } = {}) => {
    $store = createStore({
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
    });

    return mount(GPAppointments, { $store, methods: { reload: jest.fn() } });
  };

  describe('errors', () => {
    each([
      400,
      403,
      500,
      502,
      504,
      599,
    ]).it('will display an error dialog for status code: %s', (status) => {
      wrapper = mountPage({ status });
      expect(wrapper.find(`#error-dialog-${status}`).exists()).toBe(true);
    });
  });
});
