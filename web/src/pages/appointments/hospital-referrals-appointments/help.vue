<template>
  <div>
    <h2 id="referralsHelpTitle">
      {{ $t('wayfinder.wayfinderHelp.referralsHelp.referralsHelpTitle') }}
    </h2>
    <collapsible-dialog id="missingReferralsExpander">
      <div id="missingReferralsExpanderTitle" slot="header">
        {{ $t('wayfinder.wayfinderHelp.referralsHelp.missingReferralsExpanderTitle') }}
      </div>
      <p id="missingReferralsText">
        {{ $t('wayfinder.wayfinderHelp.referralsHelp.missingReferralsText') }}
      </p>
    </collapsible-dialog>
    <collapsible-dialog id="incorrectOrCancelledReferralsExpander">
      <div id="incorrectOrCancelledReferralsExpanderTitle" slot="header">
        {{ $t('wayfinder.wayfinderHelp.referralsHelp.incorrectOrCancelledReferralsExpanderTitle') }}
      </div>
      <p id="cancelledReferralsContactText">
        {{ $t('wayfinder.wayfinderHelp.referralsHelp.cancelledReferralsContactText') }}
      </p>
    </collapsible-dialog>
    <h2 id="appointmentsHelpTitle">
      {{ $t('wayfinder.wayfinderHelp.appointmentsHelp.appointmentsHelpTitle') }}
    </h2>
    <collapsible-dialog id="missingAppointmentsExpander">
      <div id="missingAppointmentsExpanderTitle" slot="header">
        {{ $t('wayfinder.wayfinderHelp.appointmentsHelp.missingAppointmentsExpanderTitle') }}
      </div>
      <p id="missingAppointmentsTextOne">
        {{ $t('wayfinder.wayfinderHelp.appointmentsHelp.missingAppointmentsTextOne') }}
      </p>
      <p id="missingAppointmentsTextTwo">
        {{ $t('wayfinder.wayfinderHelp.appointmentsHelp.missingAppointmentsTextTwo') }}
      </p>
    </collapsible-dialog>
    <collapsible-dialog id="incorrectChangedCancelledAppointmentsExpander">
      <div id="incorrectChangedCancelledAppointmentsExpanderTitle" slot="header">
        {{ $t('wayfinder.wayfinderHelp.appointmentsHelp.incorrectChangedCancelledAppointmentsExpanderTitle') }}
      </div>
      <p id="incorrectChangedCancelledAppointmentsTextOne">
        {{ $t('wayfinder.wayfinderHelp.appointmentsHelp.incorrectChangedCancelledAppointmentsTextOne') }}
      </p>
      <p id="incorrectChangedCancelledAppointmentsTextTwo">
        {{ $t('wayfinder.wayfinderHelp.appointmentsHelp.incorrectChangedCancelledAppointmentsTextTwo') }}
      </p>
      <p id="incorrectChangedCancelledAppointmentsTextThree">
        {{ $t('wayfinder.wayfinderHelp.appointmentsHelp.incorrectChangedCancelledAppointmentsTextThree') }}
      </p>
      <p id="incorrectChangedCancelledAppointmentsTextFour">
        {{ $t('wayfinder.wayfinderHelp.appointmentsHelp.incorrectChangedCancelledAppointmentsTextFour') }}
      </p>
    </collapsible-dialog>
    <desktop-generic-back-link v-if="!isNativeApp"
                               id="desktopBackLink"
                               data-purpose="back-to-wayfinder-button"
                               :path="wayfinderPath"
                               :button-text="'generic.back'"
                               @clickAndPrevent="backLinkClicked"/>
  </div>
</template>

<script>
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import { WAYFINDER_PATH } from '@/router/paths';
import CollapsibleDialog from '@/components/widgets/collapsible/CollapsibleDialog';
import { redirectTo } from '@/lib/utils';

let backLinkOverride;

export default {
  name: 'WayfinderHelp',
  components: {
    DesktopGenericBackLink,
    CollapsibleDialog,
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
  beforeDestroy() {
    this.$store.dispatch('navigation/clearBackLinkOverride');
    this.$store.dispatch('navigation/setRouteCrumb', 'defaultCrumb');
  },
  methods: {
    backLinkClicked() {
      backLinkOverride = this.$store.state.navigation.backLinkOverride;
      this.$store.dispatch('navigation/clearBackLinkOverride');
      this.$store.dispatch('navigation/setRouteCrumb', 'defaultCrumb');
      redirectTo(this, backLinkOverride || this.wayfinderPath);
    },
  },
};
</script>
