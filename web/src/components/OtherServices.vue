<template>
  <div id="other-services">
    <p>{{ $t('login.otherServicesYouCanUse') }}</p>
    <ul>
      <li class="nhsuk-u-margin-bottom-3">
        <a :href="searchConditionUrl" target="_blank" rel="noopener noreferrer">
          {{ $t('login.searchConditionsAndTreatments') }}
        </a>
      </li>
    </ul>
  </div>
</template>
<script>
export default {
  name: 'OtherServices',
  data() {
    return {
      searchConditionUrl: this.$store.$env.CONDITIONS_CHECKER_URL,
    };
  },

};
</script>

<style module lang="scss" scoped>
  @import "@/style/custom/other-services";
</style>
