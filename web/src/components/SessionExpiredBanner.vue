<template>
  <p :class="$style['error-timeout']" data-purpose="session-timeout">
    {{ $t('icons.sessionExpired.title') }}
  </p>
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
  text-align: center;
  width: 100%;
  margin-top: 0.1em;
  padding: 0.3em;
}

</style>
