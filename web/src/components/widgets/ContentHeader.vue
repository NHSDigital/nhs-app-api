<template>
  <div>
    <div :class="$style[isNative ? 'fix-breadcrumb' : '']">
      <bread-crumb-trail v-if="isBreadCrumbVisible" id="bread-crumb"
                         :crumbs="currentBreadCrumbs()"/>
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
            <strong>
              {{ $t('externalServiceWarning.warningText') }}
            </strong>
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
            <strong>{{ actingAsPersonName }}</strong>
          </p>
        </a>
      </yellow-banner>

      <div :class="['nhsuk-width-container']">
        <div class="nhsuk-grid-row">
          <div id="page-title-container" class="nhsuk-grid-column-two-thirds">
            <page-title v-if="(showContentHeader || overrideShowContentHeader)
                          && hasHeaderOrCaption"
                        :caption="caption"
                        :caption-size="captionSize"
                        :title-key="`${header}${caption}`">
              {{ header }}
            </page-title>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import get from 'lodash/fp/get';
import isEmpty from 'lodash/fp/isEmpty';
import BreadCrumbTrail from '@/components/widgets/BreadCrumbTrail';
import PageTitle from '@/components/widgets/PageTitle';
import { SWITCH_PROFILE_PATH } from '@/router/paths';
import {
  SWITCH_PROFILE_NAME,
  INDEX_NAME,
  APPOINTMENT_ADMIN_HELP_NAME,
  APPOINTMENT_GP_ADVICE_NAME,
} from '@/router/names';
import OnUpdateHeaderMixin from '@/plugins/mixinDefinitions/OnUpdateHeaderMixin';
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
  mixins: [OnUpdateHeaderMixin],
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
  data() {
    return {
      header: '',
      caption: '',
      captionSize: '',
      overrideShowContentHeader: false,
    };
  },
  computed: {
    isProxying() {
      return this.$store.getters['session/isProxying'];
    },
    isNative() {
      return this.$store.state.device.isNativeApp;
    },
    hasHeaderOrCaption() {
      return !isEmpty(this.header) || !isEmpty(this.caption);
    },
    showCoronaVirusBanner() {
      return !this.isProxying && this.$route.name === INDEX_NAME;
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
        (this.isProxying && this.$route.name !== SWITCH_PROFILE_NAME);
    },
    showExternalServiceWarning() {
      const warningBanner = get('$route.meta.warningBanner', this);
      const routeName = this.$route.name;
      if (routeName === APPOINTMENT_ADMIN_HELP_NAME
        || routeName === APPOINTMENT_GP_ADVICE_NAME) {
        return warningBanner && this.demographicsQuestionAnswered;
      }
      return warningBanner;
    },
    actingAsPersonName() {
      return this.$store.state.linkedAccounts.actingAsUser.fullName;
    },
  },
  methods: {
    proxyBannerClicked() {
      if (this.$route.name !== SWITCH_PROFILE_NAME) {
        this.goToUrl(SWITCH_PROFILE_PATH);
      }
    },
    currentBreadCrumbs() {
      const { crumbSetName } = this.$store.state.navigation;
      if (this.$route.meta.crumb[crumbSetName] === undefined) {
        this.$store.dispatch('navigation/setRouteCrumb', 'defaultCrumb');
        return this.$route.meta.crumb.defaultCrumb;
      }

      return this.$route.meta.crumb[crumbSetName];
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

