<template>
  <div v-if="showTemplate" :class="!$store.state.device.isNativeApp && $style.desktopWeb">
    <div v-if="$store.state.device.isNativeApp"
         :class="[$style.webHeader, 'pull-content']">
      <header-slim :show-in-native="true" />
      <updated-terms-conditions v-if="isUpdatedConsentRequired"/>
      <terms-conditions v-else/>
    </div>
    <div v-else>
      <div :class="$style['header-container-desktop']" class="nhsuk-width-container--full">
        <web-header :show-menu="false"
                    :show-links="false"
                    :show-header-buttons="false"/>
      </div>

      <div id="mainContent" ref="mainContent" tabindex="-1"
           class="nhsuk-width-container nhsuk-width-container--full"
           :class="$style.mainContent">
        <div class="nhsuk-grid-row">
          <main class="nhsuk-main-wrapper nhsuk-main-wrapper--no-padding nhsuk-homepage">
            <div class="nhsuk-grid-column-three-quarters">
              <updated-terms-conditions v-if="isUpdatedConsentRequired"/>
              <terms-conditions v-else/>
            </div>
          </main>
        </div>
      </div>
      <div v-if="!this.$store.state.device.isNativeApp"
           :class="$style['footer-container-desktop']">
        <web-footer/>
      </div>
    </div>
  </div>
</template>
<script>
/* eslint-disable import/extensions */
import HeaderSlim from '@/components/HeaderSlim';
import TermsConditions from '@/components/TermsConditions';
import UpdatedTermsConditions from '@/components/UpdatedTermsConditions';
import WebHeader from '@/components/widgets/WebHeader';
import WebFooter from '@/components/widgets/WebFooter';

export default {
  layout: 'termsAndConditions',
  components: {
    HeaderSlim,
    TermsConditions,
    UpdatedTermsConditions,
    WebHeader,
    WebFooter,
  },
  data() {
    return {
      areAccepted: this.$store.state.termsAndConditions.areAccepted,
      updatedConsentRequired: this.$store.state.termsAndConditions.updatedConsentRequired,
    };
  },
  computed: {
    pageHeader() {
      if ((this.areAccepted)
        && (this.updatedConsentRequired)) {
        return this.$t('updatedTermsAndConditions.title');
      }
      return this.$t('termsAndConditions.title');
    },
    isUpdatedConsentRequired() {
      return ((this.areAccepted)
        && (this.updatedConsentRequired));
    },
  },
  mounted() {
    if (this.$store.state.termsAndConditions.updatedConsentRequired) {
      this.$store.dispatch('pageTitle/updatePageTitle', this.$t('updatedTermsAndConditions.title'));
      if (process.client) {
        window.document.title = `${this.$t('updatedTermsAndConditions.title')} - ${this.$t('appTitle')}`;
      }
    }
  },
};
</script>

<style lang="scss">
  @import "../style/main";
  @import "../style/pulltorefresh";
  @import "../style/elements";
</style>

<style module lang="scss" scoped>
  @import "../style/spacings";
  .webHeader {
    padding: 3.625em 0 3.125em 2.0px;
  }

  .header-container-desktop, .footer-container-desktop {
    order: 0;
    flex: 0 0 auto;
    align-self: stretch;
  }

  section {
    display: block;
    padding: 0 1em 2.5em;
  }

  .mainContent {
    outline: none;
  }


</style>
