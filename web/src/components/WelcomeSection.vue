<template>
  <div data-sid="welcome-info" class="nhsuk-u-padding-top-3">
    <h1 ref="welcomePageHeader" tabindex="-1" class="nhsuk-u-margin-bottom-1 nhsuk-heading-l">
      {{ $t('home.indexPageHeader') }}
    </h1>
    <dl class="nhsuk-summary-list nhsuk-summary-list--no-border
               nhs-app-summary-list-inline nhsuk-u-margin-bottom-0">
      <div class="nhsuk-summary-list__row">
        <div data-sid="user-name" class="nhsuk-summary-list__value" data-hj-suppress >
          {{ fullNameLine }}
        </div>
      </div>
      <div v-if="nhsNumber" :class="[$style['info-key'], 'nhsuk-summary-list__row']">
        {{ $t('home.nhsNumber') }}:
        <generic-voice-over-text-split :text="nhsNumber" data-sid="user-nhs-number" />
      </div>
    </dl>
    <span aria-hidden="true" :class="$style['callout-arrow__solo']" />
  </div>
</template>
<script>
import GenericVoiceOverTextSplit from './widgets/GenericVoiceOverTextSplit';

export default {
  name: 'WelcomeSection',
  components: { GenericVoiceOverTextSplit },
  props: {
    displayName: {
      type: String,
      required: true,
    },
    nhsNumber: {
      type: String,
      default: '',
    },
  },
  computed: {
    fullNameLine() {
      return this.displayName.toUpperCase();
    },
  },
  updated() {
    this.focusWelcomePageHeader();
  },
  mounted() {
    this.focusWelcomePageHeader();
  },
  methods: {
    focusWelcomePageHeader() {
      if (document.activeElement !== null) {
        document.activeElement.blur();
      }
      if (this.$refs.welcomePageHeader) {
        this.$refs.welcomePageHeader.focus();
      }
    },
  },
};
</script>

<style lang="scss" module scoped>
  @import '@/style/_screensizes';
  @import "@/style/custom/callout-arrow";
  @import "@/style/custom/welcome-section";
</style>
