<template>
  <div v-if="showTemplate">
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <generic-button id="switch-profile-button" :button-classes="['nhsuk-button',
                                                                     'nhsuk-button--primary']"
                        @click.stop.prevent="switchProfileButtonClicked">
          {{ $t('switchProfile.switchToMyProfileButton') }}
        </generic-button>

        <span class="nhsuk-label nhsuk-u-padding-bottom-0 nhsuk-u-margin-bottom-0 ">
          {{ $t('switchProfile.informationHeaders.dob') }}:
        </span>
        <span id="proxy-date-of-birth"
              :class="[$style['user-info'], 'nhsuk-u-padding-top-0 nhsuk-u-padding-bottom-3']">
          {{ currentProfile.dateOfBirth | longDate }}
        </span>
        <span class="nhsuk-label nhsuk-u-padding-bottom-0 nhsuk-u-margin-bottom-0 ">
          {{ $t('switchProfile.informationHeaders.nhsNumber') }}:
        </span>
        <span id="proxy-nhs-number"
              :class="[$style['user-info'], 'nhsuk-u-padding-top-0 nhsuk-u-padding-bottom-3']">
          <generic-voice-over-text-split :class="$style.fieldValue"
                                         :text="currentProfile.nhsNumber"
                                         :data-sid="'proxy-nhs-number-data'"/>
        </span>
        <div v-if="currentProfile.gpPracticeName">
          <span class="nhsuk-label nhsuk-u-padding-bottom-0 nhsuk-u-margin-bottom-0 ">
            {{ $t('switchProfile.informationHeaders.gpPractice') }}:
          </span>
          <span id="proxy-gp-practice"
                :class="[$style['user-info'], 'nhsuk-u-padding-top-0 nhsuk-u-padding-bottom-3']">
            {{ currentProfile.gpPracticeName }}
          </span>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { redirectTo } from '@/lib/utils';
import { INDEX } from '@/lib/routes';
import GenericButton from '@/components/widgets/GenericButton';
import GenericVoiceOverTextSplit from '@/components/widgets/GenericVoiceOverTextSplit';

export default {
  layout: 'nhsuk-layout',
  components: {
    GenericButton,
    GenericVoiceOverTextSplit,
  },
  data() {
    return {
      currentProfile: this.$store.state.linkedAccounts.actingAsUser,
    };
  },
  mounted() {
    this.$store.dispatch('header/updateHeaderText', this.$t('pageHeaders.switchProfile', this.currentProfile));
    this.$store.dispatch('pageTitle/updatePageTitle', this.$t('pageTitles.switchProfile', this.currentProfile));
  },
  methods: {
    async switchProfileButtonClicked() {
      const mainPatientId = this.$store.getters['linkedAccounts/mainPatientId'];
      const mainUserObject = {
        id: mainPatientId,
      };
      await this.$store.dispatch('linkedAccounts/switchToMainUserProfile', mainUserObject);
      redirectTo(this, INDEX.path);
    },
  },
};
</script>
<style module lang="scss" scoped>
@import '../../style/colours';

.user-info {
  font-weight: 600;
  font-size: 20px;
  display: inline-block;
}
</style>
