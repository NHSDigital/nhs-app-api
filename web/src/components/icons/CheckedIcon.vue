<template>
  <div :class="$style.checkbox">
    <svg v-if="selected" :class="$style.checked" xmlns="http://www.w3.org/2000/svg" width="16" height="12" viewBox="0 0 16 12">
      <defs><style>.efcdf5ce-b03e-4827-8f05-4c309bf09cb0{fill:#0059c1;}</style></defs>
      <title>Checked</title>
    <!--eslint-disable-next-line -->
      <g id="f7c68065-00a3-4d59-834a-ff2bd7c26316" data-name="Layer 2"><g id="a5d0b063-5411-4fcd-a539-45751e5ffae7" data-name="Layer 1"><path class="efcdf5ce-b03e-4827-8f05-4c309bf09cb0" d="M.18,6.6a.58.58,0,0,1,0-.84l.87-.84a.6.6,0,0,1,.86,0L2,5,5.35,8.52a.3.3,0,0,0,.43,0L14,.18h.06a.6.6,0,0,1,.86,0l.87.84a.58.58,0,0,1,0,.84L6,11.82a.57.57,0,0,1-.43.18.55.55,0,0,1-.43-.18l-4.8-5Z"></path></g></g></svg>
  </div>
</template>

<script>
export default {
  props: {
    selected: {
      type: Boolean,
      default: false,
    },
    id: {
      type: String,
      default: '',
    },
  },
  data() {
    return {
      show: !this.hidden,
    };
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/colours";

  .checkbox {
    margin-right: 1em;
    flex: 0 0 1.5em;
    border: 0.063em $light_grey solid;
    width: 1.5em;
    height: 1.5em;
    border-radius: 0.313em;
    padding-bottom: 0.5em;
    &:active {
      border-color: $black;
    }
    .checked {
      margin-left: 0.250em;
      margin-top: 0.375em;
    }
  }
  label {
    padding-top: 0.125em;
  }
</style>
