<template>
  <div>
    <h3>{{ $t('organDonation.faith.faithSlashBeliefsDetails') }}</h3>
    <h4 class="nhsuk-heading-xs nhsuk-u-margin-0">
      {{ $t('organDonation.faith.iWouldLikeStaffToSpeakToMyFamilyAboutFaithSlashBeliefs') }}
    </h4>
    <p>{{ $t(decision) }}</p>
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
  computed: {
    decision() {
      switch (this.declaration) {
        case 'Yes':
          return 'organDonation.faith.yesThisIsApplicable';
        case 'No':
          return 'organDonation.faith.noThisIsNotApplicable';
        default:
          return 'organDonation.faith.preferNotToSay';
      }
    },
  },
};
</script>
