<template>
  <div class="warning-content-panel">
    <div class="warning-content-panel__content">
      <h1 ref="warningContentPanelHeader"
          tabindex="-1"
          class="nhsuk-u-padding-top-0 nhsuk-u-padding-bottom-0 nhsuk-u-margin-bottom-4">
        <p class="nhsuk-u-padding-top-4 nhsuk-caption-l whiteText"
           data-purpose="warning-pre-header">
          <slot name="preHeader"/>
        </p>
        <slot name="header"/>
      </h1>
      <slot/>
    </div>
  </div>
</template>

<script>
export default {
  name: 'WarningContentPanel',
  mounted() {
    if (document.activeElement !== null) {
      document.activeElement.blur();
    }
    this.$refs.warningContentPanelHeader.focus();
  },
};
</script>

<style lang="scss">
  @import "@/style/custom/warning-content-panel";
</style>
