<template>
  <div :class="[$style.welcomeInfo, !$store.state.device.isNativeApp && $style.desktopWeb]"
       data-sid="welcome-info-proxy">
    <p class="nhsuk-u-margin-bottom-0">
      <strong>
        {{ $t('home.age') }}:
      </strong>
      <span data-sid="proxy-user-age">
        {{ proxyAge }}
      </span>
      <br>
    </p>

    <p>
      <strong>
        {{ $t('home.gpSurgery') }}:
      </strong>
      <span data-sid="proxy-user-surgery">
        {{ proxyDetails.gpPracticeName }}
      </span>
    </p>
  </div>
</template>

<script>

export default {
  name: 'ProxyWelcomeSection',
  props: {
    name: {
      type: String,
      default: '',
    },
    proxyDetails: {
      type: Object,
      default: null,
    },
    proxyAge: {
      type: String,
      default: '',
    },
  },
};
</script>

<style module lang="scss">

  .welcomeInfo {
    padding-bottom: 1em;
  }
</style>
