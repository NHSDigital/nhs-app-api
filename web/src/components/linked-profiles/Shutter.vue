<template>
  <div>
    <p v-if="subHeader" id="shutter-subheader-text"
       class="nhsuk-u-margin-bottom-3">
      <strong>{{ subHeader }}</strong>
    </p>
    <p v-if="summaryText" id="shutter-summary-text"
       :aria-label="summaryText">
      {{ summaryText }}
    </p>
    <template v-if="showMedicalAdvice">
      <contact-111 id="shutter-medical-advice-text"
                   :text="medicalAdviceText" :aria-label="medicalAdviceLabel"/>
    </template>
    <p v-if="coronaVirusLinkText">
      <analytics-tracked-tag
        :href="coronaConditionsUrl"
        :text="coronaVirusLinkText"
        :aria-label="coronaVirusLinkText"
        tag="a" target="_blank">
        {{ coronaVirusLinkText }}
      </analytics-tracked-tag>
    </p>
    <p v-if="switchText"
       id="shutter-switch-text"
       class="nhsuk-u-margin-top-3 nhsuk-u-margin-bottom-3">
      {{ switchText }}
    </p>
    <generic-button
      id="btn-switch-profile"
      :button-classes="['nhsuk-button', 'nhsuk-u-margin-top-3']"
      @click.stop.prevent="switchProfileButtonClicked">
      {{ $t('profiles.switchToMyProfile') }}
    </generic-button>
  </div>
</template>

<script>
import { INDEX_PATH } from '@/router/paths';
import { redirectTo } from '@/lib/utils';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import Contact111 from '@/components/widgets/Contact111';
import GenericButton from '@/components/widgets/GenericButton';
import get from 'lodash/fp/get';

export default {
  name: 'Shutter',
  components: {
    AnalyticsTrackedTag,
    Contact111,
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
      coronaConditionsUrl: this.$store.$env.CORONA_CONDITIONS_URL,
      coronaVirusLinkText: '',
      givenName: get('$store.state.linkedAccounts.actingAsUser.givenName')(this),
      subHeader: '',
      summaryText: '',
      switchText: '',
      showMedicalAdvice: false,
      medicalAdviceText: '',
      medicalAdviceLabel: '',
    };
  },
  mounted() {
    const featureName = this.feature;

    if (this.$te(`profiles.shutter.${featureName}.subHeader`)) {
      this.subHeader = this.$t(`profiles.shutter.${featureName}.subHeader`)
        .replace('{name}', this.givenName);
    }

    if (this.$te(`profiles.shutter.${featureName}.summary`)) {
      this.summaryText = this.$t(`profiles.shutter.${featureName}.summary`)
        .replace('{name}', this.givenName);
    }

    if (this.$te(`profiles.shutter.${featureName}.switch`)) {
      this.switchText = this.$t(`profiles.shutter.${featureName}.switch`);
    }

    if (this.$te(`profiles.shutter.${featureName}.coronaVirus.link`)) {
      this.coronaVirusLinkText = this.$t(`profiles.shutter.${featureName}.coronaVirus.link`);
    }

    if (this.$te(`profiles.shutter.${featureName}.forUrgentMedicalAdvice`)) {
      this.showMedicalAdvice = true;
      this.medicalAdviceText = this.$t(`profiles.shutter.${featureName}.forUrgentMedicalAdvice.text`);
      this.medicalAdviceLabel = this.$t(`profiles.shutter.${featureName}.forUrgentMedicalAdvice.label`);
    }
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
