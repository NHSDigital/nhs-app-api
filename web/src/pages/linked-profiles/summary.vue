<template>
  <div v-if="showTemplate">
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <div v-if="showSummaryDetails" class="nhsuk-do-dont-list nhsuk-u-margin-top-3">
          <h2 :class="[$style['user-info'], 'nhsuk-u-margin-top-3 nhsuk-u-margin-bottom-3']">
            {{ $t('profiles.servicesYouCanAccessForName')
              .replace('{fullName}', linkedAccount.fullName ) }}
          </h2>
          <ul v-for="item in featuresOnBehalfOf"
              :key="item.text"
              class="nhsuk-list nhsuk-list--cross">
            <li :id="item.id">
              <Green-Tick v-if="item.canDo" />
              <Red-Cross v-else />
              {{ $t(item.text) }}
            </li>
          </ul>
        </div>
        <p v-else :class="[$style['user-info'], 'nhsuk-u-margin-top-3 nhsuk-u-margin-bottom-3']">
          {{ $t('profiles.toAccessServicesForNameYouNeedToUseTheirProfile')
            .replace('{fullName}', linkedAccount.fullName ) }}
        </p>
        <generic-button id="btn-switch-profile"
                        :button-classes="['nhsuk-button']"
                        @click.stop.prevent="switchProfileButtonClicked">
          {{ switchButtonText }}
        </generic-button>
      </div>
    </div>
  </div>
</template>

<script>
import GreenTick from '@/components/icons/GreenTick';
import RedCross from '@/components/icons/RedCross';
import GenericButton from '@/components/widgets/GenericButton';
import { redirectTo } from '@/lib/utils';
import { INDEX_PATH, LINKED_PROFILES_PATH } from '@/router/paths';

export default {
  components: {
    GreenTick,
    RedCross,
    GenericButton,
  },
  data() {
    return {
      linkedAccount: {},
    };
  },
  computed: {
    featuresOnBehalfOf() {
      return [{
        id: 'book-an-appointment',
        text: 'profiles.bookAGpAppointment',
        canDo: this.linkedAccount.canBookAppointment,
      }, {
        id: 'order-repeat-prescription',
        text: 'profiles.orderARepeatPrescription',
        canDo: this.linkedAccount.canOrderRepeatPrescription,
      }, {
        id: 'view-medical-record',
        text: 'profiles.viewTheirGpHealthRecord',
        canDo: this.linkedAccount.canViewMedicalRecord,
      }];
    },
    displayPersonalisedButton() {
      return this.linkedAccount.displayPersonalizedContent;
    },
    switchButtonText() {
      return this.displayPersonalisedButton
        ? this.$t('profiles.switchToNamesProfile', { givenName: this.linkedAccount.givenName })
        : this.$t('profiles.switchToThisProfile');
    },
    showSummaryDetails() {
      return this.linkedAccount.showSummary;
    },
  },
  async mounted() {
    if (this.$store.state.linkedAccounts.selectedLinkedAccount === null) {
      redirectTo(this, LINKED_PROFILES_PATH);
    } else {
      await this.$store.dispatch(
        'linkedAccounts/loadAccountAccessSummary',
        this.$store.state.linkedAccounts.selectedLinkedAccount.id,
      );
      this.linkedAccount = this.$store.getters['linkedAccounts/getSelectedLinkedAccount'];
    }
  },
  beforeDestroy() {
    this.$store.dispatch('linkedAccounts/clearSelectedLinkedAccount');
  },
  methods: {
    async switchProfileButtonClicked() {
      await this.$store.dispatch('linkedAccounts/switchProfile', this.linkedAccount);
      redirectTo(this, INDEX_PATH);
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "@/style/custom/linked-profiles-summary";
</style>
