<template>
  <div :class="!$store.state.device.isNativeApp && $style.desktopWeb">
    <span v-if="error && errorText" :id="errorId" class="nhsuk-error-message">
      <span class="nhsuk-u-visually-hidden">{{ $t('generic.errorPrefix') }}</span>
      {{ errorText }}
    </span>
    <div class="nhsuk-date-input__item">
      <div class="nhsuk-form-group">
        <label class="nhsuk-label" :for="id">
          {{ $t('generic.file') }}
        </label>
        <input :id="id"
               type="file"
               :name="name"
               :accept="accepts"
               :required="required"
               @change="onSelectedFileChange($event)">
      </div>
    </div>
  </div>
</template>

<script>

export default {
  name: 'GenericAttachment',
  props: {
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
      default: '',
    },
    id: {
      type: String,
      default: undefined,
    },
    accept: {
      type: Array,
      default: () => [],
    },
  },
  computed: {
    errorId() {
      return this.id ? `${this.id}-error-message` : 'error-message';
    },
    accepts() {
      return this.accept.join(', ');
    },
  },
  methods: {
    onSelectedFileChange(event) {
      this.$emit('change', event);
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "@/style/custom/div-inline-block";
</style>
