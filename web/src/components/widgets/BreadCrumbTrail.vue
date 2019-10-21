<template>
  <nav v-if="hasCrumbs && loggedIn" class="nhsuk-breadcrumb"
       aria-label="Breadcrumb">
    <div class="nhsuk-width-container">
      <div class="nhsuk-grid-row">
        <div class="nhsuk-grid-column-full">
          <ol class="nhsuk-breadcrumb__list">
            <li v-for="(route, index) in routes" :key="index"
                class="nhsuk-breadcrumb__item">
              <nuxt-link class="nhsuk-breadcrumb__link" :to="route.path" tabindex="0" >
                {{ $t(`crumbName.${route.crumb.i8nKey}`) }}
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
                                       { crumbName: $t(`crumbName.${lastCrumb.crumb.i8nKey}`)})">
              {{ $t('crumbName.backTo', { crumbName: $t(`crumbName.${lastCrumb.crumb.i8nKey}`)}) }}
            </nuxt-link>
          </p>
        </div>
      </div>
    </div>
  </nav>
</template>

<script>
import last from 'lodash/fp/last';
import isEmpty from 'lodash/fp/isEmpty';

export default {
  name: 'BreadCrumbTrail',
  props: {
    routes: {
      type: Array,
      default: () => [],
    },
  },
  computed: {
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

</style>
