<template>
  <div :class="$style.toggleAndLabel">
    <label class="nhsuk-u-font-size-19"
           aria-hidden="true"
           :for="checkboxId"
           @click.stop.prevent="onClick">
      {{ label }}
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
@import '~nhsuk-frontend/packages/core/settings/breakpoints';
@import '~nhsuk-frontend/packages/core/settings/colours';
@import '~nhsuk-frontend/packages/core/settings/globals';
@import '~nhsuk-frontend/packages/core/settings/spacing';
@import '~nhsuk-frontend/packages/core/tools/spacing';
@import '~nhsuk-frontend/packages/core/tools/sass-mq';
@import '~nhsuk-frontend/packages/core/tools/ifff';

.toggleAndLabel {
  @include nhsuk-responsive-padding(2, "top");
  @include nhsuk-responsive-padding(3, "left");
  @include nhsuk-responsive-padding(2, "bottom");
  @include nhsuk-responsive-padding(2, "right");
  @include nhsuk-responsive-margin(4, "bottom");
  background: $color_nhsuk-white;
  border-top: 1px $color_nhsuk-grey-4 solid;
  border-bottom: 1px $color_nhsuk-grey-4 solid;
  @include govuk-media-query($until: desktop) {
    padding-left: $nhsuk-gutter-half;
    margin-left: (-$nhsuk-gutter-half);
    margin-right: (-$nhsuk-gutter-half);
  }
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
