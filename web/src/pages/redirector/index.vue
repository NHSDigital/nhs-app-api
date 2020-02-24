<template>
  <div/>
</template>

<script>
import get from 'lodash/fp/get';
import { UriBuilder } from 'uribuilder';
import { REDIRECT_PARAMETER, INDEX, findByName } from '@/lib/routes';

export default {
  async mounted() {
    const redirectPath = get(REDIRECT_PARAMETER)(this.$route.query);
    if (redirectPath === undefined) {
      this.$router.push(INDEX.path);
    } else if (findByName(redirectPath)) {
      this.$router.push(findByName(redirectPath).path);
    } else {
      this.checkKnownServices(redirectPath);
    }
  },
  methods: {
    async checkKnownServices(redirectPath) {
      const services = await this.$store.state.knownServices.knownServices
        .filter(service => redirectPath.includes(service.url));
      if ((services[0].requiresAssertedLoginIdentity || {}) === true) {
        await this.$store.app.$http
          .postV1PatientAssertedLoginIdentity({
            assertedLoginIdentityRequest: {
              intendedRelyingPartyUrl: redirectPath,
            },
          })
          .then((response) => {
            '// eslint-disable-line prefer-template';

            window.location = this.appendAssertedLoginIdentity(redirectPath, response.token);
          });
      } else {
        this.$router.push(INDEX.path);
      }
    },
    appendAssertedLoginIdentity(uri, token) {
      const builder = UriBuilder.parse(uri);

      builder.query.assertedLoginIdentity = token;

      return builder.toString();
    },
  },
};
</script>
