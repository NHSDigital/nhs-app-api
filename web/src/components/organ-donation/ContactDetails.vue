<template>
  <div>
    <h3>{{ $t('organDonation.reviewYourDecision.contactDetails.subheader') }}</h3>
    <p>{{ $t('organDonation.reviewYourDecision.contactDetails.text') }}</p>
    <h4 class="nhsuk-heading-xs nhsuk-u-margin-0">
      {{ $t('organDonation.reviewYourDecision.contactDetails.addressheader') }}
    </h4>
    <p>{{ address }}</p>
    <p>
      {{ $t('organDonation.reviewYourDecision.contactDetails.changeDetailsText') }}
    </p>
  </div>
</template>

<script>
export default {
  name: 'ContactDetails',
  props: {
    address: {
      type: String,
      default: '',
    },
  },
};
</script>
