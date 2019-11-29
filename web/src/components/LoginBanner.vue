<template>
  <div>
    <h2 class="nhsuk-u-margin-bottom-0">{{ $t('loginBanner.alreadyHaveNHSLogin') }}</h2>
    <div>
      <nhs-arrow-banner :banner-text="$t('loginBanner.loginLink')"
                        :is-analytics-tracked="true"
                        :click-action="hasAnAccountLinkClicked"
                        :open-new-window="false"/>
    </div>
  </div>
</template>
<script>
import NhsArrowBanner from '@/components/widgets/NhsArrowBanner';
import { LOGIN } from '@/lib/routes';
import { setCookie } from '@/lib/cookie-manager';
import NativeCallbacks from '@/services/native-app';
import moment from 'moment';


export default {
  name: 'LoginBanner',
  components: {
    NhsArrowBanner,
  },
  methods: {
    hasAnAccountLinkClicked() {
      setCookie({
        cookies: this.$store.app.$cookies,
        key: 'BetaCookie',
        value: {
          Skipped: true,
        },
        options: {
          maxAge: moment.duration(1, 'y').asSeconds(),
          secure: this.$store.app.$env.SECURE_COOKIES,
        },
      });
      if (process.client) {
        NativeCallbacks.storeBetaCookie();
      }

      this.goToUrl(LOGIN.path);
    },
  },
};
</script>
