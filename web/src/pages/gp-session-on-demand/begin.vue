<template>
  <div>
    <div class="nhsuk-width-container">
      <div class="nhsuk-grid-row">
        <div ref="mainContent" tabindex="-1" class="nhsuk-grid-column-two-thirds">
          <div id="maincontent">
            <div tabindex="-1">
              <main>
                <spinner :always-show="true"/>
              </main>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import AuthorisationService from '@/services/authorisation-service';
import NativeApp from '@/services/native-app';
import { UPDATE_TITLE, EventBus } from '@/services/event-bus';
import Spinner from '@/components/widgets/Spinner';

export default {
  components: {
    Spinner,
  },
  layout: 'nhsuk-layout',
  async mounted() {
    const authorisationService = new AuthorisationService(this.$store.$env);
    const redirectTo = this.$router.currentRoute.query.targetPage;
    const cookies = this.$store.$cookies;

    const { token } = await this.$store.app.$http
      .postV1PatientAssertedLoginIdentity({
        assertedLoginIdentityRequest: {
          IntendedRelyingPartyUrl: window.location.hostname,
        },
      });

    if (NativeApp.supportsCreateOnDemandGpSession()) {
      authorisationService.setAuthCookieForNativeOnDemandGpSession(cookies);
      NativeApp.createOnDemandGpSession(JSON.stringify({
        redirectTo,
        assertedLoginIdentity: token,
      }));
      return;
    }

    EventBus.$emit(UPDATE_TITLE, 'navigation.pages.titles.pageLoading');
    const { gpSessionConnectUrl } = authorisationService.generateGpSessionUrl({
      redirectTo,
      cookies,
      singleSignOnDetails: {
        assertedLoginIdentity: token,
        prompt: 'none',
      },
    });

    window.location = gpSessionConnectUrl;
  },
};
</script>
