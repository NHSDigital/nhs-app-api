export default ({ $store, journey, disabled = false }) => !disabled === $store.getters[`serviceJourneyRules/${journey}Enabled`];
