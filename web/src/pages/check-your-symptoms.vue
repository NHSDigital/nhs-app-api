<template>
  <div :class="[$style['slim-no-padding'], 'pull-content', getHeaderState()]">
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
  methods: {
    getHeaderState() {
      return this.showHeader ? this.$style.main : this.$style.mainNoHeader;
    },
  },
};
</script>

<style module lang="scss" scoped>
  .slim-no-padding {
    margin-top: -3.625em;
  }
  .main {
    padding: 3.125em 0em 3.125em 2.0px;
  }
  .mainNoHeader {
    padding: 0em 0em 3.125em 2.0px;
  }
  .slim-no-padding {
    margin-top: -3.625em;
  }
</style>
