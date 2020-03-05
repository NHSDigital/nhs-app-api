<template>
  <div v-if="showTemplate">
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <div class="nhsuk-do-dont-list nhsuk-u-margin-top-3">
          <p :class="[$style['user-info'], 'nhsuk-u-margin-top-3 nhsuk-u-margin-bottom-3']">
            {{ `${$t('linkedProfiles.thingsYouCanDoOnBehalfOf.text')} ${linkedAccount.fullName}` }}
          </p>
          <ul v-for="item in thingsYouCanDoOnBehalfOf"
              :key="item.text"
              class="nhsuk-list nhsuk-list--cross">
            <li :id="item.id">
              <Green-Tick v-if="item.canDo" />
              <Red-Cross v-else />
              {{ $t(item.text) }}
            </li>
          </ul>
        </div>
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
    thingsYouCanDoOnBehalfOf() {
      return [{
        id: 'book-an-appointment',
        text: 'linkedProfiles.thingsYouCanDoOnBehalfOf.bookAnAppointment',
        canDo: this.linkedAccount.canBookAppointment,
      }, {
        id: 'order-repeat-prescription',
        text: 'linkedProfiles.thingsYouCanDoOnBehalfOf.orderRepeatPrescription',
        canDo: this.linkedAccount.canOrderRepeatPrescription,
      }, {
        id: 'view-medical-record',
        text: 'linkedProfiles.thingsYouCanDoOnBehalfOf.viewMedicalRecord',
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
