<template>
  <div>
    <h2 id="title1">
      {{ $t('wayfinder.wayfinderHelp.confirmedAppointmentsHelp.title1') }}
    </h2>
    <p id="text1">
      {{ $t('wayfinder.wayfinderHelp.confirmedAppointmentsHelp.text1') }}
    </p>
    <h2 id="title2">
      {{ $t('wayfinder.wayfinderHelp.confirmedAppointmentsHelp.title2') }}
    </h2>
    <p id="text2">
      {{ $t('wayfinder.wayfinderHelp.confirmedAppointmentsHelp.text2') }}
    </p>
    <p id="text3">
      {{ $t('wayfinder.wayfinderHelp.confirmedAppointmentsHelp.text3') }}
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
  methods: {
    backClicked() {
      redirectTo(this, this.wayfinderPath);
    },
  },
};
</script>
