<template>
  <div :class="!$store.state.device.isNativeApp && $style.desktopWeb">
    <span v-if="error && errorText" :id="errorId" class="nhsuk-error-message">
      <span class="nhsuk-u-visually-hidden">{{ $t('generic.errorPrefix') }}</span>
      {{ errorText }}
    </span>
    <img :id="id"
         :src="source"
         @click="onImageClicked($event)">
  </div>
</template>

<script>
export default {
  name: 'GenericImageInput',
  props: {
    id: {
      type: String,
      default: undefined,
    },
    source: {
      type: String,
      default: undefined,
    },
    name: {
      type: String,
      default: undefined,
    },
    required: {
      type: Boolean,
      default: true,
    },
    error: {
      type: Boolean,
      default: false,
    },
    errorText: {
      type: String,
      default: undefined,
    },
  },
  data() {
    return {
      clickedPositionValue: undefined,
    };
  },
  computed: {
    errorId() {
      return this.id ? `${this.id}-error-message` : 'error-message';
    },
  },
  methods: {
    onImageClicked(event) {
      this.$emit('input', event);
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "@/style/custom/generic-image-input";
</style>
