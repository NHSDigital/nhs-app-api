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
@import '~nhsuk-frontend/packages/core/settings/colours';
@import '~nhsuk-frontend/packages/core/settings/globals';
@import '~nhsuk-frontend/packages/core/settings/spacing';
@import '~nhsuk-frontend/packages/core/tools/ifff';
@import '~nhsuk-frontend/packages/core/tools/sass-mq';
@import '~nhsuk-frontend/packages/core/tools/spacing';
@import "../../style/colours";

.whiteText {
  color: $color_nhsuk-white !important;
}

.warning-content-panel {
  @include nhsuk-responsive-margin(4, "top");
  @include nhsuk-responsive-margin(4, "bottom");
  width: 100%;
  color: $color_nhsuk-white;
  background-color: $panel_blue;
  .warning-content-panel__content {
    @include nhsuk-responsive-padding(2);
    @include nhsuk-responsive-padding(4, "left");
    @include nhsuk-responsive-padding(4, "right");
    strong {
      display: inline;
    }

    a.nhsuk-button {
      color: $nhsuk-text-color;
    }

    a.inline-link {
      color: $color_nhsuk-white;
      text-decoration: underline;
      display: inline;
      &:visited {
        color: $color_nhsuk-white;
      }
      &:focus {
        color: $nhsuk-text-color;
      }
      &:hover {
        color: $nhsuk-text-color;
        text-decoration: none;
      }
    }
  }
}
</style>
