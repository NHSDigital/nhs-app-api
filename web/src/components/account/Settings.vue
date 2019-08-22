<template>
  <div v-if="showTemplate" :class="[$style['no-padding'], 'pull-content']">
    <h2>{{ $t('myAccount.accountSettings.header') }}</h2>
    <ul :class="[$style['list-menu'], $style.myAccountList]">
      <li v-if="showBiometrics" :class="$style.listMenuItem" @click="goToLoginOptions()">
        <analytics-tracked-tag id="btn_passwordOptions"
                               :text="$t('myAccount.accountSettings.passwordOptions')"
                               tag="a">
          {{ $t('myAccount.accountSettings.passwordOptions') }}
        </analytics-tracked-tag>
      </li>
      <li v-if="showNotifications" :class="$style.listMenuItem" @click="showNotificationsClicked()">
        <analytics-tracked-tag id="btn_notificationOptions"
                               :text="$t('myAccount.accountSettings.notificationOptions')"
                               tag="a">
          {{ $t('myAccount.accountSettings.notificationOptions') }}
        </analytics-tracked-tag>
      </li>
    </ul>
  </div>
</template>

<script>
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import { ACCOUNT_NOTIFICATIONS } from '@/lib/routes';
import NativeCallbacks from '@/services/native-app';

export default {
  name: 'Settings',
  components: {
    AnalyticsTrackedTag,
  },
  props: {
    showBiometrics: {
      type: Boolean,
      required: true,
    },
    showNotifications: {
      type: Boolean,
      required: true,
    },
  },
  methods: {
    goToLoginOptions() {
      NativeCallbacks.goToLoginOptions();
    },
    showNotificationsClicked() {
      this.$router.push(ACCOUNT_NOTIFICATIONS.path);
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "../../style/accessibility";
  @import "../../style/listmenu";
  @import "../../style/colours";
  @import "../../style/webshared";
  @import "../../style/nhsukoverrides";

.myAccountList {
  @include inner-container-width;

  .listMenuItem {
    font-family: $default-web;
    font-weight: lighter;

    a {
      @extend .focusBorder;
      &:hover {
        color: #000;
      }
    }
  }
}
.no-padding {
  margin-top: -0.5em;
  margin-left: -1em;
  margin-right: -1em;
  padding-bottom: 1em;

  p,
  h2 {
    margin-left: 0.7em;
    margin-top: 0.5em;
  }
}
</style>
