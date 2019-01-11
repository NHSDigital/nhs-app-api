<template>
  <div>
    <!-- bad indentation because of linting errors that cannot be disabled -->
    <noscript inline-template>
      <label :class="[$style.choice, $style.nojs]">
      <input :class="[$style.radio]" :value="value"
      :name="noJsName" type="radio" :checked="isSelected">
      <slot/>
      </label>
    </noscript>
    <div v-if="hasJs">
      <label :class="[$style.choice, isSelected ? $style.selected: '']"
             @click.prevent.stop="onLabelClick">
        <input :class="[$style.radio]" :value="value" :v-model="currentState" type="radio">
        <slot/>
      </label>
    </div>
  </div>
</template>

<script>
import get from 'lodash/fp/get';

export default {
  props: {
    action: {
      type: String,
      required: true,
    },
    state: {
      type: String,
      required: true,
    },
    /* eslint-disable-next-line vue/require-prop-types */
    value: {
      required: true,
    },
  },
  data() {
    return {
      hasJs: false,
    };
  },
  computed: {
    isSelected() {
      return this.currentState === this.value;
    },
    currentState() {
      return get(this.state)(this.$store.state);
    },
    noJsName() {
      return `nojs.${this.state}`;
    },
  },
  mounted() {
    this.hasJs = true;
  },
  methods: {
    onLabelClick() {
      this.$store.dispatch(this.action, this.value);
    },
  },
};
</script>

<style module lang="scss" scoped>
.choice {
  position: relative;
  margin-bottom: 1em;
  &.nojs {
    .radio {
      margin-right: 1em;
    }
  }
  &:not(.nojs) {
    padding: 0.4em 0 0.4em 2.5em;
    cursor: pointer;
    &.selected {
      &:after {
        zoom: 1;
        filter: alpha(opacity=100);
        opacity: 1;
      };
      &:before {
        box-shadow: 0 0 0 0.25em #F7E214;
      };
    };
    &:before {
      content: "";
      border: 0.15em solid currentColor;
      border-radius: 50%;
      background: #fff;
      width: 2em;
      height: 2em;
      position: absolute;
      top: 0;
      left: 0;
    };
    &:after {
      content: "";
      border-radius: 50%;
      background: black;
      width: 1em;
      height: 1em;
      position: absolute;
      top: 0.5em;
      left: 0.5em;
      zoom: 1;
      filter: alpha(opacity=0);
      opacity: 0;
    }
    .radio {
      position: absolute;
      overflow: hidden;
      clip: rect(0 0 0 0);
      height: 1px;
      width: 1px;
      margin: -1px;
      padding: 0;
      border: 0;
    }
  }
}
</style>
