<template>
  <div :class="[$style['info-message'], !$store.state.device.isNativeApp && $style.desktopWeb]"
       data-purpose="info-msg">
    <div :class="$style['info-header']"
         :aria-expanded="showContent ? 'true' : 'false'"
         role="button"
         tabindex="0"
         @click="toggle"
         @keypress.13="toggle">
      <plus-minus-icon :icon-plus="!showContent" />
      <p :class="$style['info-message-title']">
        <slot name="header" />
      </p>
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
import { key } from '@/lib/utils';

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
      if (event.key === key.Enter) {
        event.preventDefault();
        this.toggle();
      }
    },
  },
};
</script>

<style module lang="scss" scoped>
@import '../../style/collapsibledialog';
@import '../../style/desktopWeb/accessibility';

div.info-message {

  cursor: pointer;

  &.desktopWeb {
  .info-header {
    &:focus{
      @include outlineStyle;
    }
    &:hover{
      @include outlineStyleLight;
    }
  }
  }
}
</style>
