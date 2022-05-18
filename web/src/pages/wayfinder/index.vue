<template>
  <div v-if="hasLoaded" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <div class="nhsuk-u-padding-top-4">

        <template v-if="hasErrored">

          <template v-if="showDoesNotMeetMinimumAgeError">
            <p>{{ $t('wayfinder.errors.doesNotMeetMinimumAge') }}</p>
          </template>

          <template v-else>
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
          </template>
          <other-available-services-menu-items />
        </template>

        <template v-else-if="hasReferralsOrAppointments">
          <h2 id="book-Or-Manage-Referrals-And-Appointments-Title">
            {{ $t('wayfinder.bookOrManageReferralsAndAppointmentsTitle') }}
          </h2>
          <wayfinder-help-link
            id="btn_missingOrIncorrectReferralsOrAppointments"
            :href="referralsOrAppointmentsHelpPath"
            :click-func="redirectToReferralsOrAppointmentsHelp"
            :text="$t('appointments.guidance.missingOrIncorrectReferralsOrAppointments.' +
              'ReferralsOrAppointments')"/>
          <book-or-manage-referrals-or-appointments-card
            :referrals-not-in-review="referralsNotInReview"
            :referrals-in-review="referralsInReview"
            :has-referrals-not-in-review="hasReferralsNotInReview"
            :unconfirmed-appointments="unconfirmedAppointments"
            :has-unconfirmed-appointments="hasUnconfirmedAppointments"/>

          <h2 id="confirmed-appointments-title">
            {{ $t('wayfinder.confirmedAppointmentsTitle') }}
          </h2>
          <wayfinder-help-link
            id="btn_missingOrIncorrectConfirmedAppointments"
            :href="confirmedAppointmentsHelpPath"
            :click-func="redirectToConfirmedAppointmentsHelp"
            :text="$t('appointments.guidance.missingOrIncorrectReferralsOrAppointments.' +
              'ConfirmedAppointments')"/>
          <confirmed-appointments-card
            :confirmed-appointments="confirmedAppointments"
            :has-confirmed-appointments="hasConfirmedAppointments"/>

          <h2 id="referrals-in-review-title">
            {{ $t('wayfinder.inReviewTitle') }}
          </h2>
          <wayfinder-help-link
            id="btn_missingOrIncorrectReferralsInReview"
            :href="referralsInReviewHelpPath"
            :click-func="redirectToReferralsInReviewHelp"
            :text="$t('appointments.guidance.missingOrIncorrectReferralsOrAppointments.' +
              'ReferralsInReview')"/>
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
          <other-available-services-menu-items
            id="other-available-services-menu-items"
            :show-ers="false"/>
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
import WayfinderHelpLink from '@/components/wayfinder/WayfinderHelpLink';

import { redirectTo } from '@/lib/utils';
import {
  APPOINTMENTS_PATH,
  WAYFINDER_REFERRALS_OR_APPOINTMENTS_HELP_PATH,
  WAYFINDER_CONFIRMED_APPOINTMENTS_HELP_PATH,
  WAYFINDER_REFERRALS_IN_REVIEW_HELP_PATH,
} from '@/router/paths';

const loadData = async (store) => {
  await store.dispatch('wayfinder/load');
};

export default {
  name: 'WayfinderPage',
  components: {
    WayfinderHelpLink,
    DesktopGenericBackLink,
    GenericButton,
    OtherAvailableServicesMenuItems,
    BookOrManageReferralsOrAppointmentsCard,
    ConfirmedAppointmentsCard,
    ReferralsInReviewCard,
  },
  data() {
    return {
      isNativeApp: this.$store.state.device.isNativeApp,
      referralsInReviewHelpPath: WAYFINDER_REFERRALS_IN_REVIEW_HELP_PATH,
      confirmedAppointmentsHelpPath: WAYFINDER_CONFIRMED_APPOINTMENTS_HELP_PATH,
      referralsOrAppointmentsHelpPath: WAYFINDER_REFERRALS_OR_APPOINTMENTS_HELP_PATH,
      appoinmentsHubPath: APPOINTMENTS_PATH,
    };
  },
  computed: {
    apiError() {
      return this.$store.state.wayfinder.apiError;
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
      return this.apiError !== null;
    },
    showDoesNotMeetMinimumAgeError() {
      return this.hasErrored && this.apiError && this.apiError.status === 470;
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
  },
  async mounted() {
    await loadData(this.$store);

    if (this.hasErrored) {
      const header = 'wayfinder.errors.cannotViewOrManageReferralsAndAppointments';
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
    ariaLabelCaption(header, body) {
      return `${this.$t(header)}. ${this.$t(body)}`;
    },
    redirectToReferralsOrAppointmentsHelp() {
      this.$router.push(this.WAYFINDER_REFERRALS_OR_APPOINTMENTS_HELP_PATH);
    },
    redirectToConfirmedAppointmentsHelp() {
      this.$router.push(this.WAYFINDER_CONFIRMED_APPOINTMENTS_HELP_PATH);
    },
    redirectToReferralsInReviewHelp() {
      this.$router.push(this.WAYFINDER_REFERRALS_IN_REVIEW_HELP_PATH);
    },
  },
};
</script>
