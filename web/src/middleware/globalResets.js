export default ({ to, store, next }) => {
  store.dispatch('http/cancelRequests');
  store.dispatch('flashMessage/clear');
  store.dispatch('errors/setRoutePath', to);

  // clear errors
  store.dispatch('myAppointments/clearError');
  store.dispatch('availableAppointments/clearError');

  return next();
};
