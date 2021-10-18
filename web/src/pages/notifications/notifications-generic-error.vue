<template>
  <no-return-flow-layout>
    <div v-if="showTemplate">
      <p>{{ $t('notifications.error.select')
      }}<strong>{{ $t('notifications.error.continue')
      }}</strong>{{ $t('notifications.error.goTo')
      }}<strong>{{ $t('notifications.error.settings')
      }}</strong>{{ $t('notifications.error.tryAgain') }}.
      </p>
      <primary-button id="btn_continue" @click="onContinue">
        {{ $t('generic.continue') }}
      </primary-button>
    </div>
  </no-return-flow-layout>
</template>

<script>
import PrimaryButton from '@/components/PrimaryButton';
import NoReturnFlowLayout from '@/layouts/no-return-flow-layout';
import NativeApp from '@/services/native-app';
import RedirectMixin from '@/components/RedirectMixin';

export default {
  name: 'NotificationsGenericError',
  components: {
    NoReturnFlowLayout,
    PrimaryButton,
  },
  mixins: [RedirectMixin],
  mounted() {
    NativeApp.showHeaderSlim();
    NativeApp.hideWhiteScreen();
  },
  methods: {
    onContinue() {
      this.$store.dispatch('notifications/addNotificationCookie');

      this.conditionalRedirect();
    },
  },
};
</script>
