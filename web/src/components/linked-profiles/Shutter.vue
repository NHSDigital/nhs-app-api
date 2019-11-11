<template>
  <div>
    <p v-if="subHeader" id="shutter-subheader-text"
       class="nhsuk-u-margin-bottom-3">
      <b>{{ subHeader }}</b>
    </p>
    <p v-if="summaryText" id="shutter-summary-text">{{ summaryText }}</p>
    <p v-if="switchText"
       id="shutter-switch-text"
       class="nhsuk-u-margin-top-3 nhsuk-u-margin-bottom-3">
      {{ switchText }}
    </p>
    <generic-button
      id="btn-switch-profile"
      :button-classes="['nhsuk-button', 'nhsuk-u-margin-top-3']"
      @click.stop.prevent="switchProfileButtonClicked">
      {{ $t('linkedProfiles.switchToMyProfileButton') }}
    </generic-button>
  </div>
</template>

<script>
import { INDEX } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';
import GenericButton from '@/components/widgets/GenericButton';
import get from 'lodash/fp/get';

export default {
  name: 'Shutter',
  layout: 'nhsuk-layout',
  components: {
    GenericButton,
  },
  props: {
    feature: {
      type: String,
      required: true,
      default: null,
    },
  },
  data() {
    return {
      subHeader: '',
      summaryText: '',
      switchText: '',
      givenName: get('$store.state.linkedAccounts.actingAsUser.givenName')(this),
    };
  },
  mounted() {
    const featureName = this.feature;

    if (this.$te(`linkedProfiles.shutter.${featureName}.subHeader`)) {
      this.subHeader = this.$t(`linkedProfiles.shutter.${featureName}.subHeader`)
        .replace('{name}', this.givenName);
    }

    if (this.$te(`linkedProfiles.shutter.${featureName}.summary`)) {
      this.summaryText = this.$t(`linkedProfiles.shutter.${featureName}.summary`)
        .replace('{name}', this.givenName);
    }

    if (this.$te(`linkedProfiles.shutter.${featureName}.switch`)) {
      this.switchText = this.$t(`linkedProfiles.shutter.${featureName}.switch`);
    }
  },
  methods: {
    async switchProfileButtonClicked() {
      const mainPatientId = this.$store.getters['linkedAccounts/mainPatientId'];
      const mainUserObject = {
        id: mainPatientId,
      };
      await this.$store.dispatch('linkedAccounts/switchToMainUserProfile', mainUserObject);
      redirectTo(this, INDEX.path);
    },
  },
};
</script>
<style module lang="scss" scoped>
</style>
