<template>
  <div id="other-services">
    <p>{{ $t('login.otherServicesYouCanUse') }}</p>
    <ul>
      <li class="nhsuk-u-margin-bottom-3">
        <a href="https://www.nhs.uk/conditions/" target="_blank" rel="noopener noreferrer">
          {{ $t('login.searchConditionsAndTreatments') }}
        </a>
      </li>
    </ul>
  </div>
</template>
<script>
export default {
  name: 'OtherServices',
};
</script>

<style module lang="scss" scoped>
  @import "@/style/custom/other-services";
</style>
