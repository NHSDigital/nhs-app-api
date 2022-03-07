<template>
  <div id="other-services">
    <h2 class="nhsuk-u-margin-bottom-3">
      {{ $t('login.forHealthInformation') }}
    </h2>
    <p>{{ $t('login.toFindOutMoreAboutHealthConditions') }}</p>
    <div class="nhsuk-action-link">
      <a class="nhsuk-action-link__link"
         :class="$style['nhsuk-action-link__link']"
         :href="goToNhsWebsiteUrl">
        <svg class="nhsuk-icon nhsuk-icon__arrow-right-circle" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" aria-hidden="true" width="36" height="36">
          <path d="M0 0h24v24H0z" fill="none"/>
          <!--eslint-disable-next-line max-len-->
          <path d="M12 2a10 10 0 0 0-9.95 9h11.64L9.74 7.05a1 1 0 0 1 1.41-1.41l5.66 5.65a1 1 0 0 1 0 1.42l-5.66 5.65a1 1 0 0 1-1.41 0 1 1 0 0 1 0-1.41L13.69 13H2.05A10 10 0 1 0 12 2z"/>
        </svg>
        <span class="nhsuk-action-link__text">{{ $t('login.goToTheNhsWebsite') }}</span>
      </a>
    </div>
  </div>
</template>
<script>
export default {
  name: 'OtherServices',
  data() {
    return {
      goToNhsWebsiteUrl: this.$store.$env.GO_TO_NHS_WEBSITE,
    };
  },

};
</script>

<style module lang="scss" scoped>
  @import "@/style/custom/other-services";
</style>
