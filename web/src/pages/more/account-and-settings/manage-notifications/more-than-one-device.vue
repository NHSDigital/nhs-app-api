<template>
  <div v-if="showTemplate">
    <p class="nhsuk-u-margin-top-3">
      {{ $t('moreThanOneDevice.mainHeading') }}
    </p>
    <p>
      {{ $t('moreThanOneDevice.description') }}
    </p>
  </div>
</template>
<script>
export default {
  name: 'MoreThanOneDevicePage',
  layout: 'nhsuk-layout',
};
</script>
