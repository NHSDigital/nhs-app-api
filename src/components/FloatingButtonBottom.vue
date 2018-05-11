<template>
  <div :class="stylingClass">
    <!--eslint-disable-next-line -->
    <button class="button" id="btn_floating" @click="$emit('on-click')" :disabled="isButtonDisabled">
      <slot/>
    </button>
  </div>
</template>

<script>

export default {
  props: {
    clickable: {
      default: true,
      type: Boolean,
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

</style>
