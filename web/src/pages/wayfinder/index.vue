<template>
  <div v-if="hasLoaded" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <div class="nhsuk-u-padding-top-4">

        <template v-if="hasErrored">
          <p>{{ $t('wayfinder.errors.cannotViewTryAgain') }}</p>

          <generic-button
            id="try-again-button"
            :class="['nhsuk-button', 'nhsuk-button--secondary']"
            @click="tryAgainClicked">
            {{ $t('generic.tryAgain') }}
          </generic-button>

          <p>
            <a id="contact-us-link"
               :href="contactUsLink"
               :aria-label="contactUsAriaLabel"
               @click.stop.prevent="contactUsClicked">
              {{ contactUsLinkText }}
            </a>
          </p>

          <h2>{{ $t('wayfinder.otherReferralsAppointmentsAndServices') }}</h2>
          <p>{{ $t('wayfinder.errors.mayHaveOtherServicesAvailableNotShown') }}</p>

          <other-available-services-menu-items />
        </template>

        <template v-else-if="hasReferralsOrAppointments">
          <h2 id="book-Or-Manage-Referrals-And-Appointments-Title">
            {{ $t('wayfinder.bookOrManageReferralsAndAppointmentsTitle') }}
          </h2>
          <book-or-manage-referrals-or-appointments-card
            :referrals-not-in-review="referralsNotInReview"
            :has-referrals-not-in-review="hasReferralsNotInReview"
            :unconfirmed-appointments="unconfirmedAppointments"
            :has-unconfirmed-appointments="hasUnconfirmedAppointments"/>

          <h2 id="confirmed-appointments-title">
            {{ $t('wayfinder.confirmedAppointmentsTitle') }}
          </h2>
          <confirmed-appointments-card
            :confirmed-appointments="confirmedAppointments"
            :has-confirmed-appointments="hasConfirmedAppointments"/>

          <h2 id="referrals-in-review-title">
            {{ $t('wayfinder.inReviewTitle') }}
          </h2>
          <referrals-in-review-card
            :referrals-in-review="referralsInReview"
            :has-referrals-in-review="hasReferralsInReview"/>
        </template>

        <template v-else>
          <p id="you-may-have-other-referrals-text">
            {{ $t('wayfinder.youMayHaveOtherReferrals') }}
          </p>
          <p id="contact-the-organisation-text">
            {{ $t('wayfinder.contactTheOrganisation') }}
          </p>
          <p id="contact-the-healthcare-provider-text">
            {{ $t('wayfinder.contactTheHealthcareProvider') }}
          </p>
          <h2 id="other-referrals-appointments-and-services-header">
            {{ $t('wayfinder.otherReferralsAppointmentsAndServices') }}
          </h2>
          <other-available-services-menu-items id="other-available-services-menu-items" />
        </template>
      </div>

      <desktopGenericBackLink v-if="!isNativeApp"
                              id="desktopBackLink"
                              data-purpose="back-to-appointments-hub-button"
                              :path="appoinmentsHubPath"
                              :button-text="'generic.back'"
                              @clickAndPrevent="backClicked"/>
    </div>
  </div>
</template>

<script>
import { EventBus, UPDATE_HEADER, UPDATE_TITLE } from '@/services/event-bus';
import GenericButton from '@/components/widgets/GenericButton';
import OtherAvailableServicesMenuItems from '@/components/wayfinder/OtherAvailableServicesMenuItems';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import BookOrManageReferralsOrAppointmentsCard from '@/components/wayfinder/sections/BookOrManageReferralsOrAppointmentsCard';
import ConfirmedAppointmentsCard from '@/components/wayfinder/sections/ConfirmedAppointmentsCard';
import ReferralsInReviewCard from '@/components/wayfinder/sections/ReferralsInReviewCard';
import { redirectTo } from '@/lib/utils';
import { APPOINTMENTS_PATH } from '@/router/paths';

const loadData = async (store) => {
  await store.dispatch('wayfinder/load');
};

export default {
  name: 'WayfinderPage',
  components: {
    DesktopGenericBackLink,
    GenericButton,
    OtherAvailableServicesMenuItems,
    BookOrManageReferralsOrAppointmentsCard,
    ConfirmedAppointmentsCard,
    ReferralsInReviewCard,
  },
  computed: {
    apiError() {
      return this.$store.state.wayfinder.apiError;
    },
    appoinmentsHubPath() {
      return APPOINTMENTS_PATH;
    },
    contactUsAriaLabel() {
      const errorCode = this.apiError.serviceDeskReference.split('');
      return this.$t('wayfinder.errors.contactUs', { errorCode });
    },
    contactUsLink() {
      return `${this.$store.$env.CONTACT_US_URL}?errorcode=${this.apiError.serviceDeskReference}`;
    },
    contactUsLinkText() {
      const errorCode = this.apiError.serviceDeskReference;
      return this.$t('wayfinder.errors.contactUs', { errorCode });
    },
    hasErrored() {
      return this.apiError !== undefined;
    },
    hasLoaded() {
      return this.$store.state.wayfinder.hasLoaded;
    },
    hasReferralsOrAppointments() {
      return this.hasReferralsInReview
      || this.hasReferralsNotInReview
      || this.hasConfirmedAppointments
      || this.hasUnconfirmedAppointments;
    },
    referralsInReview() {
      return this.$store.state.wayfinder.summary.referralsInReview;
    },
    hasReferralsInReview() {
      return this.referralsInReview.length >= 1;
    },
    referralsNotInReview() {
      return this.$store.state.wayfinder.summary.referralsNotInReview;
    },
    hasReferralsNotInReview() {
      return this.referralsNotInReview.length >= 1;
    },
    unconfirmedAppointments() {
      return this.$store.state.wayfinder.summary.unconfirmedAppointments;
    },
    hasUnconfirmedAppointments() {
      return this.unconfirmedAppointments.length >= 1;
    },
    confirmedAppointments() {
      return this.$store.state.wayfinder.summary.confirmedAppointments;
    },
    hasConfirmedAppointments() {
      return this.confirmedAppointments.length >= 1;
    },
    isNativeApp() {
      return this.$store.state.device.isNativeApp;
    },
  },
  async mounted() {
    await loadData(this.$store);

    if (this.hasErrored) {
      const header = 'wayfinder.errors.cannotViewAndManageReferralsAndAppointments';
      EventBus.$emit(UPDATE_HEADER, header);
      EventBus.$emit(UPDATE_TITLE, header);
    } else if (!this.hasReferralsOrAppointments) {
      const header = 'wayfinder.noReferralsOrAppointments';
      EventBus.$emit(UPDATE_HEADER, header);
      EventBus.$emit(UPDATE_TITLE, header);
    }
  },
  beforeDestroy() {
    this.$store.dispatch('wayfinder/init');
  },
  methods: {
    tryAgainClicked() {
      this.$router.go();
    },
    contactUsClicked() {
      window.open(this.contactUsLink, '_blank', 'noopener,noreferrer');
    },
    backClicked() {
      redirectTo(this, this.appoinmentsHubPath);
    },
  },
};
</script>
