<template>
  <svg width="30px" height="30px" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 100 100"
       preserveAspectRatio="xMidYMid" class="lds-rolling" style="background: none;">
    <circle cx="50" cy="50" fill="none" stroke="#005eb8" stroke-width="8" r="35"
            stroke-dasharray="164.93361431346415 56.97787143782138"
            transform="rotate(335.886 50 50)">
      <animateTransform attributeName="transform" type="rotate" calcMode="linear"
                        values="0 50 50;360 50 50" keyTimes="0;1" dur="1.5s"
                        begin="0s" repeatCount="indefinite" />
    </circle>
  </svg>
</template>

<script>
export default {
  name: 'ToggleSpinner',
};
</script>

<style module lang="scss" scoped>

</style>
