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
  @import "@/style/custom/session-expired-banner";
</style>
