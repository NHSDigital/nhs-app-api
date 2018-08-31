<template>
  <div :class="$style['info-message']" data-purpose="info-msg">
    <div :class="$style['info-header']" @click="toggle">
      <plus-minus-icon :icon-plus="!showContent" />
      <label :class="$style['info-message-title']">
        <slot name="header" />
      </label>
    </div>
    <div v-if="showContent" :class="$style['info-content']"
         data-purpose="info-content"><slot /></div>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import PlusMinusIcon from '@/components/icons/PlusMinusIcon';

export default {
  components: {
    PlusMinusIcon,
  },
  data() {
    return {
      showContent: false,
    };
  },
  methods: {
    toggle() {
      this.showContent = !this.showContent;
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/collapsibledialog";
</style>
