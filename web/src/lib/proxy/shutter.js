import get from 'lodash/fp/get';
import { GP_APPOINTMENTS_NAME } from '@/router/names';
import { redirectTo } from '@/lib/utils';

const isForbiddenApiError = ({ $store }) => $store.getters['errors/showApiError']
    && get('state.errors.apiErrors[0].status')($store) === 403;

const isAppointmentsForbiddenError = (currentRoute, { $store }) =>
  currentRoute.name === GP_APPOINTMENTS_NAME
    && get('error.status')($store.state.myAppointments) === 403;

export default (currentRoute, self) => {
  if (self.$store.getters['session/isProxying']
      && get('proxyShutterPath')(currentRoute.meta)
      && (isForbiddenApiError(self) || isAppointmentsForbiddenError(currentRoute, self))) {
    self.$store.dispatch('errors/clearAllApiErrors');
    redirectTo(self, currentRoute.meta.proxyShutterPath);
  }
};
