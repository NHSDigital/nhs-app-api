<template>
  <!--eslint-disable max-len -->
  <svg :class="$style.account" aria-hidden="true" xmlns="http://www.w3.org/2000/svg" width="20" height="16" viewBox="0 0 83.63 111.84">
    <defs/>
    <title>{{ $t('icons.clinicianIcon.title') }}</title>
    <path :class="$style['icon-account']" style="stroke: #979797; stroke-width: 8px;" d="M31.07,54.29s1.09,11.65-19.51,16.13C4.89,72.51,2,79.5,2,84.75v9.16c0,5.26,4.05,8.33,9.56,10.35a84.84,84.84,0,0,0,60.52,0c5.3-2.52,9.55-5.09,9.55-10.35V84.75c0-5.25-2.69-12.44-9.55-14.33C53.26,65.64,52.17,54.29,52.17,54.29"/>
    <rect :class="$style['icon-account']" style="stroke: #979797; stroke-width: 8px;" x="21.07" y="2" width="43.18" height="54.55" rx="21.59" ry="21.59"/>
  </svg>
</template>

<script>
// This is required for JEST tests.  If omitted it results in the error
//     'No script available to transform'
export default {};
</script>

<style module lang="scss" scoped>
@import "../../style/icons";
@import "../../style/colours";
svg {
  width: 20px;
  height: 16px;
}
</style>
