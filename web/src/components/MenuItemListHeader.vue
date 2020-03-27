<template>
  <div :class="$style['nhs-app-panel-heading']">
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

  .nhs-app-panel-heading {
    margin-bottom: 0;
    margin-top: 1em;

    @include mq($until: desktop) {
      margin: 1em -1em 0;
    }

    h1, h2, h3, h4, h5 {
      background-color: white;
      padding: 0.5em 16px 0.5em 16px;
      border-top: 1px solid #d8dde0;
      margin-bottom: 0;
    }
  }
</style>
