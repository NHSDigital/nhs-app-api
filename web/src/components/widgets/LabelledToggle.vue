<template>
  <div :class="$style.toggleAndLabel">
    <label :for="checkboxId" aria-hidden="true" @click.stop.prevent="onClick">{{ label }}</label>
    <toggle :value="value" :checkbox-id="checkboxId" :is-waiting="isWaiting" @input="onClick"/>
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
    isWaiting: {
      type: Boolean,
      default: false,
    },
    label: {
      type: String,
      required: true,
    },
    checkboxId: {
      type: String,
      default: 'default-id',
    },
    // eslint-disable-next-line vue/require-prop-types
    value: {
      default: '',
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
.toggleAndLabel {
  background: #fff;
  border-top: 1px #d8dde0 solid;
  border-bottom: 1px #d8dde0 solid;
  padding: 0.5em;
  margin: 0 0 1em;
  max-width: 540px;
  label {
    display: inline-block;
    width: calc(100% - 4em);
    vertical-align: middle;
  }
  &.waiting{
    .spinner{
      display:block;
    }
    .toggleWrapper label{
      &:before, &:after{
        display: none;
      }
    }
  }
}
</style>
