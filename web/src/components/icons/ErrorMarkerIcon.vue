<template>
  <!--eslint-disable max-len -->
  <svg width="16px" height="16px" viewBox="0 0 16 16" version="1.1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink"
       style="margin-bottom: -2px;">
    <title>error-marker</title>
    <defs/>
    <g id="Page-1" stroke="none" stroke-width="1" fill="none" fill-rule="evenodd">
      <g id="errors-v4" transform="translate(-16.000000, -666.000000)">
        <g id="error-marker" transform="translate(16.000000, 666.000000)">
          <circle id="Oval-2" :class="[!$store.state.device.isNativeApp ? $style.desktopErrorIcon: $style['error-circle']]" cx="8" cy="8" r="8"/>
          <path id="!" d="M8.791,10.13 L7.195,10.13 L6.929,3.228 L9.085,3.228 L8.791,
          10.13 Z M8.987,13 L7.027,13 L7.027,11.04 L8.987,11.04 L8.987,13 Z" fill="#FFFFFF"/>
        </g>
      </g>
    </g>
  </svg>
</template>

<script>
// This is required for JEST tests.  If omitted it results in the error
//     'No script available to transform'
export default {
};
</script>

<style module lang="scss" scoped>
@import "../../style/colours";
.error-circle {
  fill: $error_dark
}

 .desktopErrorIcon{
  fill: red
 }
</style>
