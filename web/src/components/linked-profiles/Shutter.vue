<template>
  <div>
    <p v-if="subHeader" id="shutter-subheader-text"
       class="nhsuk-u-margin-bottom-3">
      <strong>{{ subHeader }}</strong>
    </p>
    <p v-if="summaryText" id="shutter-summary-text"
       :aria-label="summaryLabelText">
      {{ summaryText }}
      <template v-if="postSummaryLinks">
        <a href="https://111.nhs.uk" target="_blank" rel="noopener noreferrer"
           style="display:inline">{{ oneOneOneLinkText }}</a>
        {{ orCallText }}
      </template>
    </p>
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
      {{ $t('profiles.switchToMyProfile') }}
    </generic-button>
  </div>
</template>

<script>
import { INDEX_PATH } from '@/router/paths';
import { redirectTo } from '@/lib/utils';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import GenericButton from '@/components/widgets/GenericButton';
import get from 'lodash/fp/get';

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
      coronaServiceUrl: this.$store.$env.CORONA_SERVICE_URL,
      coronaVirusBodyText: '',
      coronaVirusHeaderText: '',
      coronaVirusLinkLabelText: undefined,
      coronaVirusLinkText: '',
      givenName: get('$store.state.linkedAccounts.actingAsUser.givenName')(this),
      subHeader: '',
      summaryText: '',
      summaryLabelText: undefined,
      switchText: '',
      oneOneOneLinkText: '',
      orCallText: '',
      postSummaryLinks: false,
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

    if (this.$te(`profiles.shutter.${featureName}.summaryLabel`)) {
      this.summaryLabelText = this.$t(`profiles.shutter.${featureName}.summaryLabel`)
        .replace('{name}', this.givenName);
    }

    if (this.$te(`profiles.shutter.${featureName}.switch`)) {
      this.switchText = this.$t(`profiles.shutter.${featureName}.switch`);
    }

    if (this.$te(`profiles.shutter.${featureName}.coronaVirus.header`)) {
      this.coronaVirusHeaderText = this.$t(`profiles.shutter.${featureName}.coronaVirus.header`)
        .replace('{name}', this.givenName);
    }

    if (this.$te(`profiles.shutter.${featureName}.coronaVirus.body`)) {
      this.coronaVirusBodyText = this.$t(`profiles.shutter.${featureName}.coronaVirus.body`);
    }

    if (this.$te(`profiles.shutter.${featureName}.coronaVirus.link`)) {
      this.coronaVirusLinkText = this.$t(`profiles.shutter.${featureName}.coronaVirus.link`);
    }

    if (this.$te(`profiles.shutter.${featureName}.coronaVirus.linkLabel`)) {
      this.coronaVirusLinkLabelText = this.$t(`profiles.shutter.${featureName}.coronaVirus.linkLabel`);
    }

    if (this.$te(`profiles.shutter.${featureName}.postSummaryLinks`)) {
      this.postSummaryLinks = true;
      this.oneOneOneLinkText = this.$t(`profiles.shutter.${featureName}.postSummaryLinks.nhs111Link`);
      this.orCallText = this.$t(`profiles.shutter.${featureName}.postSummaryLinks.call111`);
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
