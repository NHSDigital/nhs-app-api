const sjrIf = ({ $store, journey }) => $store.getters[`serviceJourneyRules/${journey}Enabled`];

export default sjrIf;
