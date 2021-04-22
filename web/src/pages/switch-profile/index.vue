<template>
  <div v-if="showTemplate">
    <div class="nhsuk-grid-row nhsuk-u-margin-bottom-4">
      <div class="nhsuk-grid-column-full">
        <p class="nhsuk-u-margin-bottom-0">
          <strong>{{ $t('profiles.age') }}:</strong>
          <span id="proxy-age"> {{ getDisplayedAgeText(currentProfile) }}</span>
        </p>

        <p v-if="currentProfile.gpPracticeName" class="nhsuk-u-margin-bottom-0">
          <strong>{{ $t('profiles.gpSurgery') }}:</strong>
          <span id="proxy-gp-practice"> {{ currentProfile.gpPracticeName }}</span>
        </p>
      </div>
    </div>

    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <switch-profile-button />
      </div>
    </div>
  </div>
</template>

<script>
import SwitchProfileButton from '@/components/switch-profile/SwitchProfileButton';
import CalculateAgeInMonthsAndYears from '../../plugins/mixinDefinitions/CalculateAgeInMonthsAndYears';

export default {
  layout: 'nhsuk-layout',
  components: {
    SwitchProfileButton,
  },
  mixins: [CalculateAgeInMonthsAndYears],
  data() {
    return {
      currentProfile: this.$store.state.linkedAccounts.actingAsUser,
    };
  },
};
</script>
