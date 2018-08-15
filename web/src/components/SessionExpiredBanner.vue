<template>
  <div :class="$style['error-timeout']" data-purpose="session-timeout">
    {{ $t('icons.sessionExpired.title') }}
  </div>
</template>

<style module lang="scss" scoped>
@import "../style/textstyles";
@import "../style/colours";
.error-timeout {
  background: $warning_title;
  @include default_text;
  color: #000;
  font-size: 1.125em;
  text-align: center;
  height: 2.5em;
  line-height: 2.5em;
  width: 100%;
  margin-top: 0.1em;
}

</style>
