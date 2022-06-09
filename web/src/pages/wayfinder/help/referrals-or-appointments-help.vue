<template>
  <div>
    <h2 id="missingAppointmentsHelpTitle">
      {{ $t('wayfinder.wayfinderHelp.referralsOrAppointmentsHelp.missingAppointmentsHelpTitle') }}
    </h2>
    <p id="missingAppointmentsHelpText">
      {{ $t('wayfinder.wayfinderHelp.referralsOrAppointmentsHelp.missingAppointmentsHelpText') }}
    </p>
    <p id="Under18HelpText">
      {{ $t('wayfinder.wayfinderHelp.referralsOrAppointmentsHelp.Under18HelpText') }}
    </p>
    <h2 id="changeNotShowingHelpTitle">
      {{ $t('wayfinder.wayfinderHelp.referralsOrAppointmentsHelp.changeNotShowingHelpTitle') }}
    </h2>
    <p id="changeNotShowingHelpText">
      {{ $t('wayfinder.wayfinderHelp.referralsOrAppointmentsHelp.changeNotShowingHelpText') }}
    </p>
    <p id="changeNeedApprovalHelpText">
      {{ $t('wayfinder.wayfinderHelp.referralsOrAppointmentsHelp.changeNeedApprovalHelpText') }}
    </p>
    <desktopGenericBackLink v-if="!isNativeApp"
                            id="desktopBackLink"
                            data-purpose="back-to-wayfinder-button"
                            :path="wayfinderPath"
                            :button-text="'generic.back'"
                            @clickAndPrevent="backClicked"/>
  </div>
</template>

<script>
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import { WAYFINDER_PATH } from '@/router/paths';
import { redirectTo } from '@/lib/utils';

export default {
  name: 'ReferralsOrAppointmentsHelp',
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
  methods: {
    backClicked() {
      redirectTo(this, this.wayfinderPath);
    },
  },
};
</script>
