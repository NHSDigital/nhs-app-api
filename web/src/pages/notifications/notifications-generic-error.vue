<template>
  <no-return-flow-layout>
    <div v-if="showTemplate">
      <p>{{ $t('notifications.error.toTryAgain') }}</p>
      <ol>
        <li>{{ $t('notifications.error.select') }}
          <strong>{{ $t('notifications.error.continue') }}</strong>
        </li>
        <li>{{ $t('notifications.error.goTo') }}
          <strong>{{ $t('notifications.error.more') }}</strong>
        </li>
        <li>{{ $t('notifications.error.select') }}
          <strong>{{ $t('notifications.error.accountAndSettings') }}</strong>
        </li>
        <li>{{ $t('notifications.error.thenSelect') }}
          <strong>{{ $t('notifications.error.manageNotifications') }}</strong>
        </li>
        <li>{{ $t('notifications.error.turnOnThe') }}
          <strong>{{ $t('notifications.error.notifications') }}</strong>
          {{ $t('notifications.error.toggle') }}
        </li>
      </ol>
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
