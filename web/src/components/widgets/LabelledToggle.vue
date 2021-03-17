<template>
  <div :class="$style.toggleAndLabel">
    <label class="nhsuk-u-font-size-19"
           :for="checkboxId"
           @click.stop.prevent="onClick">
      <strong>{{ label }}</strong>
      <template v-if="hintText">
        <br>
        <span class="nhsuk-body">{{ hintText }}</span>
      </template>
    </label>
    <toggle :value="value" :checkbox-id="checkboxId" :is-waiting="isWaiting"
            :name="checkboxId" @input="onClick"/>
  </div>
</template>

<script>
import Toggle from '@/components/widgets/Toggle';

export default {
  name: 'LabelledToggle',
  components: {
    Toggle,
  },
  props: {
    checkboxId: {
      type: String,
      default: 'default-id',
    },
    isWaiting: {
      type: Boolean,
      default: false,
    },
    label: {
      type: String,
      required: true,
    },
    // eslint-disable-next-line vue/require-prop-types
    value: {
      default: '',
    },
    hintText: {
      type: String,
      default: '',
    },
  },
  computed: {
    labelText() {
      return `${this.label} ${this.hintText}`;
    },
  },
  methods: {
    onClick() {
      if (!this.isWaiting) {
        this.$emit('input', !this.value);
      }
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "@/style/custom/labelled-toggle";
</style>
