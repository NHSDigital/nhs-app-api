<template>
  <div :class="[$style['info-message'], !$store.state.device.isNativeApp && $style.desktopWeb]"
       data-purpose="info-msg">
    <div :class="$style['info-header']"
         :aria-expanded="showContent ? 'true' : 'false'"
         role="button"
         tabindex="0"
         @click="toggle"
         @keypress.enter.prevent="toggle">
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
  },
};
</script>

<style module lang="scss" scoped>
@import '../../style/collapsibledialog';
@import '../../style/desktopWeb/accessibility';

div.info-message {
  cursor: pointer;

  .info-header {
    &:focus{
      @include outlineStyle;
    }
  }
  &.desktopWeb {
    .info-header {
      &:hover{
        @include outlineStyleLight;
      }
      h2 {
        font-family: $default-web;
        font-weight: normal;
      }
    }
  }
}
</style>
