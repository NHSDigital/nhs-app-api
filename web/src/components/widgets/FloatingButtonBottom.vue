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
  name: 'FloatingButtonBottom',
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
  @import "@/style/custom/floating-button-bottom";
</style>
