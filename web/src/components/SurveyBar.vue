<template>
  <div :class="$style.surveyBanner" :style="{ bottom: setBottom() }" data-purpose="survey">
    <div :class="$style.surveyTab" data-purpose="tabForToggle">
      <div ref="tab" :class="surveyStyle()" data-purpose="toggleContent" @click="toggleTab()"/>
    </div>
    <div ref="content" :class="$style.surveyContent" data-purpose="content">
      <p data-purpose="info" >{{ $t('surveyBar.barText') }}
        <a :href="hotJarLinkUrl" data-purpose="link" target="_blank">
          {{ $t('surveyBar.linkText') }}</a>
      </p>
    </div>
  </div>
</template>
<script>
export default {
  data() {
    return {
      open: true,
      hotJarLinkUrl: process.env.HOT_JAR_URL,
    };
  },
  methods: {
    toggleTab() {
      // using refs in html to grab elements
      if (this.$refs.content.style.display === 'none') {
        // will show the banner
        this.$refs.content.style.display = '';
        this.open = true;
      } else {
        // will hide the banner
        this.$refs.content.style.display = 'none';
        this.open = false;
      }
    },
    setBottom() {
      if (this.$store.state.device.isNativeApp) {
        return '0em';
      }
      return '4.4em';
    },
    surveyStyle() {
      // check if the banner is hidden or not to determine arrow
      if (this.open) {
        return this.$style.surveyTabHandleOpened;
      }
      return this.$style.surveyTabHandleClosed;
    },
  },
};
</script>
<style lang="scss" module>
  @import "../style/surveybar";
</style>
