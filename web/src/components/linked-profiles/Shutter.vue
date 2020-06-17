<template>
  <div>
    <p v-if="subHeader" id="shutter-subheader-text"
       class="nhsuk-u-margin-bottom-3">
      <strong>{{ subHeader }}</strong>
    </p>
    <p v-if="summaryText"
       id="shutter-summary-text"
       :aria-label="summaryLabelText">{{ summaryText }}</p>
    <h2 v-if="coronaVirusHeaderText">{{ coronaVirusHeaderText }}</h2>
    <p v-if="coronaVirusBodyText">{{ coronaVirusBodyText }}</p>
    <p v-if="coronaVirusLinkText">
      <analytics-tracked-tag
        :href="coronaServiceUrl"
        :text="coronaVirusLinkText"
        :aria-label="coronaVirusLinkLabelText"
        tag="a" target="_blank">
        {{ coronaVirusLinkText }}
      </analytics-tracked-tag>
    </p>
    <hr v-if="coronaVirusHeaderText"
        class="nhsuk-u-margin-top-3 nhsuk-u-margin-bottom-1"
        aria-hidden="true">
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
import { INDEX_PATH } from '@/router/paths';
import { redirectTo } from '@/lib/utils';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import GenericButton from '@/components/widgets/GenericButton';
import get from 'lodash/fp/get';
import {
  CORONA_SERVICE_URL,
} from '@/router/externalLinks';

export default {
  name: 'Shutter',
  components: {
    AnalyticsTrackedTag,
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
      coronaServiceUrl: CORONA_SERVICE_URL,
      coronaVirusBodyText: '',
      coronaVirusHeaderText: '',
      coronaVirusLinkLabelText: undefined,
      coronaVirusLinkText: '',
      givenName: get('$store.state.linkedAccounts.actingAsUser.givenName')(this),
      subHeader: '',
      summaryText: '',
      summaryLabelText: undefined,
      switchText: '',
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

    if (this.$te(`linkedProfiles.shutter.${featureName}.summaryLabel`)) {
      this.summaryLabelText = this.$t(`linkedProfiles.shutter.${featureName}.summaryLabel`)
        .replace('{name}', this.givenName);
    }

    if (this.$te(`linkedProfiles.shutter.${featureName}.switch`)) {
      this.switchText = this.$t(`linkedProfiles.shutter.${featureName}.switch`);
    }

    if (this.$te(`linkedProfiles.shutter.${featureName}.coronaVirus.header`)) {
      this.coronaVirusHeaderText = this.$t(`linkedProfiles.shutter.${featureName}.coronaVirus.header`)
        .replace('{name}', this.givenName);
    }

    if (this.$te(`linkedProfiles.shutter.${featureName}.coronaVirus.body`)) {
      this.coronaVirusBodyText = this.$t(`linkedProfiles.shutter.${featureName}.coronaVirus.body`);
    }

    if (this.$te(`linkedProfiles.shutter.${featureName}.coronaVirus.link`)) {
      this.coronaVirusLinkText = this.$t(`linkedProfiles.shutter.${featureName}.coronaVirus.link`);
    }

    if (this.$te(`linkedProfiles.shutter.${featureName}.coronaVirus.linkLabel`)) {
      this.coronaVirusLinkLabelText = this.$t(`linkedProfiles.shutter.${featureName}.coronaVirus.linkLabel`);
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
