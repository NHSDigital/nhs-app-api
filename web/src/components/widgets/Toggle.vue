<template>
  <div :class="{ [$style.toggleWrapper]: true, [$style.waiting]: isWaiting }">
    <toggle-spinner :id="`${checkboxId}_spinner`" v-visible="isWaiting" :class="$style.spinner" />
    <input :id="checkboxId"
           :class="$style.toggle"
           type="checkbox"
           :name="name"
           :checked="value"
           role="switch"
           @click.stop.prevent="onClick">
    <span :id="`span${checkboxId}`" v-visible="!isWaiting"
          @click.stop.prevent="onClick"/>
  </div>
</template>

<script>
import ToggleSpinner from '@/components/icons/ToggleSpinner';

export default {
  name: 'Toggle',
  components: {
    ToggleSpinner,
  },
  props: {
    name: {
      type: String,
      default: '',
    },
    checkboxId: {
      type: String,
      default: 'default-id',
    },
    isWaiting: {
      type: Boolean,
      default: false,
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
@import '../../style/colours';

@mixin beforeAnimation {
  transition: 0.2s cubic-bezier(0.24, 0, 0.5, 1);
}

@mixin afterAnimation {
  box-shadow: 0 0 0 1px hsla(0, 0%, 0%, 0.1), 0 4px 0px 0 hsla(0, 0%, 0%, 0.04),
    0 4px 9px hsla(0, 0%, 0%, 0.13), 0 3px 3px hsla(0, 0%, 0%, 0.05);
  transition: 0.35s cubic-bezier(0.54, 1.6, 0.5, 1);
}
@mixin inactiveMixin {
  content: "";
  position: absolute;
  display: block;
}

// Mobile Toggle Switch
.toggleWrapper {
  display: inline-block;
  position: relative;
  .spinner {
    background: none;
    position: absolute;
    display: block;
    z-index: 1;
    padding: 0 0.875em;
    width: auto;
    height: 2em;
    opacity: 0;
    transition: visibility 0s, opacity 0.5s linear;
  }
  input.toggle {
    opacity: 0; // hides checkbox
    position: absolute;
    z-index: -1;
    & + span {
      position: relative;
      display: inline-block;
      user-select: none;
      transition: 0.4s ease;
      height: 2.05em;
      width: 3.5em;
      border: 1px solid #e4e4e4;
      border-radius: 4em;
      vertical-align: middle;
      &::before {
        @include inactiveMixin;
        @include beforeAnimation;
        height: 2em;
        width: 3.4em;
        top: 0;
        left: 0;
        border-radius: 2em;
      }
      &::after {
        @include inactiveMixin;
        @include afterAnimation;
        background: $background;
        height: calc(2em - 2px);
        width: calc(2em - 2px);
        top: 1px;
        left: 0px;
        border-radius: 4em;
      }
    }
    // When Active
    &:checked {
      & + span::before {
        background: $light_green;
        transition: width 0.2s cubic-bezier(0, 0, 0, 0.1);
      }
      & + span::after {
        left: 1.6em;
      }
    }
  }

  &.waiting {
    .spinner {
      opacity: 1;
    }
    input.toggle + span {
      opacity: 0;
      &::before,
      &::after {
        visibility: hidden;
        opacity: 0;
      }
    }
  }
}
</style>
