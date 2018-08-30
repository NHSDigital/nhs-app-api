<template>
  <div :class="$style.surveyBanner" :style="{ bottom: setBottom() }" data-purpose="survey">
    <div :class="$style.surveyTab" data-purpose="tabForToggle">
      <div :class="[open ? $style.surveyTabHandleOpened : $style.surveyTabHandleClosed]"
           data-purpose="toggleContent" @click="toggleTab()"/>
    </div>
    <div :class="[$style.surveyContent, !open ? $style.closed : undefined]"
         data-purpose="content">
      <p data-purpose="info" >{{ $t('surveyBar.barText') }}
        <a :href="hotJarLinkUrl" data-purpose="link" target="_blank">
          {{ $t('surveyBar.linkText') }}</a>
      </p>
    </div>
  </div>
</template>
<script>
export default {
  props: {
    initialBarStatusOpen: {
      default: true,
      type: Boolean,
    },
  },
  data() {
    return {
      open: true,
      hotJarLinkUrl: process.env.HOT_JAR_URL,
    };
  },
  created() {
    this.open = this.initialBarStatusOpen;
  },
  methods: {
    toggleTab() {
      this.open = !this.open;
      this.$emit('onBarStatusChanged', this.open);
    },
    setBottom() {
      if (this.$store.state.device.isNativeApp) {
        return '0em';
      }
      return '4.375em';
    },
  },
};
</script>
<style lang="scss" module scoped>
  @import "../style/surveybar";
</style>
