<template>
  <generic-button id="switch-profile-button"
                  :button-classes="['nhsuk-button', 'nhsuk-button--primary']"
                  @click.stop.prevent="switchProfileButtonClicked">
    {{ $t('switchProfile.switchToMyProfileButton') }}
  </generic-button>
</template>

<script>
import GenericButton from '@/components/widgets/GenericButton';
import { redirectTo } from '@/lib/utils';
import { INDEX } from '@/lib/routes';

export default {
  name: 'SwitchProfileButton',
  components: {
    GenericButton,
  },
  methods: {
    async switchProfileButtonClicked() {
      const mainPatientId = this.$store.getters['linkedAccounts/mainPatientId'];
      await this.$store.dispatch('linkedAccounts/switchToMainUserProfile', { id: mainPatientId });
      await this.$store.dispatch('serviceJourneyRules/init');
      redirectTo(this, INDEX.path);
    },
  },
};

</script>

<style module lang="scss" scoped>
  @import '../../style/buttons';
</style>
