<template>
  <span :class="$style['header-link']">
    <router-link v-if="anchorInternal && anchorAction"
                 :id="anchorId"
                 :to="anchorValue"
                 tabindex="0"
                 onclick="anchorAction()">
      {{ anchorName }}
    </router-link>
    <router-link v-else-if="anchorInternal && !anchorAction"
                 :id="anchorId"
                 :to="anchorValue"
                 tabindex="0">
      {{ anchorName }}
    </router-link>
    <a v-else :id="anchorId" :href="anchorValue" target="_blank"
       rel="noopener noreferrer">
      {{ anchorName }}
    </a>
  </span>
</template>

<script>
export default {
  name: 'HeaderLink',
  props: {
    anchorId: {
      type: String,
      required: true,
    },
    anchorName: {
      type: String,
      required: true,
    },
    anchorValue: {
      type: String,
      required: true,
    },
    anchorAction: {
      type: Function,
      default: undefined,
    },
    anchorInternal: {
      type: Boolean,
      default: false,
    },
  },
};
</script>
<style module lang="scss" scoped>
  @import "@/style/custom/header-link";
</style>
