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
import { UPDATE_HEADER, UPDATE_TITLE, EventBus } from '@/services/event-bus';
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
  mounted() {
    // TODO: move into route header/title callbacks. don't think there's a need for this here
    EventBus.$emit(UPDATE_HEADER, this.$t('navigation.pages.headers.switchProfile', this.currentProfile), true);
    EventBus.$emit(UPDATE_TITLE, this.$t('navigation.pages.titles.switchProfile', this.currentProfile), true);
  },
};
</script>
