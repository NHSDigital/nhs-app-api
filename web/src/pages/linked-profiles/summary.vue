<template>
  <div v-if="showTemplate">
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">

        <generic-button id="btn-switch-profile"
                        :button-classes="['nhsuk-button', 'nhsuk-button--secondary']"
                        @click.stop.prevent="switchProfileButtonClicked">
          {{ $t('linkedProfiles.switchProfileButton') }}
        </generic-button>

        <span class="nhsuk-label nhsuk-u-padding-bottom-0 nhsuk-u-margin-bottom-0 ">
          {{ $t('linkedProfiles.informationHeaders.dob') }}:
        </span>
        <span id="user-date-of-birth"
              :class="[$style['user-info'], 'nhsuk-u-padding-top-0 nhsuk-u-padding-bottom-3']">
          {{ linkedAccount.dateOfBirth | longDate }}
        </span>

        <span class="nhsuk-label nhsuk-u-padding-bottom-0 nhsuk-u-margin-bottom-0 ">
          {{ $t('linkedProfiles.informationHeaders.nhsNumber') }}:
        </span>
        <span id="user-nhs-number"
              :class="[$style['user-info'], 'nhsuk-u-padding-top-0 nhsuk-u-padding-bottom-3']">
          {{ linkedAccount.nhsNumber }}
        </span>

        <div v-if="linkedAccount.gpPracticeName">
          <span class="nhsuk-label nhsuk-u-padding-bottom-0 nhsuk-u-margin-bottom-0 ">
            {{ $t('linkedProfiles.informationHeaders.gpPractice') }}:
          </span>
          <span id="user-gp-practice"
                :class="[$style['user-info'], 'nhsuk-u-padding-top-0 nhsuk-u-padding-bottom-3']">
            {{ linkedAccount.gpPracticeName }}
          </span>
        </div>

        <div class="nhsuk-do-dont-list">
          <span :class="[$style['user-info'], 'nhsuk-u-margin-top-3 nhsuk-u-margin-bottom-3']">
            {{ `${$t('linkedProfiles.thingsYouCanDoOnBehalfOf.text')} ${linkedAccount.name}` }}
          </span>
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
      </div>
    </div>
  </div>
</template>

<script>
import GreenTick from '@/components/icons/GreenTick';
import RedCross from '@/components/icons/RedCross';
import GenericButton from '@/components/widgets/GenericButton';
import { LINKED_PROFILES } from '@/lib/routes';

export default {
  layout: 'nhsuk-layout',
  components: {
    GreenTick,
    RedCross,
    GenericButton,
  },
  data() {
    return {
      linkedAccount: this.$store.state.linkedAccounts.selectedLinkedAccount,
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
    const accountName = this.linkedAccount.name;
    this.$store.dispatch('header/updateHeaderText', accountName);
    this.$store.dispatch('pageTitle/updatePageTitle', accountName);
  },
  beforeDestroy() {
    this.$store.dispatch('linkedAccounts/clearSelectedLinkedAccount');
  },
  methods: {
    switchProfileButtonClicked() {
    },
  },
};
</script>

<style module lang="scss" scoped>
  .user-info {
    font-weight: 600;
    font-size: 20px;
    display: inline-block;
  }
</style>
