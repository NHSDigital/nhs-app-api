<template>
  <div id="other-services">
    <p>{{ $t('web.home.otherServicesTitle') }}</p>
    <ul>
      <li class="nhsuk-u-margin-bottom-3">
        <a href="https://111.nhs.uk/service/COVID-19/" target="_blank" rel="noopener noreferrer">
          {{ $t('web.home.otherServicesBullets.one') }}
        </a>
      </li>
      <li class="nhsuk-u-margin-bottom-3">
        <a href="https://www.nhs.uk/conditions/" target="_blank" rel="noopener noreferrer">
          {{ $t('web.home.otherServicesBullets.two') }}
        </a>
      </li>
      <li class="nhsuk-u-margin-bottom-3">
        <a href="https://111.nhs.uk/" target="_blank" rel="noopener noreferrer">
          {{ $t('web.home.otherServicesBullets.three') }}
        </a>
      </li>
    </ul>
  </div>
</template>
<script>
export default {
  name: 'OtherServices',
};
</script>
<style module lang="scss" scoped>
    @import "../style/home";
    @import "../style/spacings";
    @import "../style/webshared";

    @include desktopAndBelow() {
    }

    @include fromDesktop() {
    }

    a {
        display: inline-block;
    }

</style>
