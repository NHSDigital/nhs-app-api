<template>
  <div id="other-services">
    <p>{{ $t('login.otherServicesYouCanUse') }}</p>
    <ul>
      <li class="nhsuk-u-margin-bottom-3">
        <a href="https://111.nhs.uk/service/COVID-19/" target="_blank" rel="noopener noreferrer">
          {{ $t('login.checkIfYouHaveCoronavirusSymptoms') }}
        </a>
      </li>
      <li class="nhsuk-u-margin-bottom-3">
        <a href="https://www.nhs.uk/conditions/" target="_blank" rel="noopener noreferrer">
          {{ $t('login.searchConditionsAndTreatments') }}
        </a>
      </li>
      <li class="nhsuk-u-margin-bottom-3">
        <a href="https://111.nhs.uk/" target="_blank" rel="noopener noreferrer">
          {{ $t('login.useNhs111Online') }}
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
