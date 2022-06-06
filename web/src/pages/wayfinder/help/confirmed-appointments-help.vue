<template>
  <div>
    <h2 id="missingAppointmentsHelpTitle">
      {{ $t('wayfinder.wayfinderHelp.confirmedAppointmentsHelp.missingAppointmentsHelpTitle') }}
    </h2>
    <p id="missingAppointmentsHelpText">
      {{ $t('wayfinder.wayfinderHelp.confirmedAppointmentsHelp.missingAppointmentsHelpText') }}
    </p>
    <p id="under18HelpText">
      {{ $t('wayfinder.wayfinderHelp.confirmedAppointmentsHelp.under18HelpText') }}
    </p>
    <h2 id="changeNotShowingHelpTitle">
      {{ $t('wayfinder.wayfinderHelp.confirmedAppointmentsHelp.changeNotShowingHelpTitle') }}
    </h2>
    <p id="changeNotShowingHelpText">
      {{ $t('wayfinder.wayfinderHelp.confirmedAppointmentsHelp.changeNotShowingHelpText') }}
    </p>
    <p id="changeNeedApprovalHelpText">
      {{ $t('wayfinder.wayfinderHelp.confirmedAppointmentsHelp.changeNeedApprovalHelpText') }}
    </p>
    <desktop-generic-back-link v-if="!isNativeApp"
                               id="desktopBackLink"
                               data-purpose="back-to-wayfinder-button"
                               :path="wayfinderPath"
                               :button-text="'generic.back'"/>
  </div>
</template>

<script>
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import { WAYFINDER_PATH } from '@/router/paths';

export default {
  name: 'ConfirmedAppointmentsHelp',
  components: {
    DesktopGenericBackLink,
  },
  data() {
    return {
      wayfinderPath: WAYFINDER_PATH,
    };
  },
  computed: {
    isNativeApp() {
      return this.$store.state.device.isNativeApp;
    },
  },
};
</script>
