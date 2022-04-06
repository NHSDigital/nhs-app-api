<template>
  <div v-if="showTemplate" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <div v-if="hasNoReferralsOrAppointments" class="nhsuk-u-padding-top-4">
        <p>
          {{ $t('appointments.wayfinder.youMayHaveOtherReferrals') }}
        </p>

        <p>
          {{ $t('appointments.wayfinder.contactTheOrganisation') }}
        </p>

        <p>
          {{ $t('appointments.wayfinder.contactTheHealthcareProvider') }}
        </p>

        <h2>
          {{ $t('appointments.wayfinder.otherReferralsAppointmentsAndServices') }}
        </h2>

        <menu-item-list>
          <third-party-jump-off-button v-if="showManageYourReferral"
                                       id="btn_manage_your_referral"
                                       provider-id="ers"
                                       :provider-configuration="thirdPartyProvider.ers.
                                         manageYourReferralWayfinder" />

          <third-party-jump-off-button v-if="showPkbAppointments"
                                       id="btn_pkb_appointments"
                                       provider-id="pkb"
                                       :provider-configuration="thirdPartyProvider.pkb.
                                         appointments" />

          <third-party-jump-off-button v-if="showPkbCieAppointments"
                                       id="btn_pkb_cie_appointments"
                                       provider-id="pkb"
                                       :provider-configuration="thirdPartyProvider.pkb.
                                         appointmentsCie" />

          <third-party-jump-off-button v-if="showPkbSecondaryCareAppointments"
                                       id="btn_pkb_secondary_care_appointments"
                                       provider-id="pkb"
                                       :provider-configuration="thirdPartyProvider.pkb.
                                         appointmentsPkbSecondaryCare" />

          <third-party-jump-off-button v-if="showPkbMyCareViewAppointments"
                                       id="btn_pkb_my_care_view_appointments"
                                       provider-id="pkb"
                                       :provider-configuration="thirdPartyProvider.pkb.
                                         appointmentsPkbMyCareView" />

          <third-party-jump-off-button v-if="showGncrAppointments"
                                       id="btn_gncr_appointments"
                                       provider-id="gncr"
                                       :provider-configuration="thirdPartyProvider.gncr.
                                         appointments" />
        </menu-item-list>
      </div>
      <div v-else class="nhsuk-u-padding-top-4">
        <h2>
          {{ $t('appointments.wayfinder.referralsSectionTitle') }}
        </h2>
        <div v-if="hasNoReferrals">
          <p>
            {{ $t('appointments.wayfinder.noReferrals.youHaveNoReferrals') }}
          </p>

          <p>
            {{ $t('appointments.wayfinder.noReferrals.youMayHaveOtherReferrals') }}
          </p>

          <p>
            {{ $t('appointments.wayfinder.noReferrals.contactTheOrganisation') }}
          </p>
        </div>
        <div v-else>
          <CardGroup class="nhsuk-grid-row">

            <CardGroupItem v-for="referral in referrals"
                           :key="referral.referralId"
                           class="nhsuk-grid-column-three-quarters">

              <InReviewReferralsCard v-if="isInReview(referral)"
                                     :requested-speciality="referral.serviceSpeciality"
                                     :referred-date="referral.referredDateTime"
                                     :review-date="referral.reviewDueDate"
                                     :booking-reference="referral.referralId"
                                     :referred-by="referral.referrerOrganisation"/>

              <ReviewOverdueReferralsCard v-if="isReviewOverdue(referral)"
                                          :requested-speciality="referral.serviceSpeciality"
                                          :referred-date="referral.referredDateTime"
                                          :review-date="referral.reviewDueDate"
                                          :booking-reference="referral.referralId"
                                          :referred-by="referral.referrerOrganisation"/>

              <ReadyToRebookReferralCard v-if="isBookableWasCancelled(referral)"
                                         :requested-speciality="referral.serviceSpeciality"
                                         :referred-date="referral.referredDateTime"
                                         :booking-reference="referral.referralId"
                                         :referred-by="referral.referrerOrganisation"/>

              <BookableReferralCard v-if="isBookable(referral)"
                                    :requested-speciality="referral.serviceSpeciality"
                                    :referred-date="referral.referredDateTime"
                                    :booking-reference="referral.referralId"
                                    :referred-by="referral.referrerOrganisation"/>
            </CardGroupItem>
          </CardGroup>
        </div>

        <div>
          <h2>
            {{ $t('appointments.wayfinder.upcomingAppointmentsSectionTitle') }}
          </h2>
          <div v-if="hasNoUpcomingAppointments">
            <p>
              {{ $t('appointments.wayfinder.noAppointments.youHaveNoAppointments') }}
            </p>

            <p>
              {{ $t('appointments.wayfinder.noAppointments.contactTheOrganisation') }}
            </p>
          </div>

          <div v-else>
            <CardGroup class="nhsuk-grid-row">
              <CardGroupItem v-for="appointment in upcomingAppointments"
                             :key="appointment.appointmentId"
                             class="nhsuk-grid-column-three-quarters">

                <AppointmentBookedCard
                  v-if="isAppointmentBooked(appointment)"
                  :location-description="appointment.locationDescription"
                  :appointment-date-time="appointment.appointmentDateTime"/>

                <AppointmentReadyToBookCard
                  v-if="!isAppointmentBooked(appointment)"
                  :location-description="appointment.locationDescription"/>

              </CardGroupItem>
            </CardGroup>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { EventBus, UPDATE_HEADER, UPDATE_TITLE } from '@/services/event-bus';
import AppointmentBookedCard from '@/components/wayfinder/appointments/AppointmentBookedCard';
import AppointmentReadyToBookCard from '@/components/wayfinder/appointments/AppointmentReadyToConfirmCard';
import CardGroup from '@/components/widgets/card/CardGroup';
import CardGroupItem from '@/components/widgets/card/CardGroupItem';
import jumpOffProperties from '@/lib/third-party-providers/jump-off-configuration';
import MenuItemList from '@/components/MenuItemList';
import ThirdPartyJumpOffButton from '@/components/ThirdPartyJumpOffButton';
import InReviewReferralsCard from '@/components/wayfinder/referrals/InReviewCard';
import ReadyToRebookReferralCard from '@/components/wayfinder/referrals/ReadyToRebookReferralCard';
import ReviewOverdueReferralsCard from '@/components/wayfinder/referrals/ReviewOverdueCard';
import BookableReferralCard from '@/components/wayfinder/referrals/BookableReferralCard';
import sjrIf from '@/lib/sjrIf';
import { isEmptyArray } from '@/lib/utils';
import moment from 'moment';

const loadData = async (store) => {
  await store.dispatch('wayfinder/load');
};

export default {
  name: 'WayfinderPage',
  components: {
    AppointmentBookedCard,
    AppointmentReadyToBookCard,
    CardGroup,
    CardGroupItem,
    InReviewReferralsCard,
    ReadyToRebookReferralCard,
    ReviewOverdueReferralsCard,
    BookableReferralCard,
    MenuItemList,
    ThirdPartyJumpOffButton,
  },
  data() {
    return {
      isProxying: this.$store.getters['session/isProxying'],
      thirdPartyProvider: jumpOffProperties.thirdPartyProvider,
    };
  },
  computed: {
    referrals() {
      return this.$store.state.wayfinder.referrals;
    },
    upcomingAppointments() {
      return this.$store.state.wayfinder.upcomingAppointments;
    },
    hasNoReferrals() {
      return isEmptyArray(this.referrals);
    },
    hasNoUpcomingAppointments() {
      return isEmptyArray(this.upcomingAppointments);
    },
    hasNoReferralsOrAppointments() {
      return this.hasNoReferrals && this.hasNoUpcomingAppointments;
    },
    hasErsAppointments() {
      return sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'ers',
          serviceType: 'secondaryAppointments',
        },
      });
    },
    hasGncrAppointments() {
      return sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'gncr',
          serviceType: 'secondaryAppointments',
        },
      });
    },
    hasPkbAppointments() {
      return sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'pkb',
          serviceType: 'secondaryAppointments',
        },
      });
    },
    hasPkbCieAppointments() {
      return sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'pkbCie',
          serviceType: 'secondaryAppointments',
        },
      });
    },
    hasPkbSecondaryCareAppointments() {
      return sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'pkbSecondaryCare',
          serviceType: 'secondaryAppointments',
        },
      });
    },
    hasPkbMyCareViewAppointments() {
      return sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'pkbMyCareView',
          serviceType: 'secondaryAppointments',
        },
      });
    },
    showGncrAppointments() {
      return this.hasGncrAppointments && !this.isProxying;
    },
    showManageYourReferral() {
      return this.hasErsAppointments && !this.isProxying;
    },
    showPkbAppointments() {
      return this.hasPkbAppointments && !this.isProxying;
    },
    showPkbCieAppointments() {
      return this.hasPkbCieAppointments && !this.isProxying;
    },
    showPkbSecondaryCareAppointments() {
      return this.hasPkbSecondaryCareAppointments && !this.isProxying;
    },
    showPkbMyCareViewAppointments() {
      return this.hasPkbMyCareViewAppointments && !this.isProxying;
    },
  },
  async mounted() {
    await loadData(this.$store);

    if (this.hasNoReferralsOrAppointments) {
      EventBus.$emit(UPDATE_HEADER, {
        headerKey: 'appointments.wayfinder.noReferralsOrAppointments',
      });
      EventBus.$emit(UPDATE_TITLE, 'appointments.wayfinder.noReferralsOrAppointments');
    }
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
  },
};
</script>
