<template>
  <div class="warning-content-panel">
    <div class="warning-content-panel__content">
      <p class="nhsuk-body-m nhsuk-u-margin-bottom-0 nhsuk-u-margin-top-0"
         data-purpose="warning-pre-header">
        <slot name="preHeader"/>
      </p>
      <h1 class="nhsuk-u-padding-top-0 nhsuk-u-padding-bottom-0">
        <slot name="header"/>
      </h1>
      <slot/>
    </div>
  </div>
</template>

<script>
export default {
  name: 'WarningContentPanel',
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
