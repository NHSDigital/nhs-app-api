import get from 'lodash/fp/get';
import { APPOINTMENTS } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';

const isForbiddenApiError = ({ $store }) => $store.getters['errors/showApiError']
    && get('state.errors.apiErrors[0].status')($store) === 403;

const isAppointmentsForbiddenError = (currentRoute, { $store }) =>
  currentRoute.name === APPOINTMENTS.name
    && get('error.status')($store.state.myAppointments) === 403;

export default (currentRoute, self) => {
  if (self.$store.getters['session/isProxying']
      && get('proxyShutterPath')(currentRoute)
      && (isForbiddenApiError(self) || isAppointmentsForbiddenError(currentRoute, self))) {
    self.$store.dispatch('errors/clearAllApiErrors');
    redirectTo(self, currentRoute.proxyShutterPath);
  }
};
