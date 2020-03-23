<template>
  <div v-if="showTemplate">
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <div v-if="showSummaryDetails" class="nhsuk-do-dont-list nhsuk-u-margin-top-3">
          <h2 :class="[$style['user-info'], 'nhsuk-u-margin-top-3 nhsuk-u-margin-bottom-3']">
            {{ $t('linkedProfiles.featuresOnBehalfOf.text')
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
          {{ $t('linkedProfiles.featuresNoSummary')
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
import { INDEX, LINKED_PROFILES } from '@/lib/routes';

export default {
  layout: 'nhsuk-layout',
  components: {
    GreenTick,
    RedCross,
    GenericButton,
  },
  data() {
    return {
      linkedAccount: this.$store.getters['linkedAccounts/getSelectedLinkedAccount'],
    };
  },
  computed: {
    featuresOnBehalfOf() {
      return [{
        id: 'book-an-appointment',
        text: 'linkedProfiles.featuresOnBehalfOf.bookAnAppointment',
        canDo: this.linkedAccount.canBookAppointment,
      }, {
        id: 'order-repeat-prescription',
        text: 'linkedProfiles.featuresOnBehalfOf.orderRepeatPrescription',
        canDo: this.linkedAccount.canOrderRepeatPrescription,
      }, {
        id: 'view-medical-record',
        text: 'linkedProfiles.featuresOnBehalfOf.viewMedicalRecord',
        canDo: this.linkedAccount.canViewMedicalRecord,
      }];
    },
    displayPersonalisedButton() {
      return this.linkedAccount.displayPersonalizedContent;
    },
    switchButtonText() {
      return this.displayPersonalisedButton
        ? this.$t('linkedProfiles.switchProfileButton', { givenName: this.linkedAccount.givenName })
        : this.$t('linkedProfiles.switchProfileButtonWithoutName');
    },
    showSummaryDetails() {
      return this.linkedAccount.showSummary;
    },
  },
  asyncData({ store, redirect }) {
    if (store.state.linkedAccounts.selectedLinkedAccount === null) {
      return redirect(302, LINKED_PROFILES.path, null);
    }

    return store.dispatch(
      'linkedAccounts/loadAccountAccessSummary',
      store.state.linkedAccounts.selectedLinkedAccount.id,
    );
  },
  mounted() {
    this.$store.dispatch('header/updateHeaderText', this.$t('pageHeaders.linkedProfilesSummary').replace('{fullName}', this.linkedAccount.fullName));
    this.$store.dispatch('pageTitle/updatePageTitle', this.$t('pageTitles.linkedProfilesSummary').replace('{fullName}', this.linkedAccount.fullName));
  },
  beforeDestroy() {
    this.$store.dispatch('linkedAccounts/clearSelectedLinkedAccount');
  },
  methods: {
    async switchProfileButtonClicked() {
      await this.$store.dispatch('linkedAccounts/switchProfile', this.linkedAccount);
      await this.$store.dispatch('serviceJourneyRules/loadLinkedAccount');
      redirectTo(this, INDEX.path);
    },
  },
};
</script>

<style module lang="scss" scoped>
  .user-info {
    display: inline-block;
  }
</style>
