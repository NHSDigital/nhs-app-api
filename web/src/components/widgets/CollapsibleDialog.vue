<template>
  <div :class="[$style['info-message'], !$store.state.device.isNativeApp && $style.desktopWeb]"
       data-purpose="info-msg">
    <div :class="$style['info-header']"
         :aria-expanded="showContent ? 'true' : 'false'"
         role="button"
         tabindex="0"
         @click="toggle"
         @keypress="keyPress($event)">
      <plus-minus-icon :icon-plus="!showContent" />
      <h2 :class="$style['info-message-title']">
        <slot name="header" />
      </h2>
    </div>
    <div v-if="showContent" :class="$style['info-content']"
         data-purpose="info-content">
      <slot />
    </div>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import PlusMinusIcon from '@/components/icons/PlusMinusIcon';

const ENTER_KEY_CODE = 13;

export default {
  name: 'CollapsibleDialog',
  components: {
    PlusMinusIcon,
  },
  data() {
    return {
      showContent: true,
    };
  },
  mounted() {
    if (process.client) {
      this.showContent = !this.showContent;
    }
  },
  methods: {
    toggle() {
      this.showContent = !this.showContent;
    },
    keyPress(event) {
      if (event.keyCode === ENTER_KEY_CODE) {
        event.preventDefault();
        this.toggle();
      }
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/collapsibledialog";

div.info-message {
 &.desktopWeb {
  .info-header {
   h2 {
    font-family: $default-web;
    font-weight: normal;
   }
  }
  }
}
</style>
