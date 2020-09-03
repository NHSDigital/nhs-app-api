<template>
  <div data-id="app-panel">
    <p><strong data-id="panel-title">
      {{ $t('login.getTheNhsAppOnYourDevice') }}</strong>
    </p>
    <ul>
      <li>
        <a data-id="apple-store-icon"
           :aria-label="$t('login.downloadOnTheAppStore')"
           href="https://apps.apple.com/gb/app/nhs-app/id1388411277"
           class="nhsuk-u-margin-bottom-2 nhsuk-u-margin-right-3"
           target="_blank" rel="noopener noreferrer">
          <img :alt="$t('login.downloadOnTheAppStore')"
               src="../../assets/download-on-the-app-store.png">
        </a>
      </li>
      <li>
        <a data-id="play-store-icon"
           :aria-label="$t('login.getItOnGooglePlay')"
           href="https://play.google.com/store/apps/details?id=com.nhs.online.nhsonline" target="_blank"
           rel="noopener noreferrer">
          <img :alt="$t('login.getItOnGooglePlay')"
               src="../../assets/google-play-store.png">
        </a>
      </li>
    </ul>
  </div>
</template>

<script>
export default {
  name: 'DownloadAppPanel',
};
</script>

<style lang="scss" scoped>
  ul {
    margin: 0;
    padding: 0;
    li {
      list-style: none;
      a {
        display: inline-block;
        vertical-align: top;
        &:focus,
        &:hover {
          border-radius: 4px;
        }
        img{
          display: block;
        }
      }
    }
  }
</style>
