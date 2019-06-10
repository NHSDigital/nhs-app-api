<template>
  <div v-if="showTemplate">
    <orchestrator />
  </div>
</template>

<script>
import Orchestrator from '@/components/online-consultations/Orchestrator';
import { redirectTo } from '@/lib/utils';
import { INDEX } from '@/lib/routes';

export default {
  components: {
    Orchestrator,
  },
  async asyncData({ store, redirect }) {
    if (!(store.app.$env.ONLINE_CONSULTATIONS_ENABLED === 'true' || store.app.$env.ONLINE_CONSULTATIONS_ENABLED === true)) {
      redirect(302, INDEX.path, null);
    }
    if (!store.state.onlineConsultations.loaded) {
      await store.dispatch('onlineConsultations/clear');
      await store.dispatch('onlineConsultations/getAdminHelpServiceDefinitionId');
    }
  },
  created() {
    if (this.get('serviceDefinitionId') === undefined) {
      redirectTo(this, INDEX.path, null);
    }
    // todo: 5796 - empty parameters for now.
    this.evaluateServiceDefinition({});
  },
  methods: {
    onlineConsultationsEnabled() {
      return this.$store.app.$env.ONLINE_CONSULTATIONS_ENABLED === 'true'
             || this.$store.app.$env.ONLINE_CONSULTATIONS_ENABLED === true;
    },
    get(key) {
      return this.$store.state.onlineConsultations[key];
    },
    evaluateServiceDefinition(parameters) {
      return this.$store.dispatch('onlineConsultations/evaluateServiceDefinition', parameters);
    },
  },
};
</script>
