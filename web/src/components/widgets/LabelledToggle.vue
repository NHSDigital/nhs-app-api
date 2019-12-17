<template>
  <div :class="$style.toggleAndLabel">
    <label class="nhsuk-u-font-size-19"
           aria-hidden="true"
           :aria-label="labelText"
           :for="checkboxId"
           @click.stop.prevent="onClick">
      {{ label }}
      <span v-if="hintText !== ''" class="nhsuk-body"><br>{{ hintText }}</span>
    </label>
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
 @import '~nhsuk-frontend/packages/core/tools/mixins';
  @import '~nhsuk-frontend/packages/core/tools/spacing';
  @import '~nhsuk-frontend/packages/core/settings/spacing';
  @import '~nhsuk-frontend/packages/core/tools/sass-mq';
.toggleAndLabel {
  background: #fff;
  border-top: 1px #d8dde0 solid;
  border-bottom: 1px #d8dde0 solid;
  padding: 1em;
  margin: 5px -16px 1em;
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
  p {
    display: inline-block;
    width: calc(100% - 4em);
    vertical-align: middle;
  }

  @include govuk-media-query('tablet') {
    margin-left: 0;
    margin-right: 0;
  }
}
</style>
