<template>
  <div>
    <h3>{{ $t('organDonation.reviewYourDecision.faith.subheader') }}</h3>
    <h4 class="nhsuk-heading-xs nhsuk-u-margin-0">
      {{ $t('organDonation.reviewYourDecision.faith.description') }}
    </h4>
    <p>{{ $t(`organDonation.reviewYourDecision.faith.declaration.${declaration}`) }}</p>
  </div>
</template>
<script>
export default {
  name: 'FaithDetails',
  props: {
    declaration: {
      type: String,
      required: true,
    },
  },
};
</script>
