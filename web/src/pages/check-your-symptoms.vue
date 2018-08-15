<template>
  <div :class="[$style['slim-no-padding'], 'pull-content']">
    <header-slim/>
    <body>
      <div>
        <symptoms-check/>
      </div>
    </body>
  </div>
</template>
<script>
/* eslint-disable import/extensions */
import HeaderSlim from '@/components/HeaderSlim';
import symptomsCheck from '@/components/symptoms/SymptomsCheck';

export default {
  components: {
    HeaderSlim,
    symptomsCheck,
  },
};
</script>
<style module lang="scss" scoped>
.slim-no-padding {
    margin-top: -3.625em;
}
</style>
