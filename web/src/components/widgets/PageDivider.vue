<template>
  <div :class="[$style['page-divider'], 'nhsuk-u-margin-top-6']">
    <div :class="$style['page-divider__content']">
      <span :class="[$style['page-divider__inner'], 'nhsuk-u-font-size-19']">{{ text }}</span>
    </div>
  </div>
</template>

<script>
export default {
  name: 'PageDivider',
  props: {
    text: {
      type: String,
      required: true,
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "@/style/custom/page-divider";
</style>
