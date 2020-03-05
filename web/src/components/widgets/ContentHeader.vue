<template>
  <div>
    <div :class="$style[isNative ? 'fix-breadcrumb' : '']">
      <bread-crumb-trail v-if="isBreadCrumbVisible" id="bread-crumb"
                         :routes="currentBreadCrumbs()"/>
      <yellow-banner v-if="showYellowBanner || showCoronaVirusBanner"
                     id="yellow-banner-line"
                     :class="$style['bannerLine']"/>
    </div>
    <div :class="[$style[isNative && isBreadCrumbVisible ? 'native-padding' : '']]">
      <corona-virus-banner v-if="showCoronaVirusBanner"/>
      <yellow-banner v-if="showYellowBanner" id="yellow-banner"
                     :class="[isNative ? $style['bannerLine-padding'] : $style['']]">
        <div v-if="showExternalServiceWarning" id="external-service-warning">
          <p class="nhsuk-u-padding-bottom-2 nhsuk-u-padding-top-2
              nhsuk-u-margin-bottom-0 nhsuk-body-s">
            <b>
              {{ $t('externalServiceWarning.warningText') }}
            </b>
          </p>
        </div>
        <a v-if="isProxying" id="acting-as-other-user-warning"
           :class="$style['banner']"
           role="link"
           tabindex="0"
           @click="proxyBannerClicked"
           @keypress.enter="proxyBannerClicked">
          <p class="nhsuk-u-padding-bottom-2 nhsuk-u-padding-top-2
              nhsuk-u-margin-bottom-0 nhsuk-body-s">
            {{ $t('linkedProfiles.actingAsOtherUserBannerWarningText') }}
            <b>{{ actingAsPersonName }}</b>
          </p>
        </a>
      </yellow-banner>

      <div :class="['nhsuk-width-container']">
        <div class="nhsuk-grid-row">
          <div id="page-title-container" class="nhsuk-grid-column-two-thirds">
            <page-title v-if="showContentHeader"
                        :title-key="$store.state.header.headerText"
                        :should-show-desktop-version="showHeader">
              {{ $store.state.header.headerText }}
            </page-title>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import BreadCrumbTrail from '@/components/widgets/BreadCrumbTrail';
import isEmpty from 'lodash/fp/isEmpty';
import PageTitle from '@/components/widgets/PageTitle';
import {
  findByName,
  APPOINTMENT_GP_ADVICE,
  APPOINTMENT_ADMIN_HELP,
  SWITCH_PROFILE,
  INDEX,
} from '@/lib/routes';
import YellowBanner from './YellowBanner';
import CoronaVirusBanner from './CoronaVirusBanner';

export default {
  name: 'ContentHeader',
  components: {
    YellowBanner,
    BreadCrumbTrail,
    PageTitle,
    CoronaVirusBanner,
  },
  props: {
    showBreadCrumb: {
      type: Boolean,
      default: true,
    },
    showContentHeader: {
      type: Boolean,
      default: true,
    },
  },
  computed: {
    isProxying() {
      return this.$store.getters['session/isProxying'];
    },
    isNative() {
      return this.$store.state.device.isNativeApp;
    },
    showHeader() {
      const store = this.$store;
      const isNativeVersionAfter = store.getters['appVersion/isNativeVersionAfter'];
      return !store.state.device.isNativeApp || isNativeVersionAfter('1.17.0');
    },
    showCoronaVirusBanner() {
      return !this.isProxying && this.$route.name === INDEX.name;
    },
    demographicsQuestionAnswered() {
      return this.$store.state.onlineConsultations.demographicsQuestionAnswered;
    },
    isBreadCrumbVisible() {
      return this.showBreadCrumb &&
        !isEmpty(this.currentBreadCrumbs());
    },
    showYellowBanner() {
      return this.showExternalServiceWarning ||
        (this.isProxying && this.$route.name !== SWITCH_PROFILE.name);
    },
    showExternalServiceWarning() {
      const route = findByName(this.$route.name);
      if (route === undefined) {
        return false;
      }
      if (route.path === APPOINTMENT_ADMIN_HELP.path || route.path === APPOINTMENT_GP_ADVICE.path) {
        return route.warningBanner && this.demographicsQuestionAnswered;
      }
      return route.warningBanner;
    },
    actingAsPersonName() {
      return this.$store.state.linkedAccounts.actingAsUser.name;
    },
    getProviderName() {
      const route = findByName(this.$route.name);
      if (route !== undefined) {
        if (route === 'appointments-admin-help') {
          return this.$store.state.onlineConsultations.adminProviderName;
        }
        return this.$store.state.onlineConsultations.adviceProviderName;
      }
      return '';
    },
  },
  methods: {
    proxyBannerClicked() {
      if (this.$route.name !== SWITCH_PROFILE.name) {
        this.goToUrl(SWITCH_PROFILE.path);
      }
    },
    currentBreadCrumbs() {
      const route = findByName(
        (this.$router.history !== undefined &&
          this.$router.history.pending !== undefined &&
          this.$router.history.pending !== null) ?
          this.$router.history.pending.name :
          this.$route.name,
      );
      const { crumbSetName } = this.$store.state.navigation;

      if (route === undefined) {
        return [];
      }

      if (route.crumb[crumbSetName] === undefined) {
        this.$store.dispatch('navigation/setRouteCrumb', 'defaultCrumb');
        return route.crumb.defaultCrumb;
      }
      return route.crumb[crumbSetName];
    },
  },
};
</script>

<style lang="scss">
  #page-title-container {
    margin-top: -1px;
    padding-top: 1px;
  }
</style>

<style module lang="scss" scoped>
  @import '../../style/colours';
  @import '../../style/screensizes';
  @import '../../style/textstyles';
  @import "../../style/fonts";
  @import "../../style/arrow";
  @import '../../style/accessibility';
  @import '../../style/desktopWeb/accessibility';

  .fix-breadcrumb {
    z-index: 4;
    position: fixed;
    width: 100%;
  }

  .bannerLine {
    height: 5px;
  }

  .bannerLine-padding {
    padding-top: 7px;
  }

  .native-padding {
    padding-top: 46px;
  }

  .banner {
    @include icon-arrow-left;
    color: $black;
    text-decoration: none;
    padding: 0 10px;
    margin: 0 -10px;
    background-position: right 10px center;

    &:hover {
      text-decoration: underline;
      box-shadow: none;
      cursor: pointer;
    }

    &:focus {
      @include focusStyleLightMenuItem;
      outline-color: $black;
      box-shadow: inset 0 0 0 4px $black;
      outline-offset: -5px;
    }
  }

  .focusedItem {
    &:focus {
      @include focusStyleLightMenuItem;
      color: #000;
    }
  }


</style>

