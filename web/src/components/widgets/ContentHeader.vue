<template>
  <div>
    <div :class="$style[fixBreadCrumb ? 'fix-breadcrumb' : '']">
      <bread-crumb-trail v-if="showBreadCrumb" id="bread-crumb"
                         :routes="currentBreadCrumbs"/>
      <yellow-banner v-if="showBanner"
                     id="yellow-banner-line"
                     :class="$style['bannerLine']"/>
    </div>
    <div :class="[$style[fixBreadCrumb ? 'native-padding' : '']]">
      <yellow-banner v-if="showBanner" id="yellow-banner">
        <p>
          {{ $t('externalServiceWarning.warningText') }}
        </p>
      </yellow-banner>
      <div :class="['nhsuk-width-container']">
        <div class="nhsuk-grid-row">
          <div class="nhsuk-grid-column-full">
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
  getCrumbTrailForRoute,
} from '@/lib/routes';
import YellowBanner from './YellowBanner';

export default {
  name: 'ContentHeader',
  components: {
    YellowBanner,
    BreadCrumbTrail,
    PageTitle,
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
    currentBreadCrumbs() {
      return getCrumbTrailForRoute(findByName(this.$route.name));
    },
    showHeader() {
      const store = this.$store;
      const isNativeVersionAfter = store.getters['appVersion/isNativeVersionAfter'];
      return !store.state.device.isNativeApp || isNativeVersionAfter('1.17.0');
    },
    demographicsQuestionAnswered() {
      return this.$store.state.onlineConsultations.demographicsQuestionAnswered;
    },
    fixBreadCrumb() {
      return this.showBreadCrumb &&
        !isEmpty(this.currentBreadCrumbs) && this.$store.state.device.isNativeApp;
    },
    showBanner() {
      const route = findByName(this.$route.name);
      if (route !== undefined) {
        if (route.name === 'appointments-admin-help') {
          return !!(route.warningBanner && this.demographicsQuestionAnswered);
        }
        return !!(route.warningBanner && true);
      }
      return false;
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
};
</script>

<style module lang="scss" scoped>
  @import '../../style/colours';
  @import '../../style/screensizes';
  @import '../../style/textstyles';
  @import "../../style/fonts";

  .fix-breadcrumb {
    z-index: 4;
    position: fixed;
    width: 100%;
  }

  .bannerLine {
   height: 5px;
  }

  .native-padding {
    padding-top: 48px;
  }
</style>

