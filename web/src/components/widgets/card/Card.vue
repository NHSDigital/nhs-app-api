<template>
  <component :is="component"
             :class="$style['nhsuk-card']"
             @click="$event => $emit('click', $event)">
    <slot/>
  </component>
</template>

<script>
export default {
  name: 'Card',
  props: {
    component: {
      type: String,
      default: 'div',
      validator: value => ['div', 'a'].includes(value),
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "@/style/custom/widgets-card";
</style>
