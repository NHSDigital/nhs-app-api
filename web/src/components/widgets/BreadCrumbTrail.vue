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
                 tabindex="0"
                 :class="$style['native'] "
                 @keypress.enter.prevent="backClicked"
                 @click.prevent="backClicked" >
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
import NativeApp from '@/services/native-app';
import { EventBus, FOCUS_NHSAPP_ROOT } from '@/services/event-bus';
import {
  SWITCH_PROFILE,
  backLinkOverrides,
} from '@/lib/routes';

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
    olcBack() {
      return this.$store.state.onlineConsultations.previousQuestion !== undefined;
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
    backClicked() {
      if (this.olcBack) {
        this.goToPreviousQuestion();
        return;
      }

      this.backLinkClicked();
    },
    backLinkClicked() {
      const override = backLinkOverrides[this.$route.name];
      if (override) {
        const defaultOverride = override.defaultPath;
        const storeOverride = this.$store.state.navigation.backLinkOverride || defaultOverride;

        this.goToUrl(override.ignoreStore ? defaultOverride : storeOverride);
      } else {
        navigateBack(this);
      }
    },
    async goToPreviousQuestion() {
      const { provider, serviceDefinitionId } = this.$store.state.onlineConsultations.journeyInfo;
      document.activeElement.blur();

      await this.$store.dispatch('onlineConsultations/setPrevious');
      await this.$store.dispatch('onlineConsultations/evaluateServiceDefinition', {
        provider,
        serviceDefinitionId,
      });

      if (this.isNativeApp) {
        NativeApp.resetPageFocus();
      } else {
        EventBus.$emit(FOCUS_NHSAPP_ROOT);
      }
      window.scrollTo(0, 0);
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
