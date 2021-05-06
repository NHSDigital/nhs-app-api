<template>
  <error-screen-alternative-actions
    v-if="showAlternativeActions" id="alternative_actions"
    alternative-actions-header="forbiddenErrors.appointments.whatYouCanDoNext">
    <template v-slot:items>
      <gp-advice-menu-item v-if="isCdssAdvice" route-crumb="appointmentsCrumb"/>
      <admin-help-menu-item v-if="isCdssAdmin"/>
      <gp-advice-menu-item v-if="showEngageMedicalAdvice" route-crumb="appointmentsCrumb"/>
      <admin-help-menu-item
        v-if="showEngageAdminHelp" description="appointments
          .guidance.additionalGpServices.engage.getSickNotesAndLetters"/>
      <one-one-one-service-menu-item v-if="showOneOneOneItem" />
    </template>
  </error-screen-alternative-actions>
</template>

<script>
import sjrIf from '@/lib/sjrIf';
import GpAdviceMenuItem from '@/components/menuItems/GpAdviceMenuItem';
import AdminHelpMenuItem from '@/components/menuItems/AdminHelpMenuItem';
import OneOneOneServiceMenuItem from '@/components/menuItems/OneOneOneServiceMenuItem';
import ErrorScreenAlternativeActions from '@/components/errors/ErrorScreenAlternativeActions';

export default {
  name: 'AppointmentAlternativeActions',
  components: {
    AdminHelpMenuItem,
    ErrorScreenAlternativeActions,
    GpAdviceMenuItem,
    OneOneOneServiceMenuItem,
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
    showOneOneOneItem() {
      return sjrIf({ $store: this.$store, journey: 'oneOneOne' });
    },
    showAlternativeActions() {
      return this.isCdssAdvice || this.isCdssAdmin ||
      this.showEngageMedicalAdvice || this.showEngageAdminHelp || this.showOneOneOneItem;
    },
  },
};
</script>
