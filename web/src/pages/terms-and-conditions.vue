<template>
  <div v-if="showTemplate" :class="!$store.state.device.isNativeApp && $style.desktopWeb">
    <div v-if="$store.state.device.isNativeApp"
         :class="[$style.webHeader, 'pull-content']">
      <header-slim :show-in-native="true">
        {{ pageHeader }}
      </header-slim>
      <updated-terms-conditions v-if="isUpdatedConsentRequired"/>
      <terms-conditions v-else/>
    </div>
    <div v-else>
      <div :class="$style['header-container-desktop']">
        <web-header :show-menu="false"
                    :show-links="false"
                    :show-header-buttons="false"/>
      </div>

      <div :class="$style['main-container-desktop']">
        <section>
          <div>
            <updated-terms-conditions v-if="isUpdatedConsentRequired"/>
            <terms-conditions v-else/>
          </div>
        </section>
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
  @import "../style/home";
  @import "../style/spacings";
  @import "../style/webshared";
  .webHeader {
    padding: 3.625em 0 3.125em 2.0px;
  }

  .header-container-desktop, .footer-container-desktop {
    order: 0;
    flex: 0 0 auto;
    align-self: stretch;
  }

  section {
    @include main-container-width;
    display: block;
    margin: 0 auto;
    padding: 0 1em 2.5em;

    > div {
      @include inner-container-width;
    }
  }

  @include tablet-and-above {
    section {
      margin: 0 2em;
    }
  }

  @include desktop {
    section  {
      margin: 0 auto;
    }
  }
</style>
