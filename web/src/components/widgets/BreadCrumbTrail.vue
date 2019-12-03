<template>
  <nav v-if="hasCrumbs && loggedIn"
       id="bread-crumb"
       class="nhsuk-breadcrumb"
       aria-label="Breadcrumb">
    <div class="nhsuk-width-container">
      <div class="nhsuk-grid-row">
        <div class="nhsuk-grid-column-full">
          <div v-if="!$store.state.device.isNativeApp" id="navbar-breadcrumb">
            <ol class="nhsuk-breadcrumb__list">
              <li v-for="(route, index) in routes" :key="index"
                  class="nhsuk-breadcrumb__item">
                <nuxt-link class="nhsuk-breadcrumb__link" :to="route.path" tabindex="0" >
                  {{ $t(`crumbName.${route.crumb.i18nKey}`) }}
                </nuxt-link>
              </li>
            </ol>
            <p class="nhsuk-breadcrumb__back">
              <nuxt-link :class="['nhsuk-breadcrumb__backlink',
                                  $store.state.device.isNativeApp && $style.native
                         ]"
                         :to="lastCrumb.path"
                         tabindex="0"
                         :aria-label="$t('crumbName.backTo',
                                         { crumbName: $t(`crumbName.${lastCrumb.crumb.i18nKey}`)})">
                {{ $t('crumbName.backTo',
                      { crumbName: $t(`crumbName.${lastCrumb.crumb.i18nKey}`)}) }}
              </nuxt-link>
            </p>
          </div>
          <div v-else>
            <span class="nhsuk-breadcrumb__back" :class="$style['native-back']" >
              <a id="native-back-breadcrumb"
                 class="nhsuk-breadcrumb__backlink"
                 :class="$style['native'] "
                 @click.prevent="backLinkClicked()" >
                <span v-if="isProxyPage">Back to Home</span>
                <span v-else>Back</span>
              </a>
            </span>
          </div>
        </div>
      </div>
    </div>
  </nav>
</template>

<script>
import last from 'lodash/fp/last';
import isEmpty from 'lodash/fp/isEmpty';
import { navigateBack } from '@/lib/utils';
import { ORGAN_DONATION, MORE, ORGAN_DONATION_VIEW_DECISION, SWITCH_PROFILE, INDEX } from '@/lib/routes';

export default {
  name: 'BreadCrumbTrail',
  props: {
    routes: {
      type: Array,
      default: () => [],
    },
  },
  computed: {
    isProxyPage() {
      return this.$route.name === SWITCH_PROFILE.name;
    },
    lastCrumb() {
      return last(this.routes);
    },
    loggedIn() {
      return !!this.$store.state.session.csrfToken;
    },
    hasCrumbs() {
      return !isEmpty(this.routes);
    },
  },
  methods: {
    backLinkClicked() {
      if (this.$route.name === ORGAN_DONATION.name ||
          this.$route.name === ORGAN_DONATION_VIEW_DECISION.name) {
        // eslint-disable-next-line prefer-destructuring
        let backLinkOverride = this.$store.state.navigation.backLinkOverride;

        if (!backLinkOverride) {
          backLinkOverride = MORE.path;
        }

        this.goToUrl(backLinkOverride);
      } else if (this.$route.name === SWITCH_PROFILE.name) {
        this.goToUrl(INDEX.path);
      } else {
        navigateBack(this);
      }
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "../../style/colours";
  @import "../../style/desktopWeb/accessibility";

  a {
    line-height: 1.5em;
    text-decoration: underline;
    display: inline-block;

    &:focus {
      @include linkFocusStyle;
    }

    &:hover {
      @include linkHoverStyle;
    }

    &.native {
      &:hover {
      background: none;
      box-shadow: none;
      color: $nhs_blue;
      }
    }
  }

  .native-back {
    display: block;
  }

</style>
