<template>
  <error-screen-alternative-actions
    v-if="showAlternativeActions" id="alternative_actions"
    alternative-actions-header="forbiddenErrors.appointments.whatYouCanDoNext">
    <template #items>
      <gp-advice-menu-item v-if="isCdssAdvice" route-crumb="appointmentsCrumb"/>
      <admin-help-menu-item v-if="isCdssAdmin"/>
      <gp-advice-menu-item v-if="showEngageMedicalAdvice" route-crumb="appointmentsCrumb"/>
      <admin-help-menu-item
        v-if="showEngageAdminHelp" description="appointments
          .guidance.additionalGpServices.engage.getSickNotesAndLetters"/>
      <gp-advice-menu-item v-if="showAccurxMedicalAdvice"
                           heading="adviceCheck.gpAdvice.accurx.heading"
                           description="adviceCheck.gpAdvice.accurx.description"
                           route-crumb="appointmentsCrumb"/>
      <messages-menu-item v-if="showAccurxAdminHelp"
                          text="messages.adminAdvice.accurx.text"
                          description="messages.adminAdvice.accurx.description" />

      <third-party-jump-off-button v-if="showPatchsMedicalAdvice"
                                   id="btn_patchs_medical_advice"
                                   provider-id="patchs"
                                   :provider-configuration="thirdPartyProvider.patchs.medical"/>
      <one-one-one-service-menu-item v-if="showOneOneOneItem" />
    </template>
  </error-screen-alternative-actions>
</template>

<script>
import sjrIf from '@/lib/sjrIf';
import GpAdviceMenuItem from '@/components/menuItems/GpAdviceMenuItem';
import AdminHelpMenuItem from '@/components/menuItems/AdminHelpMenuItem';
import MessagesMenuItem from '@/components/menuItems/MessagesMenuItem';
import OneOneOneServiceMenuItem from '@/components/menuItems/OneOneOneServiceMenuItem';
import ErrorScreenAlternativeActions from '@/components/errors/ErrorScreenAlternativeActions';
import ThirdPartyJumpOffButton from '@/components/ThirdPartyJumpOffButton';
import jumpOffProperties from '@/lib/third-party-providers/jump-off-configuration';

export default {
  name: 'AppointmentAlternativeActions',
  components: {
    AdminHelpMenuItem,
    ErrorScreenAlternativeActions,
    GpAdviceMenuItem,
    MessagesMenuItem,
    OneOneOneServiceMenuItem,
    ThirdPartyJumpOffButton,
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
    showAccurxMedicalAdvice() {
      return sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'accurx',
          serviceType: 'consultations',
        },
      });
    },
    showAccurxAdminHelp() {
      return sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'accurx',
          serviceType: 'messages',
        },
      });
    },
    showPatchsMedicalAdvice() {
      return sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'patchs',
          serviceType: 'consultations',
        },
      });
    },
    showOneOneOneItem() {
      return sjrIf({ $store: this.$store, journey: 'oneOneOne' });
    },
    thirdPartyProvider() {
      return jumpOffProperties.thirdPartyProvider;
    },
    showAlternativeActions() {
      return this.isCdssAdvice || this.isCdssAdmin ||
      this.showEngageMedicalAdvice || this.showEngageAdminHelp ||
      this.showAccurxMedicalAdvice || this.showAccurxAdminHelp ||
      this.showOneOneOneItem || this.showPatchsMedicalAdvice;
    },
  },
};
</script>
