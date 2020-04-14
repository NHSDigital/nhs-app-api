<template>
  <div data-id="app-panel">
    <p><strong data-id="panel-title">
      {{ $t('web.home.appStorePanelTitle') }}</strong>
    </p>
    <a data-id="apple-store-icon" href="https://apps.apple.com/gb/app/nhs-app/id1388411277"
       target="_blank" rel="noopener noreferrer"
       class="nhsuk-u-margin-bottom-1 nhsuk-u-margin-top-2">
      <img alt="Download on the App Store"
           src="~assets/download-on-the-app-store.png">
    </a>
    <a data-id="play-store-icon" href="https://play.google.com/store/apps/details?id=com.nhs.online.nhsonline" target="_blank"
       rel="noopener noreferrer">
      <img alt="Get it on Google Play"
           src="~assets/google-play-store.png">
    </a>
  </div>
</template>

<script>
export default {
  name: 'DownloadAppPanel',
};
</script>

<style scoped>
  a {
    display: inline-block;
  }
  a:hover {
    color: transparent;
    background: none;
    box-shadow: 0 0 0 4px transparent;
  }
</style>
