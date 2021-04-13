<template>
  <div id="appRoot">
    <router-view/>
  </div>
</template>

<script>
export default {
  name: 'App',
  mounted() {
    this.setScaleValues();
  },
  methods: {
    setScaleValues() {
      const viewport = document.getElementsByName('viewport')[0];
      let content = 'width=device-width, initial-scale=1, minimum-scale=1.0';
      if (navigator.userAgent.indexOf('nhsapp-android') !== -1 || navigator.userAgent.indexOf('nhsapp-ios') !== -1) {
        content += ', maximum-scale=1.0, user-scalable=0';
      }
      viewport.setAttribute('content', content);
    },
  },
};
</script>

<style lang="scss">
  @import "~nhsuk-frontend/packages/nhsuk";
  @import "./style/links";
  @import "./style/desktopWeb/focus";
</style>
