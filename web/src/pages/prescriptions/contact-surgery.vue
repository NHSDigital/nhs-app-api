<template>
  <div v-if="showTemplate">
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full nhsuk-u-padding-top-3">
        <p>{{ $t("prescriptions.contactSurgery.repeatPrescriptionsOnly") }}</p>
        <p>{{ $t("prescriptions.contactSurgery.contactGPSurgery") }}</p>
        <p id="shutter-summary-text">
          {{ $t('prescriptions.contactSurgery.emergencyContact1') }} &nbsp;
          <emergency-prescriptions-link id="emergencyPrescriptions-link"/>
          {{ $t('prescriptions.contactSurgery.emergencyContact2') }}
        </p>
        <desktopGenericBackLink
          v-if="!$store.state.device.isNativeApp"
          id="backToPrescriptions-link"
          :path="getBackPath"
          :button-text="$t('prescriptions.contactSurgery.backButtonText')"
          @clickAndPrevent="backButtonClicked"
        />
      </div>
    </div>
  </div>
</template>

<script>
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import EmergencyPrescriptionsLink from '@/components/widgets/EmergencyPrescriptionsLink';
import { PRESCRIPTION_TYPE_PATH } from '@/router/paths';
import { redirectTo } from '@/lib/utils';

export default {
  name: 'ContactSurgeryPage',
  components: {
    EmergencyPrescriptionsLink,
    DesktopGenericBackLink,
  },
  computed: {
    getBackPath() {
      return PRESCRIPTION_TYPE_PATH;
    },
  },
  methods: {
    backButtonClicked() {
      redirectTo(this, this.getBackPath);
    },
  },
};
</script>
