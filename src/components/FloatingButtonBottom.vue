<template>
  <div :class="stylingClass">
    <!--eslint-disable-next-line -->
    <button :class="buttonStylingClasses" id="btn_floating" @click="$emit('on-click')" :disabled="isButtonDisabled">
      <slot/>
    </button>
  </div>
</template>

<script>

export default {
  props: {
    clickable: {
      type: Boolean,
      default: true,
    },
    buttonClasses: {
      type: Array,
      default: () => [],
    },
  },
  data() {
    return {
      stylingClass: 'button-container',
    };
  },
  computed: {
    isButtonDisabled() {
      return !this.clickable;
    },

    buttonStylingClasses() {
      const classes = ['button'].concat(this.buttonClasses);
      if (!this.clickable) {
        classes.push('disabled');
      }
      return classes;
    },
  },
  created() {
    if (this.$store.state.device.isNativeApp) {
      this.stylingClass = 'button-container-native';
    }
  },
};

</script>
<style lang="scss" scoped>
  @import '../style/buttons';

  button.button,
  a.button { 
    margin-bottom: 0;
  }

</style>
