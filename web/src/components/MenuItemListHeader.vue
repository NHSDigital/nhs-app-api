<template>
  <div :class="[headerCss, optionalHeaderCss]">
    <h2 class="nhsuk-heading-m" :aria-label="ariaText">{{ text }}</h2>
  </div>
</template>
<script>

export default {
  name: 'MenuItemListHeader',
  props: {
    id: {
      type: String,
      required: true,
    },
    text: {
      type: String,
      default: undefined,
    },
    tag: {
      type: String,
      default: 'a',
    },
    target: {
      type: String,
      default: undefined,
    },
    preventDefault: {
      type: Boolean,
      default: true,
    },
    headerTag: {
      type: String,
      default: 'span',
    },
    widenOnTablet: {
      type: Boolean,
      default: false,
    },
    widenOnDesktop: {
      type: Boolean,
      default: false,
    },
  },
  data() {
    const optionalHeaderCss = [];
    if (this.widenOnTablet) {
      optionalHeaderCss.push(this.$style['widen-tablet']);
    }
    if (this.widenOnDesktop) {
      optionalHeaderCss.push(this.$style['widen-desktop']);
    }
    return {
      headerCss: this.$style['nhs-app-panel-heading'],
      optionalHeaderCss,
    };
  },
  computed: {
    ariaText() {
      return this.text;
    },
  },
};
</script>
<style module lang="scss" scoped>
@import '~nhsuk-frontend/packages/core/tools/sass-mq';
  .widen-tablet {
    @include mq($until: tablet) {
      margin: 1em -1em 0;
    }
  }
  .widen-desktop {
    @include mq($until: desktop) {
      margin: 1em -1em 0;
    }
  }
  .nhs-app-panel-heading {
    margin-bottom: 0;
    margin-top: 1em;

    h1, h2, h3, h4, h5 {
      background-color: white;
      padding: 0.5em 16px 0.5em 16px;
      border-top: 1px solid #d8dde0;
      margin-bottom: 0;
    }
  }
</style>
