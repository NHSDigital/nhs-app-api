<template>
  <div v-if="hasLoaded" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <div class="nhsuk-u-padding-top-3">

        <template v-if="hasErrored">
          <template v-if="showDoesNotMeetMinimumAgeError">
            <p id="doesNotMeetMinimumAge">{{ $t('wayfinder.errors.doesNotMeetMinimumAge') }}</p>
            <p id="noOtherServicesShowing">{{ $t('wayfinder.errors.noOtherServicesShowing') }}</p>
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
          <other-available-services-menu-items
            id="other-available-services-menu-items"/>
        </template>

        <template v-else>
          <book-or-manage-referrals-or-appointments-group/>
          <confirmed-appointments-group/>
          <referrals-in-review-group/>

          <wait-times-menu-item
            id="wait-times-menu-item"
            :is-wait-times-enabled="isWaitTimesEnabled"/>

          <other-available-services-menu-items
            id="other-available-services-menu-items"
            :show-ers="false"/>
        </template>
      </div>

      <desktop-generic-back-link
        v-if="!isNativeApp"
        id="desktopBackLink"
        data-purpose="back-to-appointments-hub-button"
        :path="appointmentsHubPath"
        :button-text="'generic.back'"/>
    </div>
  </div>
</template>

<script>
import { EventBus, UPDATE_HEADER, UPDATE_TITLE } from '@/services/event-bus';
import GenericButton from '@/components/widgets/GenericButton';
import WaitTimesMenuItem from '@/components/appointments/hospital-referrals-appointments/WaitTimesMenuItem';
import OtherAvailableServicesMenuItems from '@/components/appointments/hospital-referrals-appointments/OtherAvailableServicesMenuItems';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import BookOrManageReferralsOrAppointmentsGroup from '@/components/appointments/hospital-referrals-appointments/sections/BookOrManageReferralsOrAppointmentsGroup';
import ConfirmedAppointmentsGroup from '@/components/appointments/hospital-referrals-appointments/sections/ConfirmedAppointmentsGroup';
import ReferralsInReviewGroup from '@/components/appointments/hospital-referrals-appointments/sections/ReferralsInReviewGroup';

import { isTruthy } from '@/lib/utils';
import {
  APPOINTMENTS_PATH,
} from '@/router/paths';

const loadData = async (store) => {
  await store.dispatch('wayfinder/load');
};

export default {
  name: 'WayfinderPage',
  components: {
    DesktopGenericBackLink,
    GenericButton,
    OtherAvailableServicesMenuItems,
    WaitTimesMenuItem,
    BookOrManageReferralsOrAppointmentsGroup,
    ConfirmedAppointmentsGroup,
    ReferralsInReviewGroup,
  },
  data() {
    return {
      isNativeApp: this.$store.state.device.isNativeApp,
      isWaitTimesEnabled: isTruthy(this.$store.$env.SECONDARY_CARE_WAIT_TIMES_ENABLED),
      appointmentsHubPath: APPOINTMENTS_PATH,
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
  },
  async mounted() {
    await loadData(this.$store);

    if (this.hasErrored) {
      const header = 'wayfinder.errors.cannotViewOrManageReferralsAndAppointments';
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
    ariaLabelCaption(header, body) {
      return `${this.$t(header)}. ${this.$t(body)}`;
    },
  },
};
</script>
