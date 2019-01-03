<template>
  <div :class="bannerClass">
    <span data-purpose="beta-banner">
      <div :class="dynamicStyle('betaLabel')">BETA</div>
      <p :class="$style.betaLine">This is a new service.</p>
    </span>
  </div>
</template>

<script>
import { getDynamicStyle } from '@/lib/desktop-experience';

export default {
  name: 'BetaBanner',
  props: {
    bannerClass: {
      type: Array,
      default: () => [],
    },
  },
  methods: {
    dynamicStyle(...args) {
      return getDynamicStyle(this, args);
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "../style/colours";
  @import "../style/fonts";

  @mixin betaLabel-core {
    color: $white;
    background: #005EB8;
    font-weight: 700;
    font-size: 1em;
    line-height: 1.5em;
    display: inline-block;
    margin-right: 0.6em;
    margin-top: -0.3em;
    padding: 0.3em 0.6em;
  }

  .betaLabel {
    @include betaLabel-core;
    margin-left: -0.3em;
  }

  .betaLabel-desktop {
    @include betaLabel-core;
    margin-left: 0;
  }

  .betaLine {
    display: inline-block;
  }
</style>
