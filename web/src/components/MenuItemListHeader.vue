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
  @import "@/style/custom/menu-item-list-header";
</style>
