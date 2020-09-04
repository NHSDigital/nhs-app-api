<template>
  <div :class="[$style['error-timeout'],
                this.$store.state.device.isNativeApp ? $style['center'] : '']">
    <div class="nhsuk-width-container">
      <div class="nhsuk-grid-row">
        <div class="nhsuk-grid-column-full">
          <p data-purpose="session-timeout">
            {{ $t('login.forYourSecurityYouNeedToLogInAgain') }}
          </p>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'SessionExpiredBanner',
};
</script>
<style module lang="scss" scoped>
@import "../style/textstyles";
@import "../style/colours";
.error-timeout {
  background: $warning_title;
  @include default_text;
  color: #000;
  width: 100%;
  margin-top: 0.1em;
  padding: 0.3em;
}

.center {
  text-align: center;
}
</style>
