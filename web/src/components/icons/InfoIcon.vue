<template>
  <!--eslint-disable max-len -->
  <svg width="16px" height="16px" viewBox="0 0 16 16" version="1.1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" aria-hidden="true">
    <title>Icon - Info</title>
    <defs/>
    <g class="Icon---Info" stroke="none" stroke-width="1" fill="none" fill-rule="evenodd">
      <g :class="$style['info-icon--path']" class="info" fill-rule="nonzero">
        <path d="M8.00689061,16 C12.416882,16 16,12.4137931 16,8 C16,3.5862069 12.416882,0 8.00689061,0 C3.59689922,0 0,3.5862069 0,8 C0,12.4137931 3.59689922,16 8.00689061,16 Z M8.00689061,1.10344828 C11.796727,1.10344828 14.8837209,4.19310345 14.8837209,7.9862069 C14.8837209,11.7793103 11.796727,14.8689655 8.00689061,14.8689655 C4.21705426,14.8689655 1.13006029,11.7793103 1.13006029,7.9862069 C1.13006029,4.19310345 4.21705426,1.10344828 8.00689061,1.10344828 Z" />
        <path d="M6.58404558,11.8140496 C6.25641026,11.8140496 6,12.0743802 6,12.4070248 C6,12.7396694 6.25641026,13 6.58404558,13 L10.4159544,13 C10.7435897,13 11,12.7396694 11,12.4070248 C11,12.0743802 10.7435897,11.8140496 10.4159544,11.8140496 L9.07692308,11.8140496 L9.07692308,6.59297521 C9.07692308,6.26033058 8.82051282,6 8.49287749,6 L6.58404558,6 C6.25641026,6 6,6.26033058 6,6.59297521 C6,6.92561983 6.25641026,7.18595041 6.58404558,7.18595041 L7.92307692,7.18595041 L7.92307692,11.8140496 L6.58404558,11.8140496 Z" />
        <circle class="Oval" cx="8" cy="4" r="1"/>
      </g>
    </g>
  </svg>
</template>

<script>
// This is required for JEST tests.  If omitted it results in the error
//     'No script available to transform'
export default {};
</script>

<style module lang="scss" scoped>
@import "../../style/colours";
.info-icon--path {
  fill: $error
}
</style>
