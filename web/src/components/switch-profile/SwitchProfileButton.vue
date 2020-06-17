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
import { INDEX_PATH } from '@/router/paths';

export default {
  name: 'SwitchProfileButton',
  components: {
    GenericButton,
  },
  methods: {
    async switchProfileButtonClicked() {
      await this.$store.dispatch('linkedAccounts/switchToMainUserProfile');
      await this.$store.dispatch('myRecord/clear');
      await this.$store.dispatch('serviceJourneyRules/init');
      redirectTo(this, INDEX_PATH);
    },
  },
};

</script>

<style module lang="scss" scoped>
  @import '../../style/buttons';
</style>
