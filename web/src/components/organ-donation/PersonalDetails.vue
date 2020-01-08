<template>
  <div>
    <h3>{{ $t('organDonation.reviewYourDecision.personalDetails.subheader') }}</h3>
    <h4 class="nhsuk-heading-xs nhsuk-u-margin-0">
      {{ $t('organDonation.reviewYourDecision.personalDetails.nameheader') }}
    </h4>
    <p>{{ name }}</p>
    <h4 class="nhsuk-heading-xs nhsuk-u-margin-0">
      {{ $t('organDonation.reviewYourDecision.personalDetails.dateofbirthheader') }}
    </h4>
    <p>{{ dateOfBirth | longDate }}</p>
    <h4 class="nhsuk-heading-xs nhsuk-u-margin-0">
      {{ $t('organDonation.reviewYourDecision.personalDetails.genderheader') }}
    </h4>
    <p>{{ gender }}</p>
    <h4 class="nhsuk-heading-xs nhsuk-u-margin-0">
      {{ $t('organDonation.reviewYourDecision.personalDetails.nhsnumberheader') }}
    </h4>
    <p>{{ nhsNumber }}</p>
    <p>
      {{ $t('organDonation.reviewYourDecision.personalDetails.description') }}
    </p>
  </div>
</template>

<script>
export default {
  name: 'PersonalDetails',
  props: {
    name: {
      type: String,
      default: '',
    },
    dateOfBirth: {
      type: String,
      default: '',
    },
    gender: {
      type: String,
      default: '',
    },
    nhsNumber: {
      type: String,
      default: '',
    },
  },
};
</script>
