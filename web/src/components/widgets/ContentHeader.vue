<template>
  <div>
    <bread-crumb-trail v-if="showBreadCrumb" id="bread-crumb"
                       :class="$style[fixBreadCrumb ? 'fix-breadcrumb' : '']"
                       :routes="currentBreadCrumbs" />
    <div :class="['nhsuk-width-container', $style[fixBreadCrumb ? 'native-padding' : '']]">
      <div class="nhsuk-grid-row">
        <div class="nhsuk-grid-column-full">
          <page-title v-if="showContentHeader"
                      :should-show-desktop-version="showHeader"/>
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

export default {
  name: 'ContentHeader',
  components: {
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
    fixBreadCrumb() {
      return !isEmpty(this.currentBreadCrumbs) && this.$store.state.device.isNativeApp;
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

  .native-padding {
    padding-top: 48px;
  }
</style>

