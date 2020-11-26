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
              <li v-for="(crumb, index) in crumbs" :key="index"
                  class="nhsuk-breadcrumb__item">
                <router-link class="nhsuk-breadcrumb__link"
                             :to="createRouteLink(crumb.name)" tabindex="0" >
                  {{ $t(`navigation.crumbName.${crumb.i18nKey}`) }}
                </router-link>
              </li>
            </ol>
            <p class="nhsuk-breadcrumb__back">
              <router-link :class="['nhsuk-breadcrumb__backlink',
                                    $store.state.device.isNativeApp && $style.native]"
                           :to="createRouteLink(lastCrumb.name)"
                           tabindex="0"
                           :aria-label="$t(
                             'navigation.crumbName.backTo',
                             { crumbName: $t(`navigation.crumbName.${lastCrumb.i18nKey}`)})">
                {{ $t('navigation.crumbName.backTo',
                      { crumbName: $t(`navigation.crumbName.${lastCrumb.i18nKey}`)}) }}
              </router-link>
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
import { navigateBack, createRouteByNameObject } from '@/lib/utils';
import { EventBus, FOCUS_NHSAPP_TITLE } from '@/services/event-bus';
import backLinkOverrides from '@/router/backLinkOverRides';
import { SWITCH_PROFILE_NAME } from '@/router/names';

export default {
  name: 'BreadCrumbTrail',
  props: {
    crumbs: {
      type: Array,
      default: () => [],
    },
  },
  computed: {
    isProxyPage() {
      return this.$route.name === SWITCH_PROFILE_NAME;
    },
    olcBack() {
      return this.$store.state.onlineConsultations.previousQuestion !== undefined;
    },
    lastCrumb() {
      return last(this.crumbs);
    },
    loggedIn() {
      return !!this.$store.state.session.csrfToken;
    },
    hasCrumbs() {
      return !isEmpty(this.crumbs);
    },
  },
  methods: {
    createRouteLink(name) {
      return createRouteByNameObject({ name, params: {}, store: this.$store });
    },
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

      EventBus.$emit(FOCUS_NHSAPP_TITLE);
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
      display: grid;
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
