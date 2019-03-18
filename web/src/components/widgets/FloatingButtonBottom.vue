<template>
  <div :class="stylingClass">
    <generic-button :id="id" :button-classes="buttonStylingClasses"
                    :style="{ 'margin-bottom': 0}"
                    :disabled="isButtonDisabled"
                    @click="$emit('click', $event)">
      <slot/>
    </generic-button>
  </div>
</template>

<script>

import GenericButton from '@/components/widgets/GenericButton';

export default {
  components: {
    GenericButton,
  },
  props: {
    clickable: {
      type: Boolean,
      default: true,
    },
    buttonClasses: {
      type: Array,
      default: () => [],
    },
    id: {
      type: String,
      default: '',
    },
  },
  data() {
    return {
      stylingClass: [`${this.$style['float-button-container']}`],
    };
  },
  computed: {
    isButtonDisabled() {
      return !this.clickable;
    },

    buttonStylingClasses() {
      const classes = this.buttonClasses;
      classes.push('button');
      if (!this.clickable) {
        classes.push('disabled');
      }
      return classes;
    },
  },
  created() {
    if (this.$store.state.device.isNativeApp) {
      this.stylingClass.push(this.$style['button-container-native']);
    }
  },
};
</script>

<style lang="scss" scoped module>
@import "../../style/colours";
.float-button-container {
  position: fixed;
  bottom: 4.250em;
  left: 0em;
  right: 0em;
  background-color: $white;
  box-sizing: border-box;
  padding: 1em;
  border-top: 0.063em $background solid;
  border-bottom: 0.063em $background solid;
  z-index: 4;
  box-shadow: 0em -0.100em 0.200em rgba(0, 0, 0, .1);
  &.button-container-native {
    bottom: 0;
    word-wrap:break-word;
  }
}

</style>
