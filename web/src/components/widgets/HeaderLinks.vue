<template>
  <div v-if="anchorLinks.length > 0" :class="$style['header-links']">
    <span v-for="(anchorLink, index) in anchorLinks" :key="index">
      <router-link v-if="anchorLink.internal"
                   :id="anchorLink.id"
                   :to="anchorLink.value"
                   tabindex="0">
        {{ anchorLink.name }}
      </router-link>
      <a v-else :id="anchorLink.id" :href="anchorLink.value" target="_blank"
         rel="noopener noreferrer">
        {{ anchorLink.name }}
      </a>
      <strong v-if="index < anchorLinks.length - 1" aria-hidden="true">
        &nbsp;|&nbsp;
      </strong>
    </span>
  </div>
</template>

<script>
export default {
  name: 'HeaderLinks',
  props: {
    anchorLinks: {
      type: Array,
      default: () => [],
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "@/style/custom/header-links";
</style>
