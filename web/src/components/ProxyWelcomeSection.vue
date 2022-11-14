<template>
  <div data-sid="welcome-info-proxy" class="nhsuk-u-padding-top-3">
    <h1 ref="proxyPageHeader" tabindex="-1" class="nhsuk-u-margin-bottom-1 nhsuk-heading-l">
      {{ $t('home.indexPageHeader') }}
    </h1>
    <dl class="nhsuk-summary-list nhsuk-summary-list--no-border
               nhs-app-summary-list-inline nhsuk-u-margin-bottom-0">
      <div class="nhsuk-summary-list__row">
        <div data-sid="proxy-name" class="nhsuk-summary-list__value" data-hj-suppress>
          {{ displayName }}
        </div>
      </div>
      <div v-if="proxyAge" class="nhsuk-summary-list__row">
        <div data-sid="proxy-user-age" class="nhsuk-summary-list__value" data-hj-suppress>
          {{ proxyAge }}
        </div>
      </div>
      <div class="nhsuk-summary-list__row">
        <div data-sid="proxy-user-surgery" class="nhsuk-summary-list__value" data-hj-suppress>
          {{ proxyDetails.gpPracticeName }}
        </div>
      </div>
    </dl>
    <span aria-hidden="true" :class="$style['callout-arrow__solo']" />
  </div>
</template>

<script>
export default {
  name: 'ProxyWelcomeSection',
  props: {
    proxyDetails: {
      type: Object,
      required: true,
    },
    proxyAge: {
      type: String,
      default: '',
    },
    displayName: {
      type: String,
      required: true,
    },
  },
  updated() {
    this.focusProxyPageHeader();
  },
  mounted() {
    this.focusProxyPageHeader();
  },
  methods: {
    focusProxyPageHeader() {
      if (document.activeElement !== null) {
        document.activeElement.blur();
      }
      if (this.$refs.proxyPageHeader) {
        this.$refs.proxyPageHeader.focus();
      }
    },
  },
};
</script>

<style lang="scss" module scoped>
  @import '@/style/custom/callout-arrow';
  @import '@/style/_screensizes';
  @import "@/style/custom/welcome-section";
</style>
