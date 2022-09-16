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
          <h2 id="book-Or-Manage-Referrals-And-Appointments-Title"
              class="nhsuk-u-padding-bottom-5">
            {{ $t('wayfinder.bookOrManageReferralsAndAppointmentsTitle') }}
          </h2>
          <wayfinder-help-link
            id="wayfinder-help-jump-off-link-referrals-or-appointments"
            :href="wayfinderHelpPath"
            :click-func="redirectToWayfinderHelp"
            :text="$t('wayfinder.wayfinderHelp.indexPageJumpOffLinks.referralsOrAppointments')"/>
          <book-or-manage-referrals-or-appointments-group/>

          <h2 id="confirmed-appointments-title"
              class="nhsuk-u-padding-bottom-5">
            {{ $t('wayfinder.confirmedAppointmentsTitle') }}
          </h2>
          <wayfinder-help-link
            id="wayfinder-help-jump-off-link-appointments"
            :href="wayfinderHelpPath"
            :click-func="redirectToWayfinderHelp"
            :text="$t('wayfinder.wayfinderHelp.indexPageJumpOffLinks.appointments')"/>
          <confirmed-appointments-group/>

          <h2 id="referrals-in-review-title"
              class="nhsuk-u-padding-bottom-5">
            {{ $t('wayfinder.inReviewTitle') }}
          </h2>
          <wayfinder-help-link
            id="wayfinder-help-jump-off-link-referrals"
            :href="wayfinderHelpPath"
            :click-func="redirectToWayfinderHelp"
            :text="$t('wayfinder.wayfinderHelp.indexPageJumpOffLinks.referrals')"/>
          <referrals-in-review-group/>

          <other-available-services-menu-items
            id="other-available-services-menu-items"
            :show-ers="false"/>
        </template>
      </div>

      <desktop-generic-back-link v-if="!isNativeApp"
                                 id="desktopBackLink"
                                 data-purpose="back-to-appointments-hub-button"
                                 :path="appoinmentsHubPath"
                                 :button-text="'generic.back'"/>
    </div>
  </div>
</template>

<script>
import { EventBus, UPDATE_HEADER, UPDATE_TITLE } from '@/services/event-bus';
import GenericButton from '@/components/widgets/GenericButton';
import OtherAvailableServicesMenuItems from '@/components/wayfinder/OtherAvailableServicesMenuItems';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import BookOrManageReferralsOrAppointmentsGroup from '@/components/wayfinder/sections/BookOrManageReferralsOrAppointmentsGroup';
import ConfirmedAppointmentsGroup from '@/components/wayfinder/sections/ConfirmedAppointmentsGroup';
import ReferralsInReviewGroup from '@/components/wayfinder/sections/ReferralsInReviewGroup';
import WayfinderHelpLink from '@/components/wayfinder/WayfinderHelpLink';

import { redirectTo } from '@/lib/utils';
import {
  APPOINTMENTS_PATH,
  WAYFINDER_HELP_PATH,
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
    BookOrManageReferralsOrAppointmentsGroup,
    ConfirmedAppointmentsGroup,
    ReferralsInReviewGroup,
  },
  data() {
    return {
      isNativeApp: this.$store.state.device.isNativeApp,
      wayfinderHelpPath: WAYFINDER_HELP_PATH,
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
    redirectToWayfinderHelp() {
      redirectTo(this, this.wayfinderHelpPath);
    },
  },
};
</script>
