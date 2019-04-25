<template>
  <nav v-if="hasCrumbs && loggedIn" :class="$style['nhsuk-breadcrumb']"
       aria-label="Breadcrumb">
    <div :class="$style['nhsuk-width-container']">
      <ol :class="$style['nhsuk-breadcrumb__list']">
        <li v-for="(route, index) in routes" :key="index"
            :class="$style['nhsuk-breadcrumb__item']">
          <nuxt-link :class="$style['nhsuk-breadcrumb__link']" :to="route.path" tabindex="0" >
            {{ $t(`crumbName.${route.crumb.i8nKey}`) }}
          </nuxt-link>
        </li>
      </ol>
      <p :class="$style['nhsuk-breadcrumb__back']">
        <nuxt-link :class="$style['nhsuk-breadcrumb__backlink']"
                   :to="lastCrumb.path" tabindex="0" >
          {{ $t('crumbName.backTo', { crumbName: $t(`crumbName.${lastCrumb.crumb.i8nKey}`)}) }}
        </nuxt-link>
      </p>
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
  @import "../../style/fonts";
  @import '../../style/desktopcomponentsizes';
  @import "../../style/screensizes";
  @import '~nhsuk-frontend/packages/core/all.scss';
  @import '~nhsuk-frontend/packages/components/breadcrumb/_breadcrumb.scss';

  a {
    line-height: 1.5em;
    font-family: $nhsuk-font $nhsuk-font-fallback;
    font-weight: $nhsuk-font-normal;
    text-decoration: underline;
    display: inline-block;
  }

  a:hover {
    text-decoration: none;
  }

  @include tablet-and-above {
    .nhsuk-breadcrumb > div {
      margin: 0 2em;
      padding: 0 1em;
    }
  }

  @include desktop {
    .nhsuk-breadcrumb > div {
      margin: 0 auto;
      padding: 0 1em;
    }
  }
</style>
