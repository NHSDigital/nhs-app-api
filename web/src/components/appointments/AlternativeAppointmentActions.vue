<template>
  <error-screen-alternative-actions
    alternative-actions-header="forbiddenErrors.appointments.whatYouCanDoNext">
    <template v-slot:items>
      <corona-virus-menu-item v-if="showCoronavirusItem"/>
      <gp-advice-menu-item v-if="isCdssAdvice" route-crumb="appointmentsCrumb"/>
      <admin-help-menu-item v-if="isCdssAdmin"/>
      <gp-advice-menu-item v-if="showEngageMedicalAdvice" route-crumb="appointmentsCrumb"/>
      <admin-help-menu-item
        v-if="showEngageAdminHelp" description="appointments
          .guidance.additionalGpServices.engage.getSickNotesAndLetters"/>
      <one-one-one-service-menu-item />
    </template>
  </error-screen-alternative-actions>
</template>

<script>
import sjrIf from '@/lib/sjrIf';
import CoronaVirusMenuItem from '@/components/menuItems/CoronaVirusMenuItem';
import GpAdviceMenuItem from '@/components/menuItems/GpAdviceMenuItem';
import AdminHelpMenuItem from '@/components/menuItems/AdminHelpMenuItem';
import OneOneOneServiceMenuItem from '@/components/menuItems/OneOneOneServiceMenuItem';
import ErrorScreenAlternativeActions from '@/components/errors/ErrorScreenAlternativeActions';

export default {
  name: 'AppointmentAlternativeActions',
  components: {
    AdminHelpMenuItem,
    CoronaVirusMenuItem,
    ErrorScreenAlternativeActions,
    GpAdviceMenuItem,
    OneOneOneServiceMenuItem,
  },
  props: {
    showCoronavirusItem: {
      type: Boolean,
      default: true,
    },
  },
  computed: {
    isCdssAdmin() {
      return sjrIf({ $store: this.$store, journey: 'cdssAdmin' });
    },
    isCdssAdvice() {
      return sjrIf({ $store: this.$store, journey: 'cdssAdvice' });
    },
    showEngageMedicalAdvice() {
      return sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'engage',
          serviceType: 'consultations',
        },
      });
    },
    showEngageAdminHelp() {
      return sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'engage',
          serviceType: 'consultationsAdmin',
        },
      });
    },
  },
};
</script>
