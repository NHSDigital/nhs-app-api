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
          <h2>{{ $t('wayfinder.referralsSectionTitle') }}</h2>

          <card-group v-if="hasReferrals" class="nhsuk-grid-row">
            <card-group-item v-for="referral in referrals"
                             :key="referral.referralId"
                             class="nhsuk-grid-column-three-quarters">

              <referral-in-review-card
                v-if="isInReview(referral)"
                :requested-speciality="referral.serviceSpeciality"
                :referred-date="referral.referredDateTime"
                :review-date="referral.reviewDueDate"
                :booking-reference="referral.referralId"
                :referred-by="referral.referrerOrganisation"/>

              <referral-review-overdue-card
                v-if="isReviewOverdue(referral)"
                :requested-speciality="referral.serviceSpeciality"
                :referred-date="referral.referredDateTime"
                :review-date="referral.reviewDueDate"
                :booking-reference="referral.referralId"
                :referred-by="referral.referrerOrganisation"/>

              <referral-ready-to-rebook-card
                v-if="isBookableWasCancelled(referral)"
                :requested-speciality="referral.serviceSpeciality"
                :referred-date="referral.referredDateTime"
                :booking-reference="referral.referralId"
                :referred-by="referral.referrerOrganisation"/>

              <referral-bookable-card
                v-if="isBookable(referral)"
                :requested-speciality="referral.serviceSpeciality"
                :referred-date="referral.referredDateTime"
                :booking-reference="referral.referralId"
                :referred-by="referral.referrerOrganisation"/>
            </card-group-item>
          </card-group>

          <template v-else>
            <p>{{ $t('wayfinder.noReferrals.youHaveNoReferrals') }}</p>

            <p>{{ $t('wayfinder.noReferrals.youMayHaveOtherReferrals') }}</p>

            <p>{{ $t('wayfinder.noReferrals.contactTheOrganisation') }}</p>
          </template>

          <h2>{{ $t('wayfinder.upcomingAppointmentsSectionTitle') }}</h2>

          <card-group v-if="hasUpcomingAppointments" class="nhsuk-grid-row">
            <card-group-item
              v-for="appointment in upcomingAppointments"
              :key="appointment.appointmentId"
              class="nhsuk-grid-column-three-quarters">

              <appointment-booked-card
                v-if="isAppointmentBooked(appointment)"
                :location-description="appointment.locationDescription"
                :appointment-date-time="appointment.appointmentDateTime"/>

              <appointment-ready-to-confirm-card
                v-if="!isAppointmentBooked(appointment)"
                :location-description="appointment.locationDescription"/>

            </card-group-item>
          </card-group>

          <template v-else>
            <p>{{ $t('wayfinder.noAppointments.youHaveNoAppointments') }}</p>

            <p>{{ $t('wayfinder.noAppointments.contactTheOrganisation') }}</p>
          </template>
        </template>

        <template v-else>
          <p>{{ $t('wayfinder.youMayHaveOtherReferrals') }}</p>

          <p>{{ $t('wayfinder.contactTheOrganisation') }}</p>

          <p>{{ $t('wayfinder.contactTheHealthcareProvider') }}</p>

          <h2>{{ $t('wayfinder.otherReferralsAppointmentsAndServices') }}</h2>

          <other-available-services-menu-items />
        </template>

      </div>
    </div>
  </div>
</template>

<script>
import { EventBus, UPDATE_HEADER, UPDATE_TITLE } from '@/services/event-bus';
import GenericButton from '@/components/widgets/GenericButton';
import OtherAvailableServicesMenuItems from '@/components/wayfinder/OtherAvailableServicesMenuItems';
import CardGroup from '@/components/widgets/card/CardGroup';
import CardGroupItem from '@/components/widgets/card/CardGroupItem';
import AppointmentBookedCard from '@/components/wayfinder/appointments/AppointmentBookedCard';
import AppointmentReadyToConfirmCard from '@/components/wayfinder/appointments/AppointmentReadyToConfirmCard';
import ReferralInReviewCard from '@/components/wayfinder/referrals/ReferralInReviewCard';
import ReferralReadyToRebookCard from '@/components/wayfinder/referrals/ReferralReadyToRebookCard';
import ReferralReviewOverdueCard from '@/components/wayfinder/referrals/ReferralReviewOverdueCard';
import ReferralBookableCard from '@/components/wayfinder/referrals/ReferralBookableCard';
import { isEmptyArray } from '@/lib/utils';
import moment from 'moment';

const loadData = async (store) => {
  await store.dispatch('wayfinder/load');
};

export default {
  name: 'WayfinderPage',
  components: {
    GenericButton,
    OtherAvailableServicesMenuItems,
    CardGroup,
    CardGroupItem,
    AppointmentBookedCard,
    AppointmentReadyToConfirmCard,
    ReferralInReviewCard,
    ReferralReadyToRebookCard,
    ReferralReviewOverdueCard,
    ReferralBookableCard,
  },
  computed: {
    hasLoaded() {
      return this.$store.state.wayfinder.hasLoaded;
    },
    apiError() {
      return this.$store.state.wayfinder.apiError;
    },
    hasErrored() {
      return this.apiError !== undefined;
    },
    contactUsLink() {
      return `${this.$store.$env.CONTACT_US_URL}?errorcode=${this.apiError.serviceDeskReference}`;
    },
    contactUsLinkText() {
      const errorCode = this.apiError.serviceDeskReference;
      return this.$t('wayfinder.errors.contactUs', { errorCode });
    },
    contactUsAriaLabel() {
      const errorCode = this.apiError.serviceDeskReference.split('');
      return this.$t('wayfinder.errors.contactUs', { errorCode });
    },
    referrals() {
      return this.$store.state.wayfinder.summary.referrals;
    },
    upcomingAppointments() {
      return this.$store.state.wayfinder.summary.upcomingAppointments;
    },
    hasReferrals() {
      return !isEmptyArray(this.referrals);
    },
    hasUpcomingAppointments() {
      return !isEmptyArray(this.upcomingAppointments);
    },
    hasReferralsOrAppointments() {
      return this.hasReferrals || this.hasUpcomingAppointments;
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
    isInReview(referral) {
      return referral.status === 'InReview' && moment(referral.reviewDueDate).isSameOrAfter(moment.now());
    },
    isBookable(referral) {
      return referral.status === 'Bookable';
    },
    isBookableWasCancelled(referral) {
      return referral.status === 'BookableWasCancelled';
    },
    isReviewOverdue(referral) {
      return referral.status === 'InReview' && moment(referral.reviewDueDate).isBefore(moment.now());
    },
    isAppointmentBooked(appointment) {
      return appointment.appointmentDateTime;
    },
    tryAgainClicked() {
      this.$router.go();
    },
    contactUsClicked() {
      window.open(this.contactUsLink, '_blank', 'noopener,noreferrer');
    },
  },
};
</script>
