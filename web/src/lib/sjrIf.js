export default ({ $store, journey, disabled = false, context = null }) => {
  if (context != null) {
    return !disabled === $store.getters[`serviceJourneyRules/${journey}Enabled`](context);
  }
  return !disabled === $store.getters[`serviceJourneyRules/${journey}Enabled`];
};
