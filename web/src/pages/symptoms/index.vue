<template>
  <main>
    <div>
      <symptoms-check/>
    </div>
  </main>
</template>

<script>
/* eslint-disable import/extensions */
import symptomsCheck from '@/components/symptoms/SymptomsCheck';

export default {
  components: {
    symptomsCheck,
  },
};
</script>

<style module lang="scss">
@import "../../style/html";
@import "../../style/fonts";
@import "../../style/buttons";
@import "../../style/elements";
</style>
