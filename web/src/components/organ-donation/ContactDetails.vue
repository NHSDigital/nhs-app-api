<template>
  <div>
    <h3>{{ $t('organDonation.contactDetails.contactDetails') }}</h3>
    <p>{{ $t('organDonation.contactDetails.onlyContactAboutOrganDonation') }}</p>
    <h4 class="nhsuk-heading-xs nhsuk-u-margin-0">
      {{ $t('organDonation.contactDetails.postalAddress') }}
    </h4>
    <p>{{ address }}</p>
    <p>
      {{ $t('organDonation.contactDetails.contactSurgeryToAmendPostalAddress') }}
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
